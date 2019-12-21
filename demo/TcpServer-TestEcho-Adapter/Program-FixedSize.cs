using System;
using HPSocket;
using HPSocket.Tcp;
using TcpServerTestEchoAdapter.DataReceiveAdapter;

namespace TcpServerTestEchoAdapter
{
    /// <summary>
    /// 定长包数据适配器示例
    /// </summary>
    class ProgramFixedSize
    {
        static void Main2(string[] args)
        {
            Console.Title = "FixedSize-Data-Receive-Adapter";

            using (ITcpServer<byte[]> server = new TcpServer<byte[]>
            {
                Address = "0.0.0.0",
                Port = 5555,
                // 指定数据接收适配器
                DataReceiveAdapter = new BinaryDataReceiveAdapter(),
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
                // 这里解析请求体事件的data就是BinaryDataReceiveAdapter.ParseRequestBody()返回的数据
                server.OnParseRequestBody += (sender, id, data) =>
                {
                    Console.WriteLine($"OnParseRequestBody({id}) -> data length: {data.Length}");

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