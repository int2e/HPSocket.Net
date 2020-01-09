using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HPSocket.Tcp;
using HPSocket.WebSocket;

namespace HPSocket.Http
{
    public class HttpServer : TcpServer, IHttpServer
    {
        public HttpServer()
            : base(Sdk.Http.Create_HP_HttpServerListener,
                Sdk.Http.Create_HP_HttpServer,
                Sdk.Http.Destroy_HP_HttpServer,
                Sdk.Http.Destroy_HP_HttpServerListener)
        {
        }

        protected HttpServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public bool HttpAutoStart
        {
            get => Sdk.Http.HP_HttpServer_IsHttpAutoStart(SenderPtr);
            set => Sdk.Http.HP_HttpServer_SetHttpAutoStart(SenderPtr, value);
        }

        /// <inheritdoc />
        public HttpVersion LocalVersion
        {
            get => Sdk.Http.HP_HttpServer_GetLocalVersion(SenderPtr);
            set => Sdk.Http.HP_HttpServer_SetLocalVersion(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint ReleaseDelay
        {
            get => Sdk.Http.HP_HttpServer_GetReleaseDelay(SenderPtr);
            set => Sdk.Http.HP_HttpServer_SetReleaseDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public event MessageBeginEventHandler OnMessageBegin;

        /// <inheritdoc />
        public event HeaderEventHandler OnHeader;

        /// <inheritdoc />
        public event ChunkHeaderEventHandler OnChunkHeader;

        /// <inheritdoc />
        public event ChunkCompleteEventHandler OnChunkComplete;

        /// <inheritdoc />
        public event UpgradeEventHandler OnUpgrade;

        /// <inheritdoc />
        public event RequestLineEventHandler OnRequestLine;

        /// <inheritdoc />
        public event HeadersCompleteEventHandler OnHeadersComplete;

        /// <inheritdoc />
        public event BodyEventHandler OnBody;

        /// <inheritdoc />
        public event MessageCompleteEventHandler OnMessageComplete;

        /// <inheritdoc />
        public event ParseErrorEventHandler OnParseError;

        /// <inheritdoc />
        public event WsMessageHeaderEventHandler OnWsMessageHeader;

        /// <inheritdoc />
        public event WsMessageBodyEventHandler OnWsMessageBody;

        /// <inheritdoc />
        public event WsMessageCompleteEventHandler OnWsMessageComplete;

        /// <inheritdoc />
        public bool SendResponse(IntPtr connId, HttpStatusCode statusCode, List<NameValue> headers, byte[] body, int length)
        {
            return SendResponse(connId, statusCode, statusCode.ToNameString(), headers, body, length);
        }

        /// <inheritdoc />
        public bool SendResponse(IntPtr connId, HttpStatusCode statusCode, string desc, List<NameValue> headers, byte[] body, int length)
        {
            var gch = GCHandle.Alloc(body, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpServer_SendResponse(SenderPtr, connId, statusCode.ToUInt16(), desc, headers.ToArray(), headers.Count, gch.AddrOfPinnedObject(), length);
            gch.Free();
            return ok;
        }

        /// <summary>
        /// http server 不实现当前方法, 请调用带HttpStatusCode版本的SendSmallFile方法
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        [Obsolete("http/https server 不实现当前方法, 请调用带HttpStatusCode版本的SendSmallFile方法", true)]
        public new bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail)
        {
            return false;
        }

        /// <summary>
        /// http server 不实现当前方法, 请调用带HttpStatusCode版本的SendSmallFile方法
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        [Obsolete("http/https server 不实现当前方法, 请调用带HttpStatusCode版本的SendSmallFile方法", true)]
        public new bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail)
        {
            return false;
        }
        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, HttpStatusCode statusCode, List<NameValue> headers, string filePath) => Sdk.Http.HP_HttpServer_SendLocalFile(SenderPtr, connId, filePath, statusCode.ToUInt16(), statusCode.ToNameString(), headers.ToArray(), headers.Count);

        /// <inheritdoc />
        public bool SendChunkData(IntPtr connId, byte[] data, int length, string extensions = null)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpServer_SendChunkData(SenderPtr, connId, gch.AddrOfPinnedObject(), length, extensions);
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendWsMessage(IntPtr connId, MessageState state, byte[] data, int length) => SendWsMessage(connId, state.Final, state.Rsv, state.OpCode, data, length);

        /// <inheritdoc />
        public bool SendWsMessage(IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] data, int length)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpServer_SendWSMessage(SenderPtr, connId, final, rsv, opCode, gch.AddrOfPinnedObject(), length, 0);
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool GetWsMessageState(IntPtr connId, out MessageState state)
        {
            var final = false;
            var reserved = Rsv.Off;
            var opCode = OpCode.Cont;
            var maskPtr = IntPtr.Zero;
            ulong bodyLength = 0;
            ulong bodyRemain = 0;
            var ok = Sdk.Http.HP_HttpServer_GetWSMessageState(SenderPtr, connId, ref final, ref reserved, ref opCode, ref maskPtr, ref bodyLength, ref bodyRemain);
            if (!ok)
            {
                state = null;
            }
            else
            {
                state = new MessageState
                {
                    Final = final,
                    Rsv = reserved,
                    OpCode = opCode,
                    BodyLength = bodyLength,
                    BodyRemain = bodyRemain,
                };

                if (maskPtr != IntPtr.Zero)
                {
                    state.Mask = new byte[4];
                    Marshal.Copy(maskPtr, state.Mask, 0, state.Mask.Length);
                }
            }

            return ok;
        }

