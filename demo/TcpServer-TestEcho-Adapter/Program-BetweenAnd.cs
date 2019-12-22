using System;
using HPSocket;
using HPSocket.Tcp;
using TcpServerTestEchoAdapter.DataReceiveAdapter;

namespace TcpServerTestEchoAdapter
{
    /// <summary>
    /// 区间数据适配器示例
    /// </summary>
    class ProgramBetweenAnd
    {
        static void Main3(string[] args)
        {
            Console.Title = "BetweenAnd-Data-Receive-Adapter";

            using (ITcpServer<string> server = new TcpServer<string>
            {
                Address = "0.0.0.0",
                Port = 5555,
                // 指定数据接收适配器
                DataReceiveAdapter = new HeadTailDataReceiveAdapter(),
            })
            {

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
                // 这里解析请求体事件的text就是HeadTailDataReceiveAdapter.ParseRequestBody()返回的数据
                server.OnParseRequestBody += (sender, id, text) =>
                {
                    Console.WriteLine($"OnParseRequestBody({id}) -> text : {text}");

                    return HandleResult.Ok;
                };

                // 连接关闭
                server.OnClose += (sender, id, operation, code) =>
                {
                    Console.WriteLine($"OnClose({id}) -> operation: {operation}, code: {code}");

                    return HandleResult.Ok;
                };

                // 启动服务
                server.Start();

                // 等待服务结束
                server.Wait();

            }
        }
    }
}