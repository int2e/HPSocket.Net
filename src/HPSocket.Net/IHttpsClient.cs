using HPSocket.Ssl;

namespace HPSocket
{
    /// <summary>
    /// https agent
    /// </summary>
    public interface IHttpsClient : ISsl, IHttpClient
    {
    }
}