        /// <inheritdoc />
        public bool IsUpgrade(IntPtr connId) => Sdk.Http.HP_HttpServer_IsUpgrade(SenderPtr, connId);

        /// <inheritdoc />
        public bool IsKeepAlive(IntPtr connId) => Sdk.Http.HP_HttpServer_IsKeepAlive(SenderPtr, connId);

        /// <inheritdoc />
        public HttpVersion GetVersion(IntPtr connId) => Sdk.Http.HP_HttpServer_GetVersion(SenderPtr, connId);

        /// <inheritdoc />
        public long GetContentLength(IntPtr connId) => Sdk.Http.HP_HttpServer_GetContentLength(SenderPtr, connId);

        /// <inheritdoc />
        public string GetContentType(IntPtr connId) => Sdk.Http.HP_HttpServer_GetContentType(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public string GetContentEncoding(IntPtr connId) => Sdk.Http.HP_HttpServer_GetContentEncoding(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public string GetTransferEncoding(IntPtr connId) => Sdk.Http.HP_HttpServer_GetTransferEncoding(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public HttpUpgradeType GetUpgradeType(IntPtr connId) => Sdk.Http.HP_HttpServer_GetUpgradeType(SenderPtr, connId);

        /// <inheritdoc />
        public int GetParseErrorInfo(IntPtr connId, out string errorMsg)
        {
            var ptr = IntPtr.Zero;
            var errorCode = Sdk.Http.HP_HttpServer_GetParseErrorCode(SenderPtr, connId, ref ptr);
            errorMsg = errorCode != 0 ? ptr.PtrToAnsiString() : string.Empty;
            return errorCode;
        }

        /// <inheritdoc />
        public string GetHeader(IntPtr connId, string name)
        {
            var ptr = IntPtr.Zero;
            var value = string.Empty;
            if (Sdk.Http.HP_HttpServer_GetHeader(SenderPtr, connId, name, ref ptr))
            {
                value = ptr.PtrToAnsiString();
            }
            return value;
        }

        /// <inheritdoc />
        public List<string> GetHeaders(IntPtr connId, string name)
        {
            var list = new List<string>();
            uint count = 0;
            Sdk.Http.HP_HttpServer_GetHeaders(SenderPtr, connId, name, null, ref count);
            if (count > 0)
            {
                var arr = new IntPtr[count];
                if (Sdk.Http.HP_HttpServer_GetHeaders(SenderPtr, connId, name, arr, ref count))
                {
                    foreach (var item in arr)
                    {
                        list.Add(item.PtrToAnsiString());
                    }
                }
            }
            return list;
        }

        /// <inheritdoc />
        public List<NameValue> GetAllHeaders(IntPtr connId)
        {
            var list = new List<NameValue>();
            uint count = 0;
            Sdk.Http.HP_HttpServer_GetAllHeaders(SenderPtr, connId, IntPtr.Zero, ref count);
            if (count > 0)
            {
                var headersArr = new NameValueIntPtr[count];
                if (Sdk.Http.HP_HttpServer_GetAllHeaders(SenderPtr, connId, Marshal.UnsafeAddrOfPinnedArrayElement(headersArr, 0), ref count))
                {
                    foreach (var item in headersArr)
                    {
                        list.Add(new NameValue
                        {
                            Name = item.Name.PtrToAnsiString(),
                            Value = item.Value.PtrToAnsiString(),
                        });
                    }
                }
            }
            return list;
        }

        /// <inheritdoc />
        public List<string> GetAllHeaderNames(IntPtr connId)
        {
            var list = new List<string>();
            uint count = 0;
            Sdk.Http.HP_HttpServer_GetAllHeaderNames(SenderPtr, connId, null, ref count);
            if (count > 0)
            {
                var arr = new IntPtr[count];
                if (Sdk.Http.HP_HttpServer_GetAllHeaderNames(SenderPtr, connId, arr, ref count))
                {
                    foreach (var item in arr)
                    {
                        list.Add(item.PtrToAnsiString());
                    }
                }
            }
            return list;
        }

        /// <inheritdoc />
        public string GetCookie(IntPtr connId, string key)
        {
            var ptr = IntPtr.Zero;
            var value = string.Empty;
            if (Sdk.Http.HP_HttpServer_GetCookie(SenderPtr, connId, key, ref ptr))
            {
                value = ptr.PtrToAnsiString();
            }
            return value;
        }

        /// <inheritdoc />
        public List<NameValue> GetAllCookies(IntPtr connId)
        {
            var list = new List<NameValue>();
            uint count = 0;
            Sdk.Http.HP_HttpServer_GetAllCookies(SenderPtr, connId, IntPtr.Zero, ref count);
            if (count > 0)
            {
                var headersArr = new NameValueIntPtr[count];
                if (Sdk.Http.HP_HttpServer_GetAllCookies(SenderPtr, connId, Marshal.UnsafeAddrOfPinnedArrayElement(headersArr, 0), ref count))
                {
                    foreach (var item in headersArr)
                    {
                        list.Add(new NameValue
                        {
                            Name = item.Name.PtrToAnsiString(),
                            Value = item.Value.PtrToAnsiString(),
                        });
                    }
                }
            }
            return list;
        }

        /// <inheritdoc />
        public string GetHost(IntPtr connId) => Sdk.Http.HP_HttpServer_GetHost(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public bool Release(IntPtr connId) => Sdk.Http.HP_HttpServer_Release(SenderPtr, connId);

        /// <inheritdoc />
        public bool StartHttp(IntPtr connId) => Sdk.Http.HP_HttpServer_StartHttp(SenderPtr, connId);

        /// <inheritdoc />
        public string GetUrlField(IntPtr connId, HttpUrlField urlField) => Sdk.Http.HP_HttpServer_GetUrlField(SenderPtr, connId, urlField).PtrToAnsiString();

        /// <inheritdoc />
        public ushort GetUrlFieldSet(IntPtr connId) => Sdk.Http.HP_HttpServer_GetUrlFieldSet(SenderPtr, connId);

        /// <inheritdoc />
        public string GetMethod(IntPtr connId) => Sdk.Http.HP_HttpServer_GetMethod(SenderPtr, connId).PtrToAnsiString();

#region SDK事件

#region SDK回调委托,防止GC

        private Sdk.OnPrepareListen _onPrepareListen;
        private Sdk.OnAccept _onAccept;
        private Sdk.OnSend _onSend;
        private Sdk.OnReceive _onReceive;
        private Sdk.OnClose _onClose;
        private Sdk.OnShutdown _onShutdown;
        private Sdk.OnHandShake _onHandShake;

        private Sdk.OnMessageBegin _onMessageBegin;
        private Sdk.OnRequestLine _onRequestLine;
        private Sdk.OnHeader _onHeader;
        private Sdk.OnHeadersComplete _onHeadersComplete;
        private Sdk.OnBody _onBody;
        private Sdk.OnChunkHeader _onChunkHeader;
        private Sdk.OnChunkComplete _onChunkComplete;
        private Sdk.OnMessageComplete _onMessageComplete;
        private Sdk.OnUpgrade _onUpgrade;
        private Sdk.OnParseError _onParseError;

        private Sdk.OnWsMessageHeader _onWsMessageHeader;
        private Sdk.OnWsMessageBody _onWsMessageBody;
        private Sdk.OnWsMessageComplete _onWsMessageComplete;

#endregion

        protected override void SetCallback()
        {
            _onPrepareListen = SdkOnPrepareListen;
            _onAccept = SdkOnAccept;
            _onSend = SdkOnSend;
            _onReceive = SdkOnReceive;
            _onClose = SdkOnClose;
            _onShutdown = SdkOnShutdown;
            _onHandShake = SdkOnHandShake;
            Sdk.Http.HP_Set_FN_HttpServer_OnPrepareListen(ListenerPtr, _onPrepareListen);
            Sdk.Http.HP_Set_FN_HttpServer_OnAccept(ListenerPtr, _onAccept);
            Sdk.Http.HP_Set_FN_HttpServer_OnSend(ListenerPtr, _onSend);
            Sdk.Http.HP_Set_FN_HttpServer_OnReceive(ListenerPtr, _onReceive);
            Sdk.Http.HP_Set_FN_HttpServer_OnClose(ListenerPtr, _onClose);
            Sdk.Http.HP_Set_FN_HttpServer_OnShutdown(ListenerPtr, _onShutdown);
            Sdk.Http.HP_Set_FN_HttpServer_OnHandShake(ListenerPtr, _onHandShake);

            _onMessageBegin = SdkOnMessageBegin;
            _onRequestLine = SdkOnRequestLine;
            _onHeader = SdkOnHeader;
            _onHeadersComplete = SdkOnHeadersComplete;
            _onBody = SdkOnBody;
            _onChunkHeader = SdkOnChunkHeader;
            _onChunkComplete = SdkOnChunkComplete;
            _onMessageComplete = SdkOnMessageComplete;
            _onUpgrade = SdkOnUpgrade;
            _onParseError = SdkOnParseError;
            Sdk.Http.HP_Set_FN_HttpServer_OnMessageBegin(ListenerPtr, _onMessageBegin);
            Sdk.Http.HP_Set_FN_HttpServer_OnRequestLine(ListenerPtr, _onRequestLine);
            Sdk.Http.HP_Set_FN_HttpServer_OnHeader(ListenerPtr, _onHeader);
            Sdk.Http.HP_Set_FN_HttpServer_OnHeadersComplete(ListenerPtr, _onHeadersComplete);
            Sdk.Http.HP_Set_FN_HttpServer_OnBody(ListenerPtr, _onBody);
            Sdk.Http.HP_Set_FN_HttpServer_OnChunkHeader(ListenerPtr, _onChunkHeader);
            Sdk.Http.HP_Set_FN_HttpServer_OnChunkComplete(ListenerPtr, _onChunkComplete);
            Sdk.Http.HP_Set_FN_HttpServer_OnMessageComplete(ListenerPtr, _onMessageComplete);
            Sdk.Http.HP_Set_FN_HttpServer_OnUpgrade(ListenerPtr, _onUpgrade);
            Sdk.Http.HP_Set_FN_HttpServer_OnParseError(ListenerPtr, _onParseError);

            _onWsMessageHeader = SdkOnWsMessageHeader;
            _onWsMessageBody = SdkOnWsMessageBody;
            _onWsMessageComplete = SdkOnWsMessageComplete;
            Sdk.Http.HP_Set_FN_HttpServer_OnWSMessageHeader(ListenerPtr, _onWsMessageHeader);
            Sdk.Http.HP_Set_FN_HttpServer_OnWSMessageBody(ListenerPtr, _onWsMessageBody);
            Sdk.Http.HP_Set_FN_HttpServer_OnWSMessageComplete(ListenerPtr, _onWsMessageComplete);

            GC.KeepAlive(_onPrepareListen);
            GC.KeepAlive(_onAccept);
            GC.KeepAlive(_onSend);
            GC.KeepAlive(_onReceive);
            GC.KeepAlive(_onClose);
            GC.KeepAlive(_onShutdown);
            GC.KeepAlive(_onHandShake);

            GC.KeepAlive(_onMessageBegin);
            GC.KeepAlive(_onRequestLine);
            GC.KeepAlive(_onHeader);
            GC.KeepAlive(_onHeadersComplete);
            GC.KeepAlive(_onBody);
            GC.KeepAlive(_onChunkHeader);
            GC.KeepAlive(_onChunkComplete);
            GC.KeepAlive(_onMessageComplete);
            GC.KeepAlive(_onUpgrade);
            GC.KeepAlive(_onParseError);

            GC.KeepAlive(_onWsMessageHeader);
            GC.KeepAlive(_onWsMessageBody);
            GC.KeepAlive(_onWsMessageComplete);
        }

        protected HandleResult SdkOnWsMessageHeader(IntPtr sender, IntPtr connId, bool final, Rsv rsv, OpCode operationCode, byte[] mask, ulong bodyLength) => OnWsMessageHeader?.Invoke(this, connId, final, rsv, operationCode, mask, bodyLength) ?? HandleResult.Ignore;

        protected HandleResult SdkOnWsMessageBody(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnWsMessageBody == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnWsMessageBody(this, connId, bytes);
        }

        protected HandleResult SdkOnWsMessageComplete(IntPtr sender, IntPtr connId)
        {
            return OnWsMessageComplete?.Invoke(this, connId) ?? HandleResult.Ignore;
        }

        protected HttpParseResult SdkOnMessageBegin(IntPtr sender, IntPtr connId) => OnMessageBegin?.Invoke(this, connId) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnHeader(IntPtr sender, IntPtr connId, string name, string value) => OnHeader?.Invoke(this, connId, name, value) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnChunkHeader(IntPtr sender, IntPtr connId, int length) => OnChunkHeader?.Invoke(this, connId, length) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnChunkComplete(IntPtr sender, IntPtr connId) => OnChunkComplete?.Invoke(this, connId) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnUpgrade(IntPtr sender, IntPtr connId, HttpUpgradeType upgradeType) => OnUpgrade?.Invoke(this, connId, upgradeType) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnRequestLine(IntPtr sender, IntPtr connId, string method, string url) => OnRequestLine?.Invoke(this, connId, method, url) ?? HttpParseResult.Ok;

        protected HttpParseResultEx SdkOnHeadersComplete(IntPtr sender, IntPtr connId) => OnHeadersComplete?.Invoke(this, connId) ?? HttpParseResultEx.Ok;

        protected HttpParseResult SdkOnBody(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnBody == null) return HttpParseResult.Ok;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnBody(this, connId, bytes);
        }

        protected HttpParseResult SdkOnMessageComplete(IntPtr sender, IntPtr connId) => OnMessageComplete?.Invoke(this, connId) ?? HttpParseResult.Ok;

        protected HttpParseResult SdkOnParseError(IntPtr sender, IntPtr connId, int errorCode, string errorDesc) => OnParseError?.Invoke(this, connId, errorCode, errorDesc) ?? HttpParseResult.Ok;

#endregion
    }
}
