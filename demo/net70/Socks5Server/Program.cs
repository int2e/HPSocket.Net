
using Socks5Server;

try
{
    var server = new Socks5Server.Socks5Server
    {
        Address = "0.0.0.0",
        Port = 1082,
        AuthenticationRequired = true,
        Accounts = new List<Account>
        {
            new()
            {
                UserName = "test1",
                Password = "123123",
            },
        },
    };
    server.Run();
    await server.WaitAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}