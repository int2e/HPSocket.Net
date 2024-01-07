using System;
using System.Collections.Generic;
using System.Text;

using HPSocket;
using HPSocket.Http;
using HPSocket.Proxy;

namespace HttpClientTestEcho
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // HttpEasyClient实例
                using (IHttpEasyClient httpClient = new HttpEasyClient())
                {
                    // httpClient.ProxyList = new List<IProxy>
                    // {
                    //     // 支持http隧道代理
                    //     // new HttpProxy
                    //     // {
                    //     //     Host = "127.0.0.1",
                    //     //     Port = 1080,
                    //     //     // 支持帐号和密码, 可选
                    //     //     // UserName = "admin",
                    //     //     // Password = "pass"
                    //     // },
                    //     // 也支持socks5代理
                    //     new Socks5Proxy
                    //     {
                    //         Host = "agent.com",
                    //         Port = 1080,
                    //         // 支持帐号和密码, 可选
                    //         UserName = "test1",
                    //         Password = "123123"
                    //     }
                    // };

                    // 绑定EasyMessageData事件
                    httpClient.OnEasyMessageData += (_, data) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(data));
                        return HttpParseResult.Ok;
                    };
                    
                    // 连接服务器
                    if (!httpClient.Connect("127.0.0.1", 8080))
                    {
                        throw new Exception($"httpClient.Connect() failed, code: {httpClient.ErrorCode}, message: {httpClient.ErrorMessage}");
                    }

                    // // 异步等待代理结束
                    // // var ok = await httpClient.WaitProxyAsync();

                    // // 同步等待代理结束
                    // var ok = httpClient.WaitProxyAsync().Result;
                    // if (!ok)
                    // {
                    //     throw new Exception($"proxy connect failed");
                    // }

                    // 发送GET请求
                    if (!httpClient.SendGet("/test", new List<NameValue>
                        {
                            new NameValue { Name = "Connection", Value = "Keep-Alive" }
                        }))
                    {
                        throw new Exception($"httpClient.SendGet() failed, code: {httpClient.ErrorCode}, message: {httpClient.ErrorMessage}");
                    }

                    // 发送POST请求
                    var body = Encoding.UTF8.GetBytes("{\"a\": \"b\", \"b\": 1}");
                    if (!httpClient.SendPost("/test", new List<NameValue>
                        {
                            new NameValue { Name = "Connection", Value = "Keep-Alive" },
                            new NameValue { Name = "Content-Type", Value = "application/json;charset=utf-8" }
                        }, body, body.Length))
                    {
                        throw new Exception($"httpClient.SendGet() failed, code: {httpClient.ErrorCode}, message: {httpClient.ErrorMessage}");
                    }

                    httpClient.Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
