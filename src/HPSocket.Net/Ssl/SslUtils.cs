using System;

namespace HPSocket.Ssl
{
    /// <summary>
    /// ssl 工具类
    /// </summary>
    public class SslUtils
    {
        /// <summary>
        /// SNI 默认回调函数
        /// <para>SSL Server 的 SetupSSLContext 方法中如果不指定 SNI 回调函数则使用此 SNI 默认回调函数</para>
        /// </summary>
        /// <param name="serverName">请求域名</param>
        /// <param name="context">ssl context 对象</param>
        /// <returns>SNI 主机证书对应的索引</returns>
        public static int DefaultServerNameCallback(string serverName, IntPtr context) => Sdk.Ssl.HP_SSL_DefaultServerNameCallback(serverName, context);

        /// <summary>
        /// 清理线程局部环境 SSL 资源
        ///<para>清理 SSL 全局运行环境，回收 SSL 相关内存</para> 
        ///<para>任何一个操作 SSL 的线程，通信结束时都需要清理线程局部环境 SSL 资源</para> 
        ///<para>1、主线程和 HP-Socket 工作线程在通信结束时会自动清理线程局部环境 SSL 资源。因此，一般情况下不必手工调用本方法</para> 
        ///<para>2、特殊情况下，当自定义线程参与 HP-Socket 通信操作并检查到 SSL 内存泄漏时，需在每次通信结束时自定义线程调用本方法</para> 
        /// </summary>
        public static void RemoveThreadLocalState() => Sdk.Ssl.HP_SSL_RemoveThreadLocalState();
    }
}
