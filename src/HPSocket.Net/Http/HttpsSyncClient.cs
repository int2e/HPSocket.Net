namespace HPSocket.Http
{
    public class HttpsSyncClient : HttpsClient, IHttpsSyncClient
    {
        public HttpsSyncClient()
            : base(Sdk.Http.Create_HP_HttpClientListener,
                Sdk.Http.Create_HP_HttpsSyncClient,
                Sdk.Http.Destroy_HP_HttpsSyncClient,
                Sdk.Http.Destroy_HP_HttpClientListener)
        {
        }

        protected HttpsSyncClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }
    }
}
