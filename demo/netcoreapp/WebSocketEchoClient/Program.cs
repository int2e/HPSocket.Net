using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HPSocket;
using HPSocket.Proxy;
using HPSocket.Ssl;
using HPSocket.WebSocket;

namespace WebSocketEchoAgent
{
    class Program
    {
        private static readonly object ConsoleLock = new object();
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("是否使用wss agent? (y.是, 其他.不是): ");
                var wss = Console.ReadLine()?.Trim().ToLower() == "y";

                using (IWebSocketAgent agent = new WebSocketAgent($"{(wss ? "wss" : "ws")}://127.0.0.1:8800")
                {
                    // 本地绑定地址
                    BindAddress = "0.0.0.0",

                    // 支持连接超时时间设置, 单位毫秒
                    ConnectionTimeout = 3000,

                    // 忽略压缩扩展, 不忽略支持数据包压缩解压缩
                    IgnoreCompressionExtensions = false,

                    // 最大封包大小
                    MaxPacketSize = 0x4000,

                    // 浏览器User-Agent, 默认chrome78.0.3904.97的User-Agent
                    UserAgent = "hp-socket5.7 websocket agent",

                    // 升级websocket协议的时候可以附带cookie
                    Cookie = "key=hello;value=world",

                    // 子协议
                    SubProtocols = null,
                })
                {
                    if (wss)
                    {
                        // wss请开启此设置, 设置ssl配置, 会自动初始化ssl环境
                        agent.SslConfiguration = new SslConfiguration
                        {
                            // 不从内存加载证书
                            FromMemory = false,

                            // 不验证证书
                            VerifyMode = SslVerifyMode.None,
                        };
                    }

                    // agent 的 http 对象, 本质上是 IHttpAgent
                    var httpAgent = (IHttpAgent)agent.Http;
                    Console.WriteLine($"IsUseCookie: {httpAgent.IsUseCookie}");

                    // 当然也就可以设置代理, http和socks5代理可以同时混用, 会随机挑选代理服务器, 支持无限多个
                    /*
                    httpAgent.ProxyList = new List<IProxy>
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

                    // 打开连接
                    agent.OnOpen += (sender, id) =>
                    {
                        Console.WriteLine($"OnOpen({id}), hp-socket version: { sender.Version}");
                        return HandleResult.Ok;
                    };

                    // 消息到达
                    agent.OnMessage += (sender, id, final, opCode, mask, data) =>
                    {
                        Console.WriteLine($"OnMessage({id}), final: {final}, op code: {opCode}, data length: {data.Length}");

                        lock (ConsoleLock)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"服务器说: {Encoding.UTF8.GetString(data)}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        SendByInput(sender, id);
                        return HandleResult.Ok;
                    };

                    // 连接关闭
                    agent.OnClose += (sender, id, socketOperation, errorCode) =>
                    {
                        Console.WriteLine($"OnClose({id}), socket operation: {socketOperation}, error code: {errorCode}");
                        // 延时1秒
                        Thread.Sleep(1000);
                        // 重连
                        sender.Connect();
                        return HandleResult.Ok;
                    };

                    // 要先启动服务
                    agent.Start();

                    // 连接服务器, 该组件不支持连接不同ws服务器
                    agent.Connect();

                    // 等待服务停止
                    await agent.WaitAsync();
                }

                Console.WriteLine("Bye!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception ：{ex.Message}\r\n\t{ex.StackTrace}");
            }
        }

        private static void SendByInput(IWebSocketAgent agent, IntPtr connId)
        {
            do
            {
                Console.WriteLine("您想对服务器说些什么?");
                Console.Write("\\>");
                var input = Console.ReadLine()?.Trim() ?? "";
                if (input.Length > 0)
                {
                    if(!agent.Text(connId, input))
                    {
                        agent.Close(connId);
                    }

                    lock (ConsoleLock)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"您说: {input}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    break;
                }
            } while (true);
        }
    }
}
