using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Proxy;
using HPSocket.Tcp;
using Models;
using Newtonsoft.Json;

namespace TcpAgentTestEcho
{
    public partial class FormAgent : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ITcpAgent _agent = new TcpAgent();
#pragma warning restore IDE0069 // 应释放可释放的字段

        /// <summary>
        /// 最大封包长度
        /// </summary>
        private const int MaxPacketSize = 4096;

        public FormAgent()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 演示设置agent属性

            // 缓冲区大小
            _agent.SocketBufferSize = 4096; // 4K
            // 异步连接
            _agent.Async = true;
            // 异步连接可以设置连接超时时间, 单位是毫秒
            _agent.ConnectionTimeout = 3000;

            // 注意这里是监听地址, 连接服务器的ip和端口在调用Connect()的时候传入
            _agent.Address = "0.0.0.0";

            // 这个还可以设置代理, http和socks5代理可以同时混用, 会随机挑选代理服务器, 支持无限多个
            /*
            _agent.ProxyList = new List<IProxy>
            {
                // 支持http隧道代理
                new HttpProxy
                {
                    Host = "127.0.0.1",
                    Port = 1080,
                    // 支持帐号和密码, 可选
                    // UserName = "admin",
                    // Password = "pass"
                },
                // 也支持socks5代理
                new Socks5Proxy
                {
                    Host = "127.0.0.1",
                    Port = 1080,
                    // 支持帐号和密码, 可选
                    // UserName = "admin",
                    // Password = "pass"
                }
            };
            */

            // 事件绑定
            _agent.OnPrepareConnect += OnPrepareConnect;
            _agent.OnConnect += OnConnect;
            _agent.OnReceive += OnReceive;
            _agent.OnClose += OnClose;

