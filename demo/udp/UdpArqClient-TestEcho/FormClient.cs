using System;
using System.Text;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Udp;
using Models;
using Newtonsoft.Json;

namespace UdpArqClientTestEcho
{
    public partial class FormClient : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly IUdpArqClient _client = new UdpArqClient();
#pragma warning restore IDE0069 // 应释放可释放的字段

        public FormClient()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 演示设置client属性

            // 报文长度，建议在局域网环境下不超过 1472 字节，在广域网环境下不超过 548 字节
            _client.MaxDatagramSize = 1432; // 1432是默认值
            // 异步连接
            _client.Async = true;

            // 设置监测包尝试次数，0 则不发送监测跳包，如果超过最大尝试次数则认为已断线
            _client.DetectAttempts = 3; // 3是默认值

            // 设置心跳检查间隔
            _client.DetectInterval = 60000; // 60000是默认值


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
        private HandleResult OnReceive(IClient sender, byte[] data)
        {
            AddLog($"OnReceive(), data length: {data.Length}");
            return OnProcessFullPacket(data);
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
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
