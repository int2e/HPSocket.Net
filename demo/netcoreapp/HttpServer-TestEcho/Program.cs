using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HPSocket;
using HPSocket.Http;

namespace HttpServerTestEcho
{
    class Program
    {
        /// <summary>
        /// html 模板 (在资源文件Resources.resx中)
        /// </summary>
        private static readonly string Template = Resources.HtmlTemplate;

        //private static readonly string Template = File.ReadAllText("./template.html", Encoding.UTF8);

        static void Main(string[] args)
        {
            // HttpEasyServer 实例
            using (IHttpEasyServer httpServer = new HttpEasyServer())
            {
                // 服务端口
                httpServer.Port = 8080;

                // 数据到达事件
                httpServer.OnEasyMessageData += HttpServer_OnEasyMessageData; ;

                // 开启服务
                if (!httpServer.Start())
                {
                    Console.WriteLine($@"httpServer.Start() error, code：{httpServer.ErrorCode}, message: {httpServer.ErrorMessage}");
                    return;
                }

                Console.WriteLine($@"bind: {httpServer.Address}:{httpServer.Port}");

                // 等待服务结束
                httpServer.Wait();
            }
        }

        private static HttpParseResult HttpServer_OnEasyMessageData(IHttpEasyServer sender, IntPtr connId, byte[] data)
        {
            var headers = sender.GetAllHeaders(connId);
            var sb = new StringBuilder();
            foreach (var item in headers)
            {
                sb.Append($"{item.Name}: {item.Value}<br />");
            }

            // 替换模板中的{{headers}}变量
            var html = Template.Replace("{{headers}}", sb.ToString());

            // 替换模板中的{{body}}变量
            var body = data?.Length > 0 ? Encoding.UTF8.GetString(data) : "--no body--";
            html = html.Replace("{{body}}", body);

            // 替换模板中的{{now}}变量
            html = html.Replace("{{now}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // 替换模板中的{{path}}变量
            var path = sender.GetUrlField(connId, HttpUrlField.Path);
            html = html.Replace("{{path}}", path);

            // 替换模板中的{{query_string}}变量
            var queryString = sender.GetUrlField(connId, HttpUrlField.QueryString);
            html = html.Replace("{{query_string}}", queryString?.Length > 0 ? queryString : "--no query string--");

            // 响应体
            var responseBody = Encoding.UTF8.GetBytes(html);
            // 响应头
            var responseHeaders = new List<NameValue>
            {
                new NameValue{ Name = "Content-Type", Value = "text/html"},
            };

            // 发送响应数据到客户端
            if (!sender.SendResponse(connId, HttpStatusCode.Ok, responseHeaders, responseBody, responseBody.Length))
            {
                return HttpParseResult.Error;
            }

            // 不是保活连接踢掉
            if (sender.GetHeader(connId, "Connection") != "Keep-Alive")
            {
                sender.Release(connId);
            }

            return HttpParseResult.Ok;
        }

    }
}
