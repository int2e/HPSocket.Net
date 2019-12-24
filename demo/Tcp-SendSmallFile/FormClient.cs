using HPSocket;
using HPSocket.Tcp;
using Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TcpSendSmallFile
{
    public partial class FormClient : Form
    {
        private readonly ITcpPackClient _client = new TcpPackClient();

        public FormClient()
        {
            InitializeComponent();
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            _client.Address = "127.0.0.1";
            _client.Port = 5555;

            _client.PackHeaderFlag = 0x00;
            _client.MaxPackSize = 0x3FFFFF;


            _client.OnConnect += OnConnect;
            _client.OnReceive += OnReceive;
            _client.OnClose += OnClose;

            if (!_client.Connect())
            {
                ShowLog($"error code: {_client.ErrorCode}, error message: {_client.ErrorMessage}");
                Close();
            }
        }

        private HandleResult OnConnect(IClient sender)
        {
            // ShowLog("OnConnect()");
            return HandleResult.Ok;
        }

        private HandleResult OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
            // ShowLog($"OnClose(), socket operation: {socketOperation}, error code: {errorCode}");

            return HandleResult.Ok;
        }

        private HandleResult OnReceive(IClient sender, byte[] data)
        {
            // ShowLog($"OnReceive(), data length: {data.Length}");
            return HandleResult.Ok;
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                // 发送当前exe目录下的1.txt, 文件大小不能超过4096K

                // 文件相对路径
                var path = "./1.txt";

                // 文件绝对路径
                var fullPath = Path.GetFullPath(path);

                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, @"hello world!", Encoding.UTF8);
                }

                // 获取文件信息
                var fileInfo = new FileInfo(fullPath);

                // 附加头部=文件大小
                var head =  BitConverter.GetBytes((int)fileInfo.Length);

                // 附加尾部=文件的详细信息
                var json = JsonConvert.SerializeObject(new MyFileInfo
                {
                    Name = fileInfo.Name,
                    Length = (int)fileInfo.Length,
                    FullName = fileInfo.FullName,
                    CreationTime = fileInfo.CreationTime,
                    LastWriteTime = fileInfo.LastWriteTime,
                });
                var tail = Encoding.UTF8.GetBytes(json);

                // 发送文件, 头尾的附加信息可以更丰富, 最后转成bytes传递给参数就行
                var ok = _client.SendSmallFile(fullPath, head, tail);
                if (!ok)
                {
                    throw new  Exception("文件发送失败");
                }
            }
            catch (Exception ex)
            {
                ShowLog("异常: " + ex.Message);
            }
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Dispose();
        }

        private void ShowLog(string log)
        {
            MessageBox.Show(log);
        }
    }
}
