using System;
using System.Collections.Generic;
using System.Text;
using HPSocket;
using HPSocket.Http;

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
                    // 绑定EasyMessageData事件
                    httpClient.OnEasyMessageData += (sender, data) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(data));
                        return HttpParseResult.Ok;
                    };

                    // 连接服务器
                    if (!httpClient.Connect("127.0.0.1", 8080))
                    {
                        throw new Exception($"httpClient.Connect() failed, code: {httpClient.ErrorCode}, message: {httpClient.ErrorMessage}");
                    }

                    // 发送GET请求
                    if (!httpClient.SendGet("/test", new List<NameValue>
                    {
                        new NameValue { Name = "Connection", Value = "Keep-Alive" }
                    }))
                    {
                        throw new Exception($"httpClient.SendGet() failed, code: {httpClient.ErrorCode}, message: {httpClient.ErrorMessage}");
                    }

                    // 发送POST请求
                    const string body = "{\"a\": \"b\", \"b\": 1}";
                    if (!httpClient.SendPost("/test",new List<NameValue>
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
