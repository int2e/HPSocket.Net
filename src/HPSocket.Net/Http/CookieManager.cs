using System.Text;

namespace HPSocket.Http
{
    /// <summary>
    /// cookie 管理器
    /// </summary>
    public class CookieManager
    {
        /// <summary>
        /// 从文件加载 Cookie
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="keepExists"></param>
        /// <returns></returns>
        public static bool LoadFromFile(string filePath, bool keepExists = true) => Sdk.Http.HP_HttpCookie_MGR_LoadFromFile(filePath, keepExists);

        /// <summary>
        /// 保存 Cookie 到文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="keepExists"></param>
        /// <returns></returns>
        public static bool SaveToFile(string filePath, bool keepExists = true) => Sdk.Http.HP_HttpCookie_MGR_SaveToFile(filePath, keepExists);

        /// <summary>
        /// 清理 Cookie
        /// </summary>
        /// <returns></returns>
        public static bool ClearCookies(string domain = null, string path = null) => Sdk.Http.HP_HttpCookie_MGR_ClearCookies(domain, path);

        /// <summary>
        /// 清理过期 Cookie
        /// </summary>
        /// <returns></returns>
        public static bool RemoveExpiredCookies(string domain = null, string path = null) => Sdk.Http.HP_HttpCookie_MGR_RemoveExpiredCookies(domain, path);

        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="maxAge"></param>
        /// <param name="httpOnly"></param>
        /// <param name="secure"></param>
        /// <param name="sameSite"></param>
        /// <param name="onlyUpdateValueIfExists"></param>
        /// <returns></returns>
        public static bool SetCookie(string name, string value, string domain, string path, int maxAge = -1, bool httpOnly = false, bool secure = false, int sameSite = 0, bool onlyUpdateValueIfExists = true) => Sdk.Http.HP_HttpCookie_MGR_SetCookie(name, value, domain, path, maxAge, httpOnly, secure, sameSite, onlyUpdateValueIfExists);

        /// <summary>
        /// 删除 Cookie
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool DeleteCookie(string domain, string path, string name) => Sdk.Http.HP_HttpCookie_MGR_DeleteCookie(domain, path, name);

        /// <summary>
        /// 获取或设置是否允许第三方 Cookie
        /// </summary>
        public static bool IsEnableThirdPartyCookie
        {
            get => Sdk.Http.HP_HttpCookie_MGR_IsEnableThirdPartyCookie();
            set => Sdk.Http.HP_HttpCookie_MGR_SetEnableThirdPartyCookie(value);
        }

        /// <summary>
        /// Cookie expires 字符串转换为整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static bool ParseExpires(string str, out ulong expires)
        {
            expires = 0;
            return Sdk.Http.HP_HttpCookie_HLP_ParseExpires(str, ref expires);
        }

        /// <summary>
        /// 整数转换为 Cookie expires 字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static bool MakeExpiresStr(ulong val, out string expires)
        {
            var length = 60;
            var sb = new StringBuilder(length);
            var ok = Sdk.Http.HP_HttpCookie_HLP_MakeExpiresStr(sb, ref length, val);
            expires = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <summary>
        /// 生成 Cookie 字符串
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="maxAge"></param>
        /// <param name="httpOnly"></param>
        /// <param name="secure"></param>
        /// <param name="sameSite"></param>
        /// <returns></returns>
        public static bool ToString(out string cookie, string name, string value, string domain, string path, int maxAge = -1, bool httpOnly = false, bool secure = false, int sameSite = 0)
        {
            var length = 1024;
            var sb = new StringBuilder(length);
            var ok = Sdk.Http.HP_HttpCookie_HLP_ToString(sb, ref length, name, value, domain, path, maxAge, httpOnly, secure, sameSite);
            cookie = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <summary>
        /// 获取当前 UTC 时间
        /// </summary>
        /// <returns></returns>
        public static ulong CurrentUtcTime() => Sdk.Http.HP_HttpCookie_HLP_CurrentUTCTime();

        /// <summary>
        /// Max-Age 到 expires
        /// </summary>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public static ulong MaxAgeToExpires(int maxAge) => Sdk.Http.HP_HttpCookie_HLP_MaxAgeToExpires(maxAge);

        /// <summary>
        /// expires 到 Max-Age
        /// </summary>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static int MaxAgeToExpires(ulong expires) => Sdk.Http.HP_HttpCookie_HLP_ExpiresToMaxAge(expires);
    }
}
