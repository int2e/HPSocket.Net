using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HPSocket.WebSocket;

namespace HPSocket.Http
{
    public class HttpEasyClient : HttpClient, IHttpEasyClient
    {
        #region 私有成员
        private List<byte> _easyData = new List<byte>();
        private readonly WebSocketSession _easyWsMessageData = new WebSocketSession();

        #endregion

        /// <inheritdoc />
        public bool AutoDecompression { get; set; } = true;


        /// <inheritdoc />
        public event HttpClientEasyDataEventHandler OnEasyChunkData;

        /// <inheritdoc />
        public event HttpClientEasyDataEventHandler OnEasyMessageData;

        /// <inheritdoc />
        public event HttpClientWebSocketEasyDataEventHandler OnEasyWebSocketMessageData;

#pragma warning disable 67

        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyChunkData事件", true)]
        new event ChunkHeaderEventHandler OnChunkHeader;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyChunkData事件", true)]
        new event ChunkCompleteEventHandler OnChunkComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event HeadersCompleteEventHandler OnHeadersComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event BodyEventHandler OnBody;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event MessageCompleteEventHandler OnMessageComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageHeaderEventHandler OnWsMessageHeader;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageBodyEventHandler OnWsMessageBody;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageCompleteEventHandler OnWsMessageComplete;
#pragma warning restore 67

        public HttpEasyClient()
            : base(Sdk.Http.Create_HP_HttpClientListener,
                Sdk.Http.Create_HP_HttpClient,
                Sdk.Http.Destroy_HP_HttpClient,
                Sdk.Http.Destroy_HP_HttpClientListener)
        {
        }

        protected HttpEasyClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        ~HttpEasyClient() => _easyData.Clear();


        #region SDK回调委托,防止GC

        private Sdk.OnClose _onClose;

        private Sdk.OnHeadersComplete _onHeadersComplete;
        private Sdk.OnBody _onBody;
        private Sdk.OnChunkHeader _onChunkHeader;
        private Sdk.OnChunkComplete _onChunkComplete;
        private Sdk.OnMessageComplete _onMessageComplete;

        private Sdk.OnWsMessageHeader _onWsMessageHeader;
        private Sdk.OnWsMessageBody _onWsMessageBody;
        private Sdk.OnWsMessageComplete _onWsMessageComplete;

        #endregion

        protected override void SetCallback()
        {
            base.SetCallback();


            _onClose = SdkOnClose;
            Sdk.Http.HP_Set_FN_HttpClient_OnClose(ListenerPtr, _onClose);

            _onHeadersComplete = SdkOnHeadersComplete;
            _onBody = SdkOnBody;
            _onChunkHeader = SdkOnChunkHeader;
            _onChunkComplete = SdkOnChunkComplete;
            _onMessageComplete = SdkOnMessageComplete;
            Sdk.Http.HP_Set_FN_HttpClient_OnHeadersComplete(ListenerPtr, _onHeadersComplete);
            Sdk.Http.HP_Set_FN_HttpClient_OnBody(ListenerPtr, _onBody);
            Sdk.Http.HP_Set_FN_HttpClient_OnChunkHeader(ListenerPtr, _onChunkHeader);
            Sdk.Http.HP_Set_FN_HttpClient_OnChunkComplete(ListenerPtr, _onChunkComplete);
            Sdk.Http.HP_Set_FN_HttpClient_OnMessageComplete(ListenerPtr, _onMessageComplete);

            _onWsMessageHeader = SdkOnWsMessageHeader;
            _onWsMessageBody = SdkOnWsMessageBody;
            _onWsMessageComplete = SdkOnWsMessageComplete;
            Sdk.Http.HP_Set_FN_HttpClient_OnWSMessageHeader(ListenerPtr, _onWsMessageHeader);
            Sdk.Http.HP_Set_FN_HttpClient_OnWSMessageBody(ListenerPtr, _onWsMessageBody);
            Sdk.Http.HP_Set_FN_HttpClient_OnWSMessageComplete(ListenerPtr, _onWsMessageComplete);

            GC.KeepAlive(_onClose);

            GC.KeepAlive(_onHeadersComplete);
            GC.KeepAlive(_onBody);
            GC.KeepAlive(_onChunkHeader);
            GC.KeepAlive(_onChunkComplete);
            GC.KeepAlive(_onMessageComplete);

            GC.KeepAlive(_onWsMessageHeader);
            GC.KeepAlive(_onWsMessageBody);
            GC.KeepAlive(_onWsMessageComplete);
        }