            AddLog($"hp-socket version: {_agent.Version}");
        }

        private void FormAgent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_agent.HasStarted)
            {
                MessageBox.Show(@"请先断开与服务器的连接", @"正在通信:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            // 停止并释放客户端
            _agent.Dispose();

            e.Cancel = false;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnPrepareConnect(IAgent sender, IntPtr connId, IntPtr socket)
        {
            AddLog($"OnPrepareConnect({connId}), socket handle: {socket}");

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnConnect(IAgent sender, IntPtr connId, IProxy proxy)
        {
            AddLog($"OnConnect({connId})");

            // 设置附加数据, 用来做粘包处理
            sender.SetExtra(connId, new ClientInfo
            {
                ConnId = connId,
                PacketData = new List<byte>(),
            });

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            AddLog($"OnReceive({connId}), data length: {data.Length}");

            var client = sender.GetExtra<ClientInfo>(connId);
            if (client == null)
            {
                return HandleResult.Error;
            }

            client.PacketData.AddRange(data);

            // 总长度小于包头
            if (client.PacketData.Count < sizeof(int))
            {
                return HandleResult.Ok;
            }

            HandleResult result;
            const int headerLength = sizeof(int);
            do
            {
                // 取头部字节得到包头
                var packetHeader = client.PacketData.GetRange(0, headerLength).ToArray();

                // 两端字节序要保持一致
                // 如果当前环境不是小端字节序
                if (!BitConverter.IsLittleEndian)
                {
                    // 翻转字节数组, 变为小端字节序
                    Array.Reverse(packetHeader);
                }

                // 得到包头指向的数据长度
                var dataLength = BitConverter.ToInt32(packetHeader, 0);

                // 完整的包长度(含包头和完整数据的大小)
                var fullLength = dataLength + headerLength;

                if (dataLength < 0 || fullLength > MaxPacketSize)
                {
                    result = HandleResult.Error;
                    break;
                }

                // 如果来的数据小于一个完整的包
                if (client.PacketData.Count < fullLength)
                {
                    // 下次数据到达处理
                    result = HandleResult.Ignore;
                    break;
                }

                // 是不是一个完整的包(包长 = 实际数据长度 + 包头长度)
                if (client.PacketData.Count == fullLength)
                {
                    // 得到完整包并处理
                    var fullData = client.PacketData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(sender, client, fullData);

                    // 清空缓存
                    client.PacketData.Clear();
                    break;
                }

                // 如果来的数据比一个完整的包长
                if (client.PacketData.Count > fullLength)
                {
                    // 先得到完整包并处理
                    var fullData = client.PacketData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(sender, client, fullData);
                    if (result == HandleResult.Error)
                    {
                        break;
                    }
                    // 再移除已经读了的数据
                    client.PacketData.RemoveRange(0, fullLength);

                    // 剩余的数据下个循环处理
                }

            } while (true);


            return result;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");

            var client = sender.GetExtra<ClientInfo>(connId);
            if (client != null)
            {
                sender.RemoveExtra(connId);
                client.PacketData.Clear();
                return HandleResult.Error;
            }

            return HandleResult.Ok;
        }

        /// <summary>
        /// 处理完整包, 业务逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
#pragma warning disable IDE0060 // 删除未使用的参数
        private HandleResult OnProcessFullPacket(IAgent sender, ClientInfo client, byte[] data)
#pragma warning restore IDE0060 // 删除未使用的参数
        {
            // 这里来的都是完整的包
            var packet = JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(data));
            var result = HandleResult.Ok;
            switch (packet.Type)
            {
                case PacketType.Echo: // 回显是个字符串显示操作
                {
                    AddLog($"OnProcessFullPacket(), type: {packet.Type}, content: {packet.Data}");
                    break;
                }
                case PacketType.Time: // 获取服务器时间依然是个字符串操作^_^
                {
                    AddLog($"OnProcessFullPacket(), type: {packet.Type}, time: {packet.Data}");
                    break;
                }
                default:
                    result = HandleResult.Error;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 发送包头
        /// <para>固定包头网络字节序</para>
        /// </summary>
        /// <param name="sender">服务器组件</param>
        /// <param name="connId">连接id</param>
        /// <param name="dataLength">实际数据长度</param>
        /// <returns></returns>
        private bool SendPacketHeader(IAgent sender, IntPtr connId, int dataLength)
        {
            // 包头转字节数组
            var packetHeaderBytes = BitConverter.GetBytes(dataLength);

            // 两端字节序要保持一致
            // 如果当前环境不是小端字节序
            if (!BitConverter.IsLittleEndian)
            {
                // 翻转字节数组, 变为小端字节序
                Array.Reverse(packetHeaderBytes);
            }

            return sender.Send(connId, packetHeaderBytes, packetHeaderBytes.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void Send(IAgent sender, IntPtr connId, PacketType type, string data)
        {
            if (!_agent.HasStarted)
            {
                return;
            }

            // 组织封包, 取得要发送的数据
            var packet = new Packet
            {
                Type = type,
                Data = data,
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));

            // 先发包头
            if (!SendPacketHeader(sender, connId, bytes.Length))
            {
                _agent.Disconnect(connId);
                return;
            }

            // 再发实际数据
            if (!_agent.Send(connId, bytes, bytes.Length))
            {
                _agent.Disconnect(connId);
            }
        }

        private async void BtnConnectSwitch_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnConnectSwitch.Text == @"连接")
                {
                    if (!_agent.Start())
                    {
                        throw new Exception($"Start() error code: {_agent.ErrorCode}, error message: {_agent.ErrorMessage}");
                    }

                    // 连接到目标服务器 
                    for (var i = 0; i < 100; i++)
                    {
                        // 这里模拟连接几个连接不上的服务器, 前面设置了超时3秒, 所以这些连接不上的到3秒会超时
                        //if (!_agent.Connect(i % 2 == 0 ? "127.0.0.1" : "192.168.37.38", 5555))
                        if (!_agent.Connect(  "127.0.0.1"  , 5555))
                        {
                            throw new Exception($"error code: {_agent.ErrorCode}, error message: {_agent.ErrorMessage}");
                        }
                    }

                    AddLog("有几个测试连接会在3秒后超时断开");

                    btnConnectSwitch.Text = @"断开";

                    // 等待服务停止
                    await _agent.WaitAsync();

                    // 停止以后还原按钮标题
                    btnConnectSwitch.Text = @"连接";
                }
                else
                {
                    // 断开所有连接并停止服务
                    await _agent.StopAsync();
                }
            }
            catch (Exception ex)
            {
                AddLog($"exception: {ex.Message}");
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            var connIds = _agent.GetAllConnectionIds();
            foreach (var connId in connIds)
            {
                Send(_agent, connId, PacketType.Echo, txtContent.Text.Trim());
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            var txt = txtContent.Text.Trim();
            var connIds = _agent.GetAllConnectionIds();
            foreach (var connId in connIds)
            {
                Send(_agent, connId, PacketType.Echo, txt);
                Send(_agent, connId, PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).NextDouble());
                Send(_agent, connId, PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).Next(1000, 1000000));
                Send(_agent, connId, PacketType.Echo, txt + Guid.NewGuid());
            }
        }

        private void BtnGetServerTime_Click(object sender, EventArgs e)
        {
            var connIds = _agent.GetAllConnectionIds();
            foreach (var connId in connIds)
            {
                Send(_agent, connId, PacketType.Time, null);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
        }

        private void AddLog(string log)
        {
            if (txtLog.IsDisposed)
            {
                return;
            }

            // 从ui线程去操作ui
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new AddLogHandler(AddLog), log);
            }
            else
            {
                txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {log}\r\n");
            }
        }

    }
}
