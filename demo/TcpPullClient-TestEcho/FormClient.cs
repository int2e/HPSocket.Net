using System;
using System.Text;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Tcp;
using Models;
using Newtonsoft.Json;

namespace TcpPullClientTestEcho
{
    public partial class FormClient : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ITcpPullClient _client = new TcpPullClient();
#pragma warning restore IDE0069 // 应释放可释放的字段

        /// <summary>
        /// 最大封包长度
        /// </summary>
        private const int MaxPacketSize = 4096;

        public FormClient()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 演示设置client属性

            // 缓冲区大小
            _client.SocketBufferSize = 4096; // 4K
            // 异步连接
            _client.Async = true;

            // 要连接的服务器地址和端口(也可以调用Connect()方法时传入服务器ip和端口)
            // 例如: _client.Connect("127.0.0.1", 555)
            _client.Address = "127.0.0.1";
            _client.Port = 5555;

            // 事件绑定
            _client.OnPrepareConnect += OnPrepareConnect;
            _client.OnConnect += OnConnect;
            _client.OnReceive += OnReceive;
            _client.OnClose += OnClose;
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (_client.HasStarted)
            {
                MessageBox.Show(@"请先断开与服务器的连接", @"正在通信:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            // 停止并释放客户端
            _client.Dispose();

            e.Cancel = false;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnPrepareConnect(IClient sender, IntPtr socket)
        {
            AddLog($"OnPrepareConnect({sender.Address}:{sender.Port}), socket handle: {socket}, hp-socket version: {sender.Version}");

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnConnect(IClient sender)
        {
            AddLog("OnConnect()");
            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnReceive(IClient sender, int length)
        {
            AddLog($"OnReceive(), data length: {length}");

            if (!(sender is ITcpPullClient client))
            {
                return HandleResult.Error;
            }

            // 封包头长度
            const int headerLength = sizeof(int);

            // 这里演示另外一种更简单的pull组件的使用方法, 与当前客户端对应的TcpPullServer的方法不同
            // TcpPullServer里用的IntPtr, 自己申请内存, 这里是out byte[]
            HandleResult result;
            do
            {
                // 窥探缓冲区, 取头部字节得到包头
                var fr = client.Peek(headerLength, out var packetHeader);
                // 连接已断开
                if (fr == FetchResult.DataNotFound)
                {
                    result = HandleResult.Error;
                    break;
                }

                // 如果来的数据长度不够封包头的长度, 等下次在处理 
                if (fr == FetchResult.LengthTooLong)
                {
                    result = HandleResult.Ignore;
                    break;
                }

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

                // 如果缓冲区数据长度不够一个完整的包, 当前不处理
                if (length < fullLength)
                {
                    result = HandleResult.Ignore;
                    break;
                }

                // 从缓冲区取数据, 注意取的是 fullLength 长的包
                fr = client.Fetch(fullLength, out var data);

                // 连接已断开
                if (fr == FetchResult.DataNotFound)
                {
                    result = HandleResult.Error;
                    break;
                }

                // 如果来的数据长度不够封包头的长度, 等下次在处理 
                if (fr == FetchResult.LengthTooLong)
                {
                    result = HandleResult.Ignore;
                    break;
                }

                // 逻辑上fr到这里必然ok

                // 注意: 现在data里是包含包头的
                // 到这里当前只处理这一个包, 其他的数据不fetch, 等下次OnReceive到达处理
                result = OnProcessFullPacket(data, headerLength);
                if (result == HandleResult.Error)
                {
                    break;
                }

                // 继续下一次循环

            } while (true);

            return result;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnClose(), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }

        /// <summary>
        /// 处理完整包, 业务逻辑
        /// </summary>
        /// <param name="data"></param>
        /// <param name="headerLength">包头长度</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private HandleResult OnProcessFullPacket(byte[] data, int headerLength)

        {
            // 这里来的都是完整的包
            // 但是因为数据是包含包头的, 所以转字符串的时候注意 Encoding.UTF8.GetString() 的用法
            var packet = JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(data, headerLength, data.Length - headerLength));
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
        /// <param name="dataLength">实际数据长度</param>
        /// <returns></returns>
        private bool SendPacketHeader(int dataLength)
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

            return _client.Send(packetHeaderBytes, packetHeaderBytes.Length);
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void Send(PacketType type, string data)
        {
            if (!_client.HasStarted)
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

            // 发送包头, 发送失败断开连接
            if (!SendPacketHeader(bytes.Length))
            {
                _client.Stop();
                return;
            }

            // 发送实际数据, 发送失败断开连接
            if (!_client.Send(bytes, bytes.Length))
            {
                _client.Stop();
            }
        }

        private async void BtnConnectSwitch_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnConnectSwitch.Text == @"连接")
                {
                    // 连接到目标服务器
                    if (!_client.Connect())
                    {
                        throw new Exception($"error code: {_client.ErrorCode}, error message: {_client.ErrorMessage}");
                    }

                    btnConnectSwitch.Text = @"断开";

                    // 等待服务停止
                    await _client.WaitAsync();

                    // 停止以后还原按钮标题
                    btnConnectSwitch.Text = @"连接";
                }
                else
                {
                    // 断开连接
                    await _client.StopAsync();
                }
            }
            catch (Exception ex)
            {
                AddLog($"exception: {ex.Message}");
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            Send(PacketType.Echo, txtContent.Text.Trim());
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            var txt = txtContent.Text.Trim();
            Send(PacketType.Echo, txt);
            Send(PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).NextDouble());
            Send(PacketType.Echo, txt + new Random(Guid.NewGuid().GetHashCode()).Next(1000, 1000000));
            Send(PacketType.Echo, txt + Guid.NewGuid());
        }

        private void BtnGetServerTime_Click(object sender, EventArgs e)
        {
            Send(PacketType.Time, null);
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
