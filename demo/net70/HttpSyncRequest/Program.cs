using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

using HPSocket;
using HPSocket.Http;

var headers = new List<NameValue>()
{
    new (){Name = "Accept", Value = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"},
    new (){Name = "Accept-Encoding", Value = "gzip, deflate, br"}, // 支持响应压缩
    new (){Name = "Accept-Language", Value = "zh-CN,zh;q=0.9"},
    new (){Name = "User-Agent", Value = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36"},
};


string? httpsUrl;


// http 的百度访问
{
    IHttpSyncClient http = new HttpSyncClient
    {
        // 请求头
        RequestHeaders = headers,
    };

    // 访问不带s的百度会返回302
    var bytes = http.Get("http://www.baidu.com");

    // 302的时候可以从Location取要跳转的地址
    if (!http.ResponseHeaders.TryGetValue("Location", out httpsUrl))
    {
        Console.WriteLine("未从响应头中找到Location");
        return;
    }

    // 打印所有的响应头
    Console.WriteLine(JsonSerializer.Serialize(http.ResponseHeaders, 
        new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            WriteIndented = true, // 漂亮的json格式
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        })
    );

    // 打印响应结果
    if (bytes != null)
    {
        Console.WriteLine(Encoding.UTF8.GetString(bytes));
    }
    
}

Console.WriteLine("==================================================================================");

// 接着 https 的百度访问
{
    IHttpsSyncClient http = new HttpsSyncClient
    {
        IgnoreCertificateValidation = true, // 忽略证书验证

        // 请求头
        RequestHeaders = headers,
    };

    var bytes = http.Get(httpsUrl);
    if (bytes != null)
    {
        Console.WriteLine(Encoding.UTF8.GetString(bytes));
    }
    
}

