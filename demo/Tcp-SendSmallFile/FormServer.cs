using HPSocket;
using HPSocket.Tcp;
using System;
using System.Text;
using System.Windows.Forms;

namespace TcpSendSmallFile
{
    public partial class FormServer : Form
    {
        delegate void AddLogHandler(string log);

        private readonly ITcpPackServer _server = new TcpPackServer();

        public FormServer()
        {
            InitializeComponent();
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            _server.Address = "0.0.0.0";
            _server.Port = 5555;

            _server.PackHeaderFlag = 0x00;
            _server.MaxPackSize = 0x3FFFFF;

            _server.OnPrepareListen += OnPrepareListen;
            _server.OnAccept += OnAccept;
            _server.OnReceive += OnReceive;
            _server.OnClose += OnClose;

            if (!_server.Start())
            {
                AddLog($"error code: {_server.ErrorCode}, error message: {_server.ErrorMessage}");
            }
        }

        private HandleResult OnPrepareListen(IServer sender, IntPtr listen)
        {
            AddLog($"OnPrepareListen({sender.Address}:{sender.Port}), listen: {listen}, hp-socket version: {sender.Version}");

            return HandleResult.Ok;
        }

        private HandleResult OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            // 获取客户端地址
            if (!sender.GetRemoteAddress(connId, out var ip, out var port))
            {
                return HandleResult.Error;
            }

            AddLog($"OnAccept({connId}), ip: {ip}, port: {port}");

            return HandleResult.Ok;
        }

        private HandleResult OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            // length = 发送时的head
            var length = BitConverter.ToInt32(data, 0);

            // 附加 head 长度
            var headLength = sizeof(int);

            // 附加 tail 的位置 = headLength + length, 长度 = data.Length - headLength - length
            var json = Encoding.UTF8.GetString(data, headLength + length, data.Length - headLength - length);

            // 反序列化文件信息
            // var fileInfo = JsonConvert.DeserializeObject<MyFileInfo>(json);

            // 文件内容 = 从 headLength 开始, 到 length 结束

            // 拷贝到其他bytes
            // var bytes = new byte[length];
            // Buffer.BlockCopy(data, sizeof(int), bytes, 0, length);

            // 打印文件内容到log
            var content = Encoding.UTF8.GetString(data, headLength, length);

            AddLog($"OnReceive({connId}), data length: {data.Length}, file length: {length}, file content: {content}, file info: {json}");

            return HandleResult.Ok;
        }

        private HandleResult OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");

            return HandleResult.Ok;
        }

        private void btnOpenClient_Click(object sender, EventArgs e)
        {
            var frm = new FormClient();
            frm.Show(this);
        }

        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server?.Dispose();
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