        #region 重写父类websocket相关方法, 父类相关事件不会继续触发 
        protected new HandleResult SdkOnWsMessageHeader(IntPtr sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, ulong bodyLength)
        {
            if (OnEasyWebSocketMessageData == null) return HandleResult.Ignore;

            var extra = _easyWsMessageData;

            extra.Final = final;
            extra.Rsv = rsv;
            extra.OpCode = opCode;
            extra.Mask = mask;
            extra.Data = null;
            extra.Data = new List<byte>((int)bodyLength);

            return HandleResult.Ok;
        }

        protected new HandleResult SdkOnWsMessageBody(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnEasyWebSocketMessageData == null) return HandleResult.Ignore;

            var extra = _easyWsMessageData;
            if (extra?.Data == null)
            {
                return HandleResult.Error;
            }

            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }

            extra.Data.AddRange(bytes);

            return HandleResult.Ok;
        }

        protected new HandleResult SdkOnWsMessageComplete(IntPtr sender, IntPtr connId)
        {
            if (OnEasyWebSocketMessageData == null) return HandleResult.Ignore;

            var extra = _easyWsMessageData;
            if (extra == null)
            {
                return HandleResult.Error;
            }

            var result = OnEasyWebSocketMessageData?.Invoke(this, extra.Final, extra.Rsv, extra.OpCode, extra.Mask, extra.Data.ToArray());
            return result ?? HandleResult.Ignore;
        }

        #endregion

        #region 重写父类chunkdata相关方法, 父类相关事件不会继续触发 

        protected new HttpParseResult SdkOnChunkHeader(IntPtr sender, IntPtr connId, int length)
        {
            if (OnEasyChunkData == null) return HttpParseResult.Ok;

            _easyData = null;
            _easyData = new List<byte>(length);

            return HttpParseResult.Ok;
        }

        protected new HttpParseResult SdkOnChunkComplete(IntPtr sender, IntPtr connId)
        {
            if (OnEasyChunkData == null) return HttpParseResult.Ok;

            var result = OnEasyChunkData?.Invoke(this, _easyData.ToArray());
            return (HttpParseResult)result;
        }

        #endregion

        #region  重写父类message相关方法, 父类相关事件不会继续触发 

        protected new HttpParseResultEx SdkOnHeadersComplete(IntPtr sender, IntPtr connId)
        {
            if (OnEasyMessageData == null) return HttpParseResultEx.Ok;

            var header = GetHeader("Content-Length") ?? "0";
            if (!int.TryParse(header, out var contentLength))
            {
                // return HttpParseResultEx.Error;
            }

            _easyData = null;
            _easyData = new List<byte>(contentLength);


            return HttpParseResultEx.Ok;
        }

        protected new HttpParseResult SdkOnBody(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnEasyMessageData == null) return HttpParseResult.Ok;

            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }

            _easyData.AddRange(bytes);

            return HttpParseResult.Ok;
        }

        protected new HttpParseResult SdkOnMessageComplete(IntPtr sender, IntPtr connId)
        {
            if (OnEasyMessageData == null) return HttpParseResult.Ok;


            var data = _easyData.ToArray();

            if (AutoDecompression && data.Length > 0)
            {
                data = data.HttpMessageDataDecompress(GetHeader("Content-Encoding"));
            }

            var result = OnEasyMessageData?.Invoke(this, data);
            return result ?? HttpParseResult.Ok;
        }

        #endregion

    }
}
