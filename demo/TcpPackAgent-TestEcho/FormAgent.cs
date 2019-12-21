using System;
using System.Text;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Tcp;
using Models;
using Newtonsoft.Json;

namespace TcpPackAgentTestEcho
{
    public partial class FormAgent : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ITcpPackAgent _agent = new TcpPackAgent();
#pragma warning restore IDE0069 // 应释放可释放的字段

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


            // pack模型专有设置
            _agent.MaxPackSize = 4096;     // 最大封包
            _agent.PackHeaderFlag = 0x01;  // 包头标识, 要与客户端对应, 否则无法通信

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
            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            AddLog($"OnReceive({connId}), data length: {data.Length}");

            return OnProcessFullPacket(sender, connId, data);
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }


        /// <summary>
        /// 处理完整包, 业务逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
#pragma warning disable IDE0060 // 删除未使用的参数
        private HandleResult OnProcessFullPacket(IAgent sender, IntPtr connId, byte[] data)
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
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void Send(IntPtr connId, PacketType type, string data)
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

            // 发送数据到服务器
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
                    for (var i = 0; i < 10; i++)
                    {
                        // 这里模拟连接几个连接不上的服务器, 前面设置了超时3秒, 所以这些连接不上的到3秒会超时
                        if (!_agent.Connect(i % 2 == 0 ? "127.0.0.1" : "192.168.37.38", 5555))
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
                Send(connId, PacketType.Echo, txtContent.Text.Trim());
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            var txt = txtContent.Text.Trim();
            var connIds = _agent.GetAllConnectionIds();
            foreach (var connId in connIds)
            {
                Send(connId, PacketType.Echo, txt);
                Send(connId, PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).NextDouble());
                Send(connId, PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).Next(1000, 1000000));
                Send(connId, PacketType.Echo, txt + Guid.NewGuid());
            }
        }

        private void BtnGetServerTime_Click(object sender, EventArgs e)
        {
            var connIds = _agent.GetAllConnectionIds();
            foreach (var connId in connIds)
            {
                Send(connId, PacketType.Time, null);
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
