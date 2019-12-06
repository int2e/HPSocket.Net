using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HPSocket;
using HPSocket.Tcp;
using HPSocket.Thread;
using Models;
using Newtonsoft.Json;
using ThreadPool = HPSocket.Thread.ThreadPool;
using Timer = System.Timers.Timer;

namespace TcpEchoServer
{
    public partial class FormServer : Form
    {
        delegate void AddLogHandler(string log);

#pragma warning disable IDE0069 // 应释放可释放的字段
        private readonly ITcpServer _server = new TcpServer();

        /// <summary>
        /// 线程池
        /// </summary>
        private readonly ThreadPool _threadPool = new ThreadPool();

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer = new Timer(5000)
        {
            AutoReset = true,
        };

#pragma warning restore IDE0069 // 应释放可释放的字段

        /// <summary>
        /// 线程池回调函数
        /// </summary>
        private TaskProcEx _taskTaskProc;

        /// <summary>
        /// 最大封包长度
        /// </summary>
        private const int MaxPacketSize = 4096;

        public FormServer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 演示设置serer属性

            // 缓冲区大小应根据实际业务设置, 并发数和包大小都是考虑条件
            // 都是小包的情况下4K合适, 刚好是一个内存分页(在非托管内存, 相关知识查百度)
            // 大包比较多的情况下, 应根据并发数设置比较大但是又不会爆内存的值
            _server.SocketBufferSize = 4096; // 4K

            // serer绑定地址和端口
            _server.Address = "0.0.0.0";
            _server.Port = 5555;

            // 演示绑定事件
            _server.OnPrepareListen += OnPrepareListen;
            _server.OnAccept += OnAccept;
            _server.OnReceive += OnReceive;
            _server.OnClose += OnClose;
            _server.OnShutdown += OnShutdown;


            // 线程池相关设置
            // 线程池回调函数
            _taskTaskProc = TaskTaskProc;

