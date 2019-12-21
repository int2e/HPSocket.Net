using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Ssl;
using Models;
using Newtonsoft.Json;

namespace SslClientTestEcho
{
    public partial class FormClient : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ISslClient _client = new SslClient();
#pragma warning restore IDE0069 // 应释放可释放的字段

        /// <summary>
        /// 封包, 做粘包用
        /// </summary>
        private readonly List<byte> _packetData = new List<byte>();

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

            // ssl 证书加载
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            _client.VerifyMode = SslVerifyMode.Peer | SslVerifyMode.FailIfNoPeerCert;
            // 单向验证或客户端可选
            _client.CaPemCertFileOrPath = "ssl-cert\\ca.crt";
            // 以下都是客户端可选的属性
            _client.PemCertFile = "ssl-cert\\client.cer";
            _client.PemKeyFile = "ssl-cert\\client.key";
            _client.KeyPassword = "123456";

            // 初始化ssl环境, 参数false表示从文件加载证书, 如果为true, 应该把证书读到内存
            // 例如, 证书是数据库里读入进来的文本数据, 此处为true
            if (!_client.Initialize(false))
            {
                AddLog("ssl环境初始化失败, 请检查证书是否存在, 证书密码是否正确");
                return;
            }

            // 事件绑定
            _client.OnPrepareConnect += OnPrepareConnect;
            _client.OnConnect += OnConnect;
            // ssl client 还应该绑定握手事件, 该事件到达说明两端握手成功, OnConnect只能说明连接成功了, 还没握手
            _client.OnHandShake += OnHandShake;
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
        private HandleResult OnHandShake(IClient sender)
        {
            AddLog("OnHandShake()");
            _packetData.Clear();
            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnReceive(IClient sender, byte[] data)
        {
            AddLog($"OnReceive(), data length: {data.Length}");

            _packetData.AddRange(data);

            // 总长度小于包头
            if (_packetData.Count < sizeof(int))
            {
                return HandleResult.Ok;
            }

            HandleResult result;
            const int headerLength = sizeof(int);
            do
            {
                // 取头部字节得到包头
                var packetHeader = _packetData.GetRange(0, headerLength).ToArray();

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
                if (_packetData.Count < fullLength)
                {
                    // 下次数据到达处理
                    result = HandleResult.Ignore;
                    break;
                }

                // 是不是一个完整的包(包长 = 实际数据长度 + 包头长度)
                if (_packetData.Count == fullLength)
                {
                    // 得到完整包并处理
                    var fullData = _packetData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(fullData);
                    // 清空缓存
                    _packetData.Clear();
                    break;
                }

                // 如果来的数据比一个完整的包长
                if (_packetData.Count > fullLength)
                {
                    // 先得到完整包并处理
                    var fullData = _packetData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(fullData);
                    if (result == HandleResult.Error)
                    {
                        break;
                    }
                    // 再移除已经读了的数据
                    _packetData.RemoveRange(0, fullLength);

                    // 剩余的数据下个循环处理
                }

            } while (true);


            return result;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
            _packetData.Clear();
            AddLog($"OnClose(), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnProcessFullPacket(byte[] data)
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
