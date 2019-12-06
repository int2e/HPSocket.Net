using System;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Proxy;

namespace TcpPortForwarding
{
    public partial class Form : System.Windows.Forms.Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ITcpPortForwarding _portForwarding = new HPSocket.Tcp.TcpPortForwarding();
#pragma warning restore IDE0069 // 应释放可释放的字段

        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // tcp端口转发组件是支持通过代理连接到目标服务器的, http和socks5代理可以同时混用, 会随机挑选代理服务器, 支持无限多个
            /*
            _portForwarding.ProxyList = new List<IProxy>
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

            // 事件绑定, 如果你不需要处理数据包和连接黑名单等业务, 只是纯做tcp转发, 事件也无需绑定, 直接[启动按钮]事件的代码设置好属性, 然后start()就ok了
            // 转发组件内部server组件的事件
            _portForwarding.OnServerAccept += OnServerAccept;
            _portForwarding.OnServerReceive += OnServerReceive;
            _portForwarding.OnServerClose += OnServerClose;
            // 转发组件内部agent组件的事件
            _portForwarding.OnAgentConnect += OnAgentConnect;
            _portForwarding.OnAgentReceive += OnAgentReceive;
            _portForwarding.OnAgentClose += OnAgentClose;

            AddLog($"hp-socket version: {_portForwarding.Version}");
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnServerAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            AddLog($"OnServerAccept({connId})");
            return HandleResult.Ok;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnServerReceive(IServer sender, IntPtr connId, byte[] data)
        {
            AddLog($"OnServerReceive({connId}), data length: {data.Length}");
            return HandleResult.Ok;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnServerClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnServerClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnAgentConnect(IAgent sender, IntPtr connId, IProxy proxy)
        {
            AddLog($"OnAgentConnect({connId})");
            return HandleResult.Ok;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnAgentReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            AddLog($"OnAgentReceive({connId}), data length: {data.Length}");
            return HandleResult.Ok;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private HandleResult OnAgentClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            AddLog($"OnAgentClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }

        private void FormAgent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnServiceSwitch.Text == @"停止")
            {
                MessageBox.Show(@"请先停止服务", @"正在通信:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            // 停止并释放客户端
            _portForwarding.Dispose();

            e.Cancel = false;
        }

        private async void BtnConnectSwitch_Click(object sender, EventArgs e)
        {
            try
            {
                // 演示设置 port forwarding 属性

                // 本地端口
                if (!ushort.TryParse(txtLocalPort.Text.Trim(), out var localPort) || localPort == 0)
                {
                    throw new Exception("本地端口有误");
                }

                // 目标端口
                if (!ushort.TryParse(txtTargetPort.Text.Trim(), out var targetPort) || targetPort == 0)
                {
                    throw new Exception("目标端口有误");
                }

                // 最大连接数
                if (!uint.TryParse(txtMaxConnectionCount.Text.Trim(), out var maxConnectionCount) || maxConnectionCount == 0)
                {
                    throw new Exception("最大连接数有误");
                }

                // 连接超时时间
                if (!int.TryParse(txtConnectionTimeout.Text.Trim(), out var connectionTimeout) || connectionTimeout < 100)
                {
                    throw new Exception("连接超时时间有误, 最小也得100ms吧?");
                }

                // 每组件工作线程数
                if (!uint.TryParse(txtWorkThreadCount.Text.Trim(), out var eachWorkThreadCount) || eachWorkThreadCount == 0)
                {
                    throw new Exception("每组件工作线程数有误, 最小也得各给1个吧?");
                }

                _portForwarding.LocalBindAddress = txtLocalAddress.Text.Trim();
                _portForwarding.LocalBindPort = localPort;
                _portForwarding.TargetAddress = txtTargetAddress.Text.Trim();
                _portForwarding.TargetPort = targetPort;
                _portForwarding.ConnectionTimeout = connectionTimeout;
                _portForwarding.EachWorkThreadCount = eachWorkThreadCount;
                _portForwarding.LocalBindAddress = txtLocalAddress.Text.Trim();

                if (btnServiceSwitch.Text == @"启动")
                {
                    if (!_portForwarding.Start())
                    {
                        throw new Exception($"Start() error code: {_portForwarding.ErrorCode}, error message: {_portForwarding.ErrorMessage}");
                    }

                    btnServiceSwitch.Text = @"停止";
                    await  _portForwarding.WaitAsync();
                    btnServiceSwitch.Text = @"启动";
                }
                else
                {
                    // 断开所有连接并停止服务
                    await _portForwarding.StopAsync();
                }
            }
            catch (Exception ex)
            {
                AddLog($"exception: {ex.Message}");
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

        private void NumberTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                {
                    e.Handled = true;
                }
            }
        }
    }
}