            // 定时输出线程池任务数
            _timer.Elapsed += (_, args) =>
            {
                if (_server.HasStarted && _threadPool.HasStarted)
                {
                    AddLog($"线程池当前在执行的任务数: {_threadPool.TaskCount}, 任务队列数: {_threadPool.QueueSize}");
                }
            };
            _timer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_server.HasStarted)
            {
                MessageBox.Show(@"请先停止服务", @"服务正在运行:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            // 停止并释放线程池, 会等待所有任务结束
            _threadPool.Dispose();

            // 停止并释放定时器
            _timer.Stop();
            _timer.Close();

            // 停止释放服务器
            _server.Dispose();
            e.Cancel = false;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnPrepareListen(IServer sender, IntPtr listen)
        {
            AddLog($"OnPrepareListen({sender.Address}:{sender.Port}), listen: {listen}, hp-socket version: {sender.Version}");

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            // 获取客户端地址
            if (!sender.GetRemoteAddress(connId, out var ip, out var port))
            {
                return HandleResult.Error;
            }

            AddLog($"OnAccept({connId}), ip: {ip}, port: {port}");

            // 设置附加数据, 用来做粘包处理
            sender.SetExtra(connId, new ClientInfo
            {
                ConnId = connId,
                PacketData = new List<byte>(),
            });

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnReceive(IServer sender, IntPtr connId, byte[] data)
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
        private HandleResult OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
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

        // ReSharper disable once InconsistentNaming
        private HandleResult OnShutdown(IServer sender)
        {
            AddLog($"OnShutdown({sender.Address}:{sender.Port})");

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnProcessFullPacket(IServer sender, ClientInfo client, byte[] data)
        {
            // 这里来的都是完整的包, 但是这里不做耗时操作, 仅把数据放入队列
            var packet = JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(data));
            var result = HandleResult.Ok;
            switch (packet.Type)
            {
                case PacketType.Echo: // 假如回显是一个非耗时操作, 在这处理
                {
                    // 组织packet为一个json
                    var json = JsonConvert.SerializeObject(new Packet
                    {
                        Type = packet.Type,
                        Data = packet.Data,
                    });

                    // json转字节数组
                    var bytes = Encoding.UTF8.GetBytes(json);

                    // 先发包头
                    if (!SendPacketHeader(sender, client.ConnId, bytes.Length))
                    {
                        result = HandleResult.Error;
                        break;
                    }

                    // 再发实际数据
                    if (!sender.Send(client.ConnId, bytes, bytes.Length))
                    {
                        result = HandleResult.Error;
                    }

                    // 至此完成回显
                    break;
                }
                case PacketType.Time: // 假如获取服务器时间是耗时操作, 将该操作放入队列
                {
                    // 向线程池提交任务
                    if (!_threadPool.Submit(_taskTaskProc, new TaskInfo
                    {
                        Client = client,
                        Packet = packet,
                    }))
                    {
                        result = HandleResult.Error;
                    }

                    break;
                }
                default:
                    result = HandleResult.Error;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 线程池任务回调函数
        /// </summary>
        /// <param name="obj">任务参数</param>
        private void TaskTaskProc(object obj)
        {
            if (!(obj is TaskInfo taskInfo))
            {
                return;
            }

            // 如果连接已经断开了(可能被踢了)
            // 它的任务就不做了(根据自己业务需求来, 也许你的任务就是要完成每个连接的所有任务, 每个包都要处理, 不管连接断开与否, 就不要写这个判断, 但是你回发包的时候要判断是否有效连接)
            if (!_server.IsConnected(taskInfo.Client.ConnId))
            {
                return;
            }

            // 在这里处理耗时任务逻辑

            switch (taskInfo.Packet.Type)
            {
                case PacketType.Time:
                {
                    // 模拟耗时操作
                    Thread.Sleep(6000);

                    // 组织packet为一个json
                    var json = JsonConvert.SerializeObject(new Packet
                    {
                        Type = PacketType.Time,
                        Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });

                    // json转字节数组
                    var bytes = Encoding.UTF8.GetBytes(json);

                    // 先发包头
                    if (!SendPacketHeader(_server, taskInfo.Client.ConnId, bytes.Length))
                    {
                        // 发送失败断开连接
                        _server.Disconnect(taskInfo.Client.ConnId);
                        break;
                    }

                    // 再发实际数据
                    if (!_server.Send(taskInfo.Client.ConnId, bytes, bytes.Length))
                    {
                        _server.Disconnect(taskInfo.Client.ConnId);
                    }

                    // 至此完成当前任务
                    break;
                }
            }
        }

        /// <summary>
        /// 发送包头
        /// <para>固定包头网络字节序</para>
        /// </summary>
        /// <param name="sender">服务器组件</param>
        /// <param name="connId">连接id</param>
        /// <param name="dataLength">实际数据长度</param>
        /// <returns></returns>
        private bool SendPacketHeader(IServer sender, IntPtr connId, int dataLength)
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

        private async void BtnSwitchService_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSwitchService.Text == @"启动")
                {
                    // 2个线程处理耗时操作, 作为相对耗时的任务, 可根据业务需求多开线程处理
                    if (!_threadPool.Start(2, RejectedPolicy.WaitFor))
                    {
                        btnSwitchService.Enabled = false;
                        throw new Exception($"线程池启动失败, 错误码: {_threadPool.ErrorCode}");
                    }

                    // 启动服务
                    if (!_server.Start())
                    {
                        throw new Exception($"error code: {_server.ErrorCode}, error message: {_server.ErrorMessage}");
                    }

                    btnSwitchService.Text = @"停止";

                    // 等待线程池停止
                    await _threadPool.WaitAsync();

                    // 等待服务停止
                    await _server.WaitAsync();

                    // 停止以后还原按钮标题
                    btnSwitchService.Text = @"启动";

                    btnSwitchService.Enabled = true;
                }
                else
                {
                    btnSwitchService.Enabled = false;
                    // 停止并等待线程池任务全部完成
                    await _threadPool.StopAsync();

                    // 等待服务停止
                    await _server.StopAsync();
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

    }
}
