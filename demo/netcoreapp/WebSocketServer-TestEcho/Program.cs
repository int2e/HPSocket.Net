using System;
using System.Threading.Tasks;
using HPSocket;
using HPSocket.Ssl;
using HPSocket.WebSocket;

namespace WebSocketServerTestEcho
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("是否wss server? (y.是, 其他.不是): ");
                var wss = Console.ReadLine()?.Trim().ToLower() == "y";

                using (IWebSocketServer server = new WebSocketServer($"{(wss ? "wss" : "ws")}://127.0.0.1:8800")
                {
                    // 忽略压缩扩展, 不忽略支持数据包压缩解压缩, 主流浏览器都支持
                    IgnoreCompressionExtensions = false,

                    // 自动发送ping消息的时间间隔
                    // 毫秒，0不自动发送，默认不发送（多数分机房的防火墙都在1分钟甚至更短时间检测空连接，超时无交互则被踢，如果间隔过长，可能被机房防火墙误杀）
                    // 目前浏览器都不支持在客户端发送ping消息，所以一般在服务器发送ping，在客户端响应接收到ping消息之后再对服务器发送pong，或客户端主动pong，服务器响应pong再发送ping给客户端
                    PingInterval = 10000,

                    // 最大封包大小
                    MaxPacketSize = 0x4000,

                    // 子协议, 微信接口等会发送自定义的子协议,询问服务器是不是支持, 如果需要配置请再此配置
                    SubProtocols = null,
                })
                {
                    if (wss)
                    {
                        // wss请开启此设置, 设置ssl配置, 会自动初始化ssl环境
                        server.SslConfiguration = new SslConfiguration
                        {
                            // 不从内存加载证书
                            FromMemory = false,

                            // ssl证书配置, 支持单向验证
                            VerifyMode = SslVerifyMode.Peer,
                            CaPemCertFileOrPath = "ssl-cert\\ca.crt",
                            PemCertFile = "ssl-cert\\server.cer",
                            PemKeyFile = "ssl-cert\\server.key",
                            KeyPassword = "123456",
                        };
                    }
                    // 注册ws服务器, 未对path注册服务则无法访问
                    // 要注册的服务必须继承自HPSocket.WebSocket.IHub接口
                    // 注册欢迎服务, 客户端通过ws[s]://127.0.0.1:8800连接
                    server.AddHub<WelcomeHub>("/");

                    // 注册回显服务, 客户端通过ws[s]://127.0.0.1:8800/echo连接
                    server.AddHub<EchoHub>("/echo");

                    // 启动服务
                    server.Start();

                    await server.WaitAsync();

                }

                Console.WriteLine("Bye!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception ：{ex.Message}\r\n\t{ex.StackTrace}");
            }
        }
    }
}
