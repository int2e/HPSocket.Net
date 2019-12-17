using System;
using System.Runtime.InteropServices;
using HPSocket.Ssl;

namespace HPSocket.Sdk
{
    internal static class Ssl
    {
        public delegate int SniServerNameCallback(string serverName);


        /**************** HPSocket4C 导出函数 ****************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLAgent(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPullServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPullAgent(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPullClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPackServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPackAgent(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SSLPackClient(IntPtr pListener);



        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLServer(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLAgent(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLClient(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPullServer(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPullAgent(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPullClient(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPackServer(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPackAgent(IntPtr pObj);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SSLPackClient(IntPtr pObj);

        /************************ SSL 初始化方法 ****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLAgent_SetupSSLContext(IntPtr pAgent, SslVerifyMode verifyMode, string lpszPemCertFile, string lpszPemKeyFile, string lpszKeyPassword, string lpszCaPemCertFileOrPath);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLAgent_SetupSSLContextByMemory(IntPtr pAgent, SslVerifyMode verifyMode /* SSL_VM_NONE */, string lpszPemCert /* nullptr */, string lpszPemKey /* nullptr */, string lpszKeyPassword /* nullptr */, string lpszCaPemCert /* nullptr */);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLClient_SetupSSLContext(IntPtr pClient, SslVerifyMode verifyMode, string lpszPemCertFile, string lpszPemKeyFile, string lpszKeyPassword, string lpszCaPemCertFileOrPath);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLClient_SetupSSLContextByMemory(IntPtr pClient, SslVerifyMode verifyMode /* SSL_VM_NONE */, string lpszPemCert /* nullptr */, string lpszPemKey /* nullptr */, string lpszKeyPassword /* nullptr */, string lpszCaPemCert /* nullptr */);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_SetupSSLContext(IntPtr pServer, SslVerifyMode verifyMode, string lpszPemCertFile, string lpszPemKeyFile, string lpszKeyPassword, string lpszCaPemCertFileOrPath, SniServerNameCallback fnServerNameCallback);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_SetupSSLContextByMemory(IntPtr pServer, SslVerifyMode verifyMode /* SSL_VM_NONE */, string lpszPemCert /* nullptr */, string lpszPemKey /* nullptr */, string lpszKeyPassword /* nullptr */, string lpszCaPemCert /* nullptr */, SniServerNameCallback fnServerNameCallback /* nullptr */);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_SSLServer_AddSSLContextByMemory(IntPtr pServer, SslVerifyMode verifyMode, string lpszPemCert, string lpszPemKey, string lpszKeyPassword /* nullptr */, string lpszCaPemCert /* nullptr */);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_BindSSLServerName(IntPtr pServer, string lpszServerName, int iContextIndex);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_SSL_DefaultServerNameCallback(string lpszServerName, IntPtr pContext);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_SSLServer_AddSSLContext(IntPtr pServer, SslVerifyMode verifyMode, string lpszPemCertFile, string lpszPemKeyFile, string lpszKeyPassword, string lpszCaPemCertFileOrPath);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLAgent_CleanupSSLContext(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLClient_CleanupSSLContext(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLServer_CleanupSSLContext(IntPtr pServer);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSL_RemoveThreadLocalState();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_StartSSLHandShake(IntPtr pServer, IntPtr dwConnId);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLServer_SetSSLAutoHandShake(IntPtr pServer, bool bAutoHandShake);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_IsSSLAutoHandShake(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLServer_SetSSLCipherList(IntPtr pServer, string lpszCipherList);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_SSLServer_GetSSLCipherList(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLServer_GetSSLSessionInfo(IntPtr pServer, IntPtr connId, SslSessionInfo enInfo, ref IntPtr lppInfo);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLAgent_StartSSLHandShake(IntPtr pAgent, IntPtr dwConnId);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLAgent_SetSSLAutoHandShake(IntPtr pAgent, bool bAutoHandShake);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLAgent_IsSSLAutoHandShake(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLAgent_SetSSLCipherList(IntPtr pAgent, string lpszCipherList);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_SSLAgent_GetSSLCipherList(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLAgent_GetSSLSessionInfo(IntPtr pAgent, IntPtr connId, SslSessionInfo enInfo, ref IntPtr lppInfo);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLClient_StartSSLHandShake(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLClient_SetSSLAutoHandShake(IntPtr pClient, bool bAutoHandShake);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLClient_IsSSLAutoHandShake(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_SSLClient_SetSSLCipherList(IntPtr pClient, string lpszCipherList);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_SSLClient_GetSSLCipherList(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_SSLClient_GetSSLSessionInfo(IntPtr pClient, SslSessionInfo sessionInfo, ref IntPtr lppInfo);
    }
}
