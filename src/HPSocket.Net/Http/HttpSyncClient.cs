using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HPSocket.Http
{
    public class HttpSyncClient : HttpClient, IHttpSyncClient, IHttpEasyData
    {
        public HttpSyncClient()
            : base(Sdk.Http.Create_HP_HttpClientListener,
                Sdk.Http.Create_HP_HttpSyncClient,
                Sdk.Http.Destroy_HP_HttpSyncClient,
                Sdk.Http.Destroy_HP_HttpClientListener)
        {
        }

        protected HttpSyncClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public bool AutoDecompression { get; set; } = true;

        /// <inheritdoc />
        public Encoding ResponseEncoding { get; set; }

        /// <inheritdoc />
        public uint ConnectTimeout
        {
            get => Sdk.Http.HP_HttpSyncClient_GetConnectTimeout(SenderPtr);
            set => Sdk.Http.HP_HttpSyncClient_SetConnectTimeout(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint RequestTimeout
        {
            get => Sdk.Http.HP_HttpSyncClient_GetRequestTimeout(SenderPtr);
            set => Sdk.Http.HP_HttpSyncClient_SetRequestTimeout(SenderPtr, value);
        }


        /// <inheritdoc />
        public bool OpenUrl(HttpMethod method, string url, List<NameValue> headers, byte[] body, int length, bool forceReconnect = false)
        {
            var gch = GCHandle.Alloc(body, GCHandleType.Pinned);
            var ok = Sdk.Http.HP_HttpSyncClient_OpenUrl(SenderPtr, method.ToNameString(), url, headers.ToArray(), headers.Count, gch.AddrOfPinnedObject(), length, forceReconnect);
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool CleanupRequestResult() => Sdk.Http.HP_HttpSyncClient_CleanupRequestResult(SenderPtr);

        /// <inheritdoc />
        public string GetResponseBody()
        {
            var ptr = IntPtr.Zero;
            var length = 0;
            var ok = Sdk.Http.HP_HttpSyncClient_GetResponseBody(SenderPtr, ref ptr, ref length);
            if (!ok || length <= 0)
            {
                return string.Empty;
            }

            var data = new byte[length];
            Marshal.Copy(ptr, data, 0, length);

            // 开启自动解压缩就尝试解压缩数据
            if (AutoDecompression)
            {
                data = data.HttpMessageDataDecompress(GetHeader("Content-Encoding"));
            }

            // 如果指定了响应编码则使用指定的
            if (ResponseEncoding != null)
            {
                return ResponseEncoding.GetString(data);
            }

            // 找 Content-Type 里的charset
            var contentEncoding = GetHeader("Content-Encoding") + ";";
            var charset = contentEncoding.GetMiddle("charset=", ";");

            Encoding defaultEncoding;
            if (!String.IsNullOrWhiteSpace(charset))
            {
                try
                {
                    // 找到了尝试根据charset指定的编码得到Encoding对象
                    defaultEncoding = Encoding.GetEncoding(charset);
                }
                catch
                {
                    // 指定编码无法转换为可用Encoding对象则默认使用utf8
                    defaultEncoding = Encoding.UTF8;
                }
            }
            else
            {
                // 没找到则默认使用utf8
                defaultEncoding = Encoding.UTF8;
            }
            return defaultEncoding.GetString(data);
        }
    }
}
