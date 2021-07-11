using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using HPSocket.Tcp;
using HPSocket.WebSocket;

namespace HPSocket.Http
{
    public class HttpAgent : TcpAgent, IHttpAgent
    {
        /// <summary>
        /// 默认请求头
        /// </summary>
        private static readonly List<NameValue> DefaultRequestHeaders = new List<NameValue>
        {
            new NameValue{ Name="Accept", Value="text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"},
            new NameValue{ Name="User-Agent", Value="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36 Edg/90.0.818.41"},
        };

        public HttpAgent()
            : base(Sdk.Http.Create_HP_HttpAgentListener,
                Sdk.Http.Create_HP_HttpAgent,
                Sdk.Http.Destroy_HP_HttpAgent,
                Sdk.Http.Destroy_HP_HttpAgentListener)
        {
            OnProxyConnected += HttpAgentOnProxyConnected;
        }

        protected HttpAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
            OnProxyConnected += HttpAgentOnProxyConnected;
        }

        /// <inheritdoc />
        public bool HttpAutoStart
        {
            get => Sdk.Http.HP_HttpAgent_IsHttpAutoStart(SenderPtr);
            set => Sdk.Http.HP_HttpAgent_SetHttpAutoStart(SenderPtr, value);
        }

        /// <inheritdoc />
        public HttpVersion LocalVersion
        {
            get => Sdk.Http.HP_HttpAgent_GetLocalVersion(SenderPtr);
            set => Sdk.Http.HP_HttpAgent_SetLocalVersion(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsUseCookie
        {
            get => Sdk.Http.HP_HttpAgent_IsUseCookie(SenderPtr);
            set => Sdk.Http.HP_HttpAgent_SetUseCookie(SenderPtr, value);
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
        public event StatusLineEventHandler OnStatusLine;

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

        public override bool Start()
        {
            if (ProxyList?.Count > 0)
            {
                HttpAutoStart = false;
            }

            return base.Start();
        }

        protected virtual void HttpAgentOnProxyConnected(IAgent sender, IntPtr connId, IProxy proxy)
        {
            StartHttp(connId);
        }

        /// <inheritdoc />
        public bool SendRequest(IntPtr connId, HttpMethod method, string path, List<NameValue> headers, byte[] body, int length)
        {
            if (headers == null)
            {
                headers = DefaultRequestHeaders;
            }
            var gch = GCHandle.Alloc(body, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpAgent_SendRequest(SenderPtr, connId, method.ToNameString(), path, headers.ToArray(), headers.Count, gch.AddrOfPinnedObject(), length);
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendRequest(IntPtr connId, HttpMethod method, string path, List<NameValue> headers)
        {
            if (headers == null)
            {
                headers = DefaultRequestHeaders;
            }

            return Sdk.Http.HP_HttpAgent_SendRequest(SenderPtr, connId, method.ToNameString(), path, headers.ToArray(), headers.Count, IntPtr.Zero, 0);
        }

        /// <summary>
        /// http agent 不实现当前方法, 请调用带HttpMethod版本的SendSmallFile方法
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        [Obsolete("http/https agent 不实现当前方法, 请调用带HttpMethod版本的SendSmallFile方法", true)]
        public new bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail) => false;

        /// <summary>
        /// http server 不实现当前方法, 请调用带HttpMethod版本的SendSmallFile方法
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        [Obsolete("http/https agent 不实现当前方法, 请调用带HttpMethod版本的SendSmallFile方法", true)]
        public new bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail) => false;

        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, HttpMethod method, string path, List<NameValue> headers, string filePath) => Sdk.Http.HP_HttpAgent_SendLocalFile(SenderPtr, connId, filePath, method.ToNameString(), path, headers.ToArray(), headers.Count);

        /// <inheritdoc />
        public bool SendChunkData(IntPtr connId, byte[] data, int length, string extensions = null)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpAgent_SendChunkData(SenderPtr, connId, gch.AddrOfPinnedObject(), length, extensions);
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendWsMessage(IntPtr connId, MessageState state, byte[] data, int length) => SendWsMessage(connId, state.Final, state.Rsv, state.OpCode, state.Mask, data, length);

        /// <inheritdoc />
        public bool SendWsMessage(IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, byte[] data, int length)
        {
            if (mask.Length != 4)
            {
                throw new ArgumentException("mask数组长度只能是4字节的", nameof(mask));
            }
            var gchData = GCHandle.Alloc(data, GCHandleType.Pinned);
            var gchMask = GCHandle.Alloc(data, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpAgent_SendWSMessage(SenderPtr, connId, final, rsv, opCode, gchMask.AddrOfPinnedObject(), gchData.AddrOfPinnedObject(), length, 0);
            gchData.Free();
            gchMask.Free();
            return ok;
        }

        /// <inheritdoc />
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        public bool SendPost(IntPtr connId, string path, List<NameValue> headers, string body, int length) => Sdk.Http.HP_HttpAgent_SendPost(SenderPtr, connId, path, headers.ToArray(), headers.Count, body, length);

        /// <inheritdoc />
        public bool SendPost(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length) => SendRequest(connId, HttpMethod.Post, path, headers, body, length);

        /// <inheritdoc />
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        public bool SendPut(IntPtr connId, string path, List<NameValue> headers, string body, int length) => Sdk.Http.HP_HttpAgent_SendPut(SenderPtr, connId, path, headers.ToArray(), headers.Count, body, length);

        /// <inheritdoc />
        public bool SendPut(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length) => SendRequest(connId, HttpMethod.Put, path, headers, body, length);

        /// <inheritdoc />
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        public bool SendPatch(IntPtr connId, string path, List<NameValue> headers, string body, int length) => Sdk.Http.HP_HttpAgent_SendPatch(SenderPtr, connId, path, headers.ToArray(), headers.Count, body, length);

        /// <inheritdoc />
        public bool SendPatch(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length) => SendRequest(connId, HttpMethod.Patch, path, headers, body, length);

        /// <inheritdoc />
        public bool SendGet(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Get, path, headers);

        /// <inheritdoc />
        public bool SendDelete(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Delete, path, headers);

        /// <inheritdoc />
        public bool SendHead(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Head, path, headers);

        /// <inheritdoc />
        public bool SendTrace(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Trace, path, headers);

        /// <inheritdoc />
        public bool SendOptions(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Options, path, headers);

        /// <inheritdoc />
        public bool SendConnect(IntPtr connId, string path, List<NameValue> headers) => SendRequest(connId, HttpMethod.Connect, path, headers);

        /// <inheritdoc />
        public bool StartHttp(IntPtr connId) => Sdk.Http.HP_HttpAgent_StartHttp(SenderPtr, connId);

        /// <inheritdoc />
        public bool GetWsMessageState(IntPtr connId, out MessageState state)
        {
            var final = false;
            var reserved = Rsv.Off;
            var opCode = OpCode.Cont;
            var maskPtr = IntPtr.Zero;
            ulong bodyLength = 0;
            ulong bodyRemain = 0;
            var ok = Sdk.Http.HP_HttpAgent_GetWSMessageState(SenderPtr, connId, ref final, ref reserved, ref opCode, ref maskPtr, ref bodyLength, ref bodyRemain);
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
        public HttpStatusCode GetStatusCode(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetStatusCode(SenderPtr, connId);

        /// <inheritdoc />
        public bool IsUpgrade(IntPtr connId) => Sdk.Http.HP_HttpAgent_IsUpgrade(SenderPtr, connId);

        /// <inheritdoc />
        public bool IsKeepAlive(IntPtr connId) => Sdk.Http.HP_HttpAgent_IsKeepAlive(SenderPtr, connId);

        /// <inheritdoc />
        public HttpVersion GetVersion(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetVersion(SenderPtr, connId);

        /// <inheritdoc />
        public long GetContentLength(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetContentLength(SenderPtr, connId);

        /// <inheritdoc />
        public string GetContentType(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetContentType(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public string GetContentEncoding(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetContentEncoding(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public string GetTransferEncoding(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetTransferEncoding(SenderPtr, connId).PtrToAnsiString();

        /// <inheritdoc />
        public HttpUpgradeType GetUpgradeType(IntPtr connId) => Sdk.Http.HP_HttpAgent_GetUpgradeType(SenderPtr, connId);

        /// <inheritdoc />
        public int GetParseErrorInfo(IntPtr connId, out string errorMsg)
        {
            var ptr = IntPtr.Zero;
            var errorCode = Sdk.Http.HP_HttpAgent_GetParseErrorCode(SenderPtr, connId, ref ptr);
            errorMsg = errorCode != 0 ? ptr.PtrToAnsiString() : string.Empty;
            return errorCode;
        }

        /// <inheritdoc />
        public string GetHeader(IntPtr connId, string name)
        {
            var ptr = IntPtr.Zero;
            var value = string.Empty;
            if (Sdk.Http.HP_HttpAgent_GetHeader(SenderPtr, connId, name, ref ptr))
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
            Sdk.Http.HP_HttpAgent_GetHeaders(SenderPtr, connId, name, null, ref count);
            if (count > 0)
            {
                var arr = new IntPtr[count];
                if (Sdk.Http.HP_HttpAgent_GetHeaders(SenderPtr, connId, name, arr, ref count))
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
            Sdk.Http.HP_HttpAgent_GetAllHeaders(SenderPtr, connId, IntPtr.Zero, ref count);
            if (count > 0)
            {
                var headersArr = new NameValueIntPtr[count];
                if (Sdk.Http.HP_HttpAgent_GetAllHeaders(SenderPtr, connId, Marshal.UnsafeAddrOfPinnedArrayElement(headersArr, 0), ref count))
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
            Sdk.Http.HP_HttpAgent_GetAllHeaderNames(SenderPtr, connId, null, ref count);
            if (count > 0)
            {
                var arr = new IntPtr[count];
                if (Sdk.Http.HP_HttpAgent_GetAllHeaderNames(SenderPtr, connId, arr, ref count))
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
            if (Sdk.Http.HP_HttpAgent_GetCookie(SenderPtr, connId, key, ref ptr))
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
            Sdk.Http.HP_HttpAgent_GetAllCookies(SenderPtr, connId, IntPtr.Zero, ref count);
            if (count > 0)
            {
                var headersArr = new NameValueIntPtr[count];
                if (Sdk.Http.HP_HttpAgent_GetAllCookies(SenderPtr, connId, Marshal.UnsafeAddrOfPinnedArrayElement(headersArr, 0), ref count))
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

        #region SDK事件

        #region SDK回调委托,防止GC

        private Sdk.OnPrepareConnect _onPrepareConnect;
        private Sdk.OnConnect _onConnect;
        private Sdk.OnSend _onSend;
        private Sdk.OnReceive _onReceive;
        private Sdk.OnClose _onClose;
        private Sdk.OnShutdown _onShutdown;
        private Sdk.OnHandShake _onHandShake;

        private Sdk.OnMessageBegin _onMessageBegin;
        private Sdk.OnStatusLine _onStatusLine;
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
            _onPrepareConnect = SdkOnPrepareConnect;
            _onConnect = SdkOnConnect;
            _onSend = SdkOnSend;
            _onReceive = SdkOnReceive;
            _onClose = SdkOnClose;
            _onShutdown = SdkOnShutdown;
            _onHandShake = SdkOnHandShake;
            Sdk.Http.HP_Set_FN_HttpAgent_OnPrepareConnect(ListenerPtr, _onPrepareConnect);
            Sdk.Http.HP_Set_FN_HttpAgent_OnConnect(ListenerPtr, _onConnect);
            Sdk.Http.HP_Set_FN_HttpAgent_OnSend(ListenerPtr, _onSend);
            Sdk.Http.HP_Set_FN_HttpAgent_OnReceive(ListenerPtr, _onReceive);
            Sdk.Http.HP_Set_FN_HttpAgent_OnClose(ListenerPtr, _onClose);
            Sdk.Http.HP_Set_FN_HttpAgent_OnShutdown(ListenerPtr, _onShutdown);
            Sdk.Http.HP_Set_FN_HttpAgent_OnHandShake(ListenerPtr, _onHandShake);

            _onMessageBegin = SdkOnMessageBegin;
            _onStatusLine = SdkOnStatusLine;
            _onHeader = SdkOnHeader;
            _onHeadersComplete = SdkOnHeadersComplete;
            _onBody = SdkOnBody;
            _onChunkHeader = SdkOnChunkHeader;
            _onChunkComplete = SdkOnChunkComplete;
            _onMessageComplete = SdkOnMessageComplete;
            _onUpgrade = SdkOnUpgrade;
            _onParseError = SdkOnParseError;
            Sdk.Http.HP_Set_FN_HttpAgent_OnMessageBegin(ListenerPtr, _onMessageBegin);
            Sdk.Http.HP_Set_FN_HttpAgent_OnStatusLine(ListenerPtr, _onStatusLine);
            Sdk.Http.HP_Set_FN_HttpAgent_OnHeader(ListenerPtr, _onHeader);
            Sdk.Http.HP_Set_FN_HttpAgent_OnHeadersComplete(ListenerPtr, _onHeadersComplete);
            Sdk.Http.HP_Set_FN_HttpAgent_OnBody(ListenerPtr, _onBody);
            Sdk.Http.HP_Set_FN_HttpAgent_OnChunkHeader(ListenerPtr, _onChunkHeader);
            Sdk.Http.HP_Set_FN_HttpAgent_OnChunkComplete(ListenerPtr, _onChunkComplete);
            Sdk.Http.HP_Set_FN_HttpAgent_OnMessageComplete(ListenerPtr, _onMessageComplete);
            Sdk.Http.HP_Set_FN_HttpAgent_OnUpgrade(ListenerPtr, _onUpgrade);
            Sdk.Http.HP_Set_FN_HttpAgent_OnParseError(ListenerPtr, _onParseError);

            _onWsMessageHeader = SdkOnWsMessageHeader;
            _onWsMessageBody = SdkOnWsMessageBody;
            _onWsMessageComplete = SdkOnWsMessageComplete;
            Sdk.Http.HP_Set_FN_HttpAgent_OnWSMessageHeader(ListenerPtr, _onWsMessageHeader);
            Sdk.Http.HP_Set_FN_HttpAgent_OnWSMessageBody(ListenerPtr, _onWsMessageBody);
            Sdk.Http.HP_Set_FN_HttpAgent_OnWSMessageComplete(ListenerPtr, _onWsMessageComplete);

            GC.KeepAlive(_onPrepareConnect);
            GC.KeepAlive(_onConnect);
            GC.KeepAlive(_onSend);
            GC.KeepAlive(_onReceive);
            GC.KeepAlive(_onClose);
            GC.KeepAlive(_onShutdown);
            GC.KeepAlive(_onHandShake);

            GC.KeepAlive(_onMessageBegin);
            GC.KeepAlive(_onStatusLine);
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

        protected HttpParseResult SdkOnStatusLine(IntPtr sender, IntPtr connId, ushort statusCode, string desc) => OnStatusLine?.Invoke(this, connId, statusCode, desc) ?? HttpParseResult.Ok;

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
