using System;
using HPSocket;
using HPSocket.Tcp;
using Models;
using TcpServerTestEchoAdapter.DataReceiveAdapter;
using Timer = System.Timers.Timer;

namespace TcpServerTestEchoAdapter
{
    /// <summary>
    /// 固定包头长度数据适配器示例
    /// </summary>
    class Program
    {
        /// <summary>
        /// 线程池
        /// </summary>
        private static readonly HPSocket.Thread.ThreadPool ThreadPool = new HPSocket.Thread.ThreadPool();

        static void Main(string[] args)
        {
            Console.Title = "TcpServer-TestEcho-Adapter";


            // TcpServer-TestEcho-Adapter 适配 TcpClient-TestEcho 和 TcpAgent-TestEcho, 可以使用这两个客户端连接到当前服务端

            using (ITcpServer<Packet> server = new TcpServer<Packet>
            {
                Address = "0.0.0.0",
                Port = 5555,
                // 指定数据接收适配器
                DataReceiveAdapter = new PacketDataReceiveAdapter(),
            })
            {
                // 定时器
                var timer = new Timer(5000)
                {
                    AutoReset = true,
                };

                // 定时输出线程池任务数
                timer.Elapsed += (sender, eventArgs) =>
                {
                    if (ThreadPool.HasStarted)
                    {
                        Console.WriteLine($"线程池当前在执行的任务数: {ThreadPool.TaskCount}, 任务队列数: {ThreadPool.QueueSize}");
                    }
                };
                timer.Start();

                // 准备监听事件
                server.OnPrepareListen += (sender, listen) =>
                {
                    Console.WriteLine($"OnPrepareListen({listen})");
                    return HandleResult.Ok;
                };

                // 连接进入
                server.OnAccept += (sender, id, client) =>
                {
                    Console.WriteLine($"OnAccept({id}) -> socket handle: {client}");

                    return HandleResult.Ok;
                };

                // 不需要在绑定OnReceive事件
                // 这里解析请求体事件的packet就是PacketDataReceiveAdapter.ParseRequestBody()返回的数据
                server.OnParseRequestBody += (sender, id, packet) =>
                {
                    Console.WriteLine($"OnParseRequestBody({id}) -> type: {packet.Type}, data: {packet.Data}");

                    // 处理业务逻辑
                    return ProcessPacket(sender, id, packet);
                };

                // 连接关闭
                server.OnClose += (sender, id, operation, code) =>
                {
                    Console.WriteLine($"OnClose({id}) -> operation: {operation}, code: {code}");

                    return HandleResult.Ok;
                };

                // 启动线程池
                ThreadPool.Start(2, HPSocket.Thread.RejectedPolicy.WaitFor);

                // 启动服务
                server.Start();

                // 等待线程池任务全部结束
                ThreadPool.Wait();

                // 等待服务结束
                server.Wait();

                timer.Stop();
                timer.Close();
            }
        }

        private static HandleResult ProcessPacket(IServer sender, IntPtr id, Packet packet)
        {
            var result = HandleResult.Ignore;
            switch (packet.Type)
            {
                case PacketType.Echo: // 假如回显是一个非耗时操作, 在这处理
                {
                    // 回显数据
                    var data = PacketHelper.ToSendBytes(packet);
                    result = sender.Send(id, data, data.Length) ? HandleResult.Ok : HandleResult.Error;
                    break;
                }
                case PacketType.Time: // 假如获取服务器时间是耗时操作, 将该操作放入队列
                {
                    // 向线程池提交任务
                    if (!ThreadPool.Submit(ThreadProc, new { sender, id, packet })) // 线程池传参, 这里使用的是匿名对象
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
        /// 线程池回调函数
        /// </summary>
        /// <param name="obj"></param>
        static void ThreadProc(object obj)
        {
            // 解构动态对象
            dynamic dyObj = obj;
            IServer sender = dyObj.sender;
            IntPtr id = dyObj.id;
            Packet packet = dyObj.packet;

            // 如果连接已经断开了(可能被踢了)
            // 它的任务就不做了(根据自己业务需求来, 也许你的任务就是要完成每个连接的所有任务, 每个包都要处理, 不管连接断开与否, 就不要写这个判断, 但是你回发包的时候要判断是否有效连接)
            if (!sender.IsConnected(id))
            {
                return;
            }

            if (packet.Type != PacketType.Time)
            {
                return;
            }

            // 模拟耗时操作
            System.Threading.Thread.Sleep(6000);

            // 组织回包
            packet = new Packet
            {
                Type = PacketType.Time,
                Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            // 发送数据
            var data = PacketHelper.ToSendBytes(packet);
            if (!sender.Send(id, data, data.Length))
            {
                // 发送失败断开连接
                sender.Disconnect(id);
            }
        }
    }
}
