#region license
/*
 *
 * 部分websocket相关扩展方法抄自 websocket-sharp  https://github.com/sta/websocket-sharp
 * BytesExtensions.IndexesOf() 方法 Boyer-Moore-Horspool 搜索, 来自 https://blog.csdn.net/lindexi_gd/article/details/100174714 
 * BoyerMoore 类来自 https://gist.github.com/mjs3339/0772431281093f1bca1fce2f2eca527d
 *
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
#if !NET20 && !NET30 && !NET35
using System.Linq;
#endif
using System.Runtime.InteropServices;
using System.Text;

using HPSocket.Http;
using HPSocket.Tcp;
using HPSocket.WebSocket;

#if NET20 || NET30 || NET35
namespace System.Runtime.CompilerServices
{
    internal class ExtensionAttribute : Attribute { }
}
#endif

namespace HPSocket
{
    #region net20,30,35
#if NET20 || NET30 || NET35
    internal partial class String
    {
        internal static bool IsNullOrWhiteSpace(string str)
        {
            return string.IsNullOrEmpty(str) || str.Trim() == string.Empty;
        }
    }

    internal static class MemoryStreamExtensions
    {
        internal static void CopyTo(this Stream stream, Stream destination, int bufferSize)
        {
            int num;
            byte[] buffer = new byte[bufferSize];
            while ((num = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, num);
            }
        }
    }
#endif
    #endregion

    #region websocket

    internal static class WebSocketExtensions
    {
        private static readonly byte[] Last = { 0x00 };
        private static byte[] Compress(this byte[] data)
        {
            if (data.LongLength == 0)
                //return new byte[] { 0x00, 0x00, 0x00, 0xff, 0xff };
                return data;

            using (var input = new MemoryStream(data))
                return input.CompressToArray();
        }

        private static MemoryStream Compress(this Stream stream)
        {
            var output = new MemoryStream();
            if (stream.Length == 0)
                return output;

            stream.Position = 0;
            using (var ds = new DeflateStream(output, CompressionMode.Compress, true))
            {
                stream.CopyTo(ds, 1024);
                ds.Close(); // BFINAL set to 1.
                output.Write(Last, 0, 1);
                output.Position = 0;

                return output;
            }
        }

        private static byte[] CompressToArray(this Stream stream)
        {
            using (var output = stream.Compress())
            {
                output.Close();
                return output.ToArray();
            }
        }

        private static byte[] Decompress(this byte[] data)
        {
            if (data.LongLength == 0)
                return data;

            using (var input = new MemoryStream(data))
                return input.DecompressToArray();
        }
        private static MemoryStream Decompress(this Stream stream)
        {
            var output = new MemoryStream();
            if (stream.Length == 0)
                return output;

            stream.Position = 0;
            using (var ds = new DeflateStream(stream, CompressionMode.Decompress, true))
            {
                ds.CopyTo(output, 1024);
                output.Position = 0;

                return output;
            }
        }

        private static byte[] DecompressToArray(this Stream stream)
        {
            using (var output = stream.Decompress())
            {
                output.Close();
                return output.ToArray();
            }
        }

        internal static byte[] Compress(this byte[] data, CompressionMethod method)
        {
            return method == CompressionMethod.Deflate
                ? data.Compress()
                : data;
        }

        internal static byte[] Decompress(this byte[] data, CompressionMethod method)
        {
            return method == CompressionMethod.Deflate
                ? data.Decompress()
                : data;
        }

        internal static bool IsData(this byte opCode)
        {
            return opCode == 0x1 || opCode == 0x2;
        }

        internal static bool IsData(this OpCode opCode)
        {
            return opCode == OpCode.Text || opCode == OpCode.Binary;
        }

        internal static bool MaybeUri(this string value)
        {
            if (value == null)
                return false;

            if (value.Length == 0)
                return false;

            var idx = value.IndexOf(':');
            if (idx == -1)
                return false;

            if (idx >= 10)
                return false;

            var scheme = value.Substring(0, idx);
            return scheme.IsPredefinedScheme();
        }

        /// <summary>
        /// Determines whether the specified string is a predefined scheme.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is a predefined scheme;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <param name="value">
        /// A <see cref="string"/> to test.
        /// </param>
        internal static bool IsPredefinedScheme(this string value)
        {
            if (value == null || value.Length < 2)
                return false;

            var c = value[0];
            if (c == 'h')
                return value == "http" || value == "https";

            if (c == 'w')
                return value == "ws" || value == "wss";

            if (c == 'f')
                return value == "file" || value == "ftp";

            if (c == 'g')
                return value == "gopher";

            if (c == 'm')
                return value == "mailto";

            if (c == 'n')
            {
                c = value[1];
                return c == 'e'
                    ? value == "news" || value == "net.pipe" || value == "net.tcp"
                    : value == "nntp";
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified string is a URI string.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> may be a URI string;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <param name="value">
        /// A <see cref="string"/> to test.
        /// </param>
        internal static Uri ToUri(this string value)
        {
            Uri.TryCreate(
                value, value.MaybeUri() ? UriKind.Absolute : UriKind.Relative, out var ret
            );

            return ret;
        }

        /// <summary>
        /// Tries to create a new <see cref="Uri"/> for WebSocket with
        /// the specified <paramref name="uriString"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="Uri"/> was successfully created;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <param name="uriString">
        /// A <see cref="string"/> that represents a WebSocket URL to try.
        /// </param>
        /// <param name="result">
        /// When this method returns, a <see cref="Uri"/> that
        /// represents the WebSocket URL or <see langword="null"/>
        /// if <paramref name="uriString"/> is invalid.
        /// </param>
        /// <param name="message">
        /// When this method returns, a <see cref="string"/> that
        /// represents an error message or <see langword="null"/>
        /// if <paramref name="uriString"/> is valid.
        /// </param>
        internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
        {
            result = null;
            message = null;

            var uri = uriString.ToUri();
            if (uri == null)
            {
                message = "An invalid URI string.";
                return false;
            }

            if (!uri.IsAbsoluteUri)
            {
                message = "A relative URI.";
                return false;
            }

            var scheme = uri.Scheme;
            if (!(scheme == "ws" || scheme == "wss"))
            {
                message = "The scheme part is not 'ws' or 'wss'.";
                return false;
            }

            var port = uri.Port;
            if (port == 0)
            {
                message = "The port part is zero.";
                return false;
            }

            if (uri.Fragment.Length > 0)
            {
                message = "It includes the fragment component.";
                return false;
            }

            result = port != -1
                     ? uri
                     : new Uri($"{scheme}://{uri.Host}:{(scheme == "ws" ? 80 : 443)}{uri.PathAndQuery}");

            return true;
        }

        internal static CompressionMethod ToCompressionMethod(this string value)
        {
            var methods = Enum.GetValues(typeof(CompressionMethod));
            foreach (CompressionMethod method in methods)
            {
                if (method.ToExtensionString() == value)
                    return method;
            }

            return CompressionMethod.None;
        }

        internal static bool IsCompressionExtension(
            this string value, CompressionMethod method
        )
        {
            return value.StartsWith(method.ToExtensionString());
        }

        internal static string ToExtensionString(
            this CompressionMethod method, params string[] parameters
        )
        {
            if (method == CompressionMethod.None)
                return string.Empty;

            var name = $"permessage-{method.ToString().ToLower()}";

            return parameters != null && parameters.Length > 0
                ? $"{name}; {parameters.ToString("; ")}"
                : name;
        }

        /// <summary>
        /// Converts the specified array to a string.
        /// </summary>
        /// <returns>
        ///   <para>
        ///   A <see cref="string"/> converted by concatenating each element of
        ///   <paramref name="array"/> across <paramref name="separator"/>.
        ///   </para>
        ///   <para>
        ///   An empty string if <paramref name="array"/> is an empty array.
        ///   </para>
        /// </returns>
        /// <param name="array">
        /// An array of T to convert.
        /// </param>
        /// <param name="separator">
        /// A <see cref="string"/> used to separate each element of
        /// <paramref name="array"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type of elements in <paramref name="array"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        internal static string ToString<T>(this T[] array, string separator)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var len = array.Length;
            if (len == 0)
                return string.Empty;

            if (separator == null)
                separator = string.Empty;

            var buff = new StringBuilder(64);
            var end = len - 1;

            for (var i = 0; i < end; i++)
                buff.AppendFormat("{0}{1}", array[i], separator);

            buff.Append(array[end]);
            return buff.ToString();
        }

        internal static IEnumerable<string> SplitHeaderValue(
            this string value, params char[] separators
        )
        {
            var len = value.Length;

            var buff = new StringBuilder(32);
            var end = len - 1;
            var escaped = false;
            var quoted = false;

            for (var i = 0; i <= end; i++)
            {
                var c = value[i];
                buff.Append(c);

                if (c == '"')
                {
                    if (escaped)
                    {
                        escaped = false;
                        continue;
                    }

                    quoted = !quoted;
                    continue;
                }

                if (c == '\\')
                {
                    if (i == end)
                        break;

                    if (value[i + 1] == '"')
                        escaped = true;

                    continue;
                }

                if (Array.IndexOf(separators, c) > -1)
                {
                    if (quoted)
                        continue;

                    buff.Length -= 1;
                    yield return buff.ToString();

                    buff.Length = 0;
                }
            }

            yield return buff.ToString();
        }

        internal static string GetRandomWebSocketKey(this string str)
        {
            var bytes = new byte[16];
            new Random(Guid.NewGuid().GetHashCode()).NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    #endregion

    #region other

    public static class HttpEasyDataExtensions
    {
        public static string GetMiddle(this string html, string begin, string end, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            var startPosition = html.IndexOf(begin, comparisonType);
            if (startPosition == -1)
            {
                return "";
            }

            var msgPosition = startPosition + begin.Length;
            var endPosition = html.IndexOf(end, msgPosition, comparisonType);
            if (endPosition == -1)
            {
                return "";
            }

            return html.Substring(msgPosition, endPosition - msgPosition);
        }

        /// <summary>
        /// http message data decompress
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public static byte[] HttpMessageDataDecompress(this byte[] data, string contentEncoding)
        {
            if (contentEncoding == "")
            {
                return data;
            }

            Stream stream;
            if (contentEncoding.ToLower().Trim() == "gzip")
            {
                stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            }
            else if (contentEncoding.ToLower().Trim() == "deflate")
            {
                stream = new DeflateStream(new MemoryStream(data), CompressionMode.Decompress);
            }
            else
            {
                return data;
            }

            using (var ms = new MemoryStream())
            {
#if NET20 || NET30 || NET35
                stream.CopyTo(ms, 4096);
#else
                stream.CopyTo(ms);
#endif
                stream.Dispose();
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// 非托管附加数据扩展
    /// </summary>
    internal static class NativeExtraExtensions
    {
        /// <summary>
        /// 伪引用计数
        /// </summary>
        private static readonly ExtraData<IntPtr, int> ReferenceData = new ExtraData<IntPtr, int>();

        /// <summary>
        /// 结构体转 int ptr, 必须调用 IntPtr.FreeNativeExtraIntPtr()
        /// </summary>
        /// <param name="extra"></param>
        /// <returns></returns>
        internal static IntPtr ToIntPtr(this NativeExtra extra)
        {
            lock (ReferenceData)
            {
#if NET20 || NET30 || NET35 || NET40 || NET45
                var size = Marshal.SizeOf(typeof(NativeExtra));
                var ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(extra, ptr, true);
#else
                var size = Marshal.SizeOf<NativeExtra>();
                var ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr<NativeExtra>(extra, ptr, true);
#endif
              
                ReferenceData.Set(ptr, 1);
                return ptr;
            }
        }

        /// <summary>
        /// IntPtr转NativeExtra
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal static NativeExtra ToNativeExtra(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("参数不能为IntPtr.Zero", nameof(ptr));
            }

            lock (ReferenceData)
            {
                // 针对可能出现的指针不存在而做的特殊处理
                if (!ReferenceData.ContainsKey(ptr))
                {
                    return new NativeExtra();
                }
#if NET20 || NET30 || NET35 || NET40 || NET45
                return (NativeExtra)Marshal.PtrToStructure(ptr, typeof(NativeExtra));
#else
                return Marshal.PtrToStructure<NativeExtra>(ptr);
#endif
            }
        }

        /// <summary>
        /// 释放由NativeExtra.ToIntPtr()申请的内存
        /// </summary>
        /// <param name="ptr"></param>
        internal static void FreeNativeExtraIntPtr(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("参数不能为IntPtr.Zero", nameof(ptr));
            }

            lock (ReferenceData)
            {
                if (ReferenceData.ContainsKey(ptr))
                {
                    var count = ReferenceData.Get(ptr);
                    if (count == 1)
                    {
                        ReferenceData.Remove(ptr);
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
        }
    }

    /// <summary>
    /// HttpMethod 扩展
    /// </summary>
    internal static class HttpMethodExtensions
    {
        /// <summary>
        /// 到名称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static string ToNameString(this HttpMethod method)
        {
            return Enum.GetName(typeof(HttpMethod), method)?.ToUpper();
        }
    }

    /// <summary>
    /// HttpStatusCode 扩展
    /// </summary>
    internal static class HttpStatusCodeExtensions
    {
        /// <summary>
        /// 到短正整数
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        internal static ushort ToUInt16(this HttpStatusCode statusCode)
        {
            return (ushort)statusCode;
        }

        /// <summary>
        /// 到名称
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        internal static string ToNameString(this HttpStatusCode statusCode)
        {
            return Enum.GetName(typeof(HttpStatusCode), statusCode);
        }
    }

    /// <summary>
    /// IntPtr 扩展
    /// </summary>
    internal static class IntPtrExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        internal static string PtrToAnsiString(this IntPtr ptr)
        {
            var str = string.Empty;
            try
            {
                if (ptr != IntPtr.Zero)
                {
                    str = Marshal.PtrToStringAnsi(ptr);
                }
            }
            catch
            {
                // ignored
            }

            return str;
        }
    }

    /// <summary>
    /// 连接状态扩展
    /// </summary>
    internal static class ConnectionStateExtensions
    {
        // 提升性能的操作, 不要奇怪
        private static readonly List<TcpConnectionState> ConnectionStateValues = new List<TcpConnectionState>();
        private static readonly List<IntPtr> ConnectionStateIntPtrValues = new List<IntPtr>();

        static ConnectionStateExtensions()
        {
            var arr = Enum.GetValues(typeof(TcpConnectionState));
            foreach (var item in arr)
            {
                ConnectionStateValues.Add((TcpConnectionState)item);
                ConnectionStateIntPtrValues.Add((IntPtr)(int)item);
            }
        }

        /// <summary>
        /// 是否有效的状态值
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal static bool InvalidValue(this TcpConnectionState state)
        {
#if NET20 || NET30 || NET35
            foreach (var item in ConnectionStateValues)
            {
                if (item == state)
                {
                    return true;
                }
            }

            return false;
#else
            return ConnectionStateValues.Any(p => p == state);
#endif

        }

        /// <summary>
        /// 是否有效的状态值
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal static bool InvalidValue(IntPtr state)
        {
#if NET20 || NET30 || NET35
            foreach (var item in ConnectionStateIntPtrValues)
            {
                if (item == state)
                {
                    return true;
                }
            }

            return false;
#else
            return ConnectionStateIntPtrValues.Any(p => p == state);
#endif
        }
    }

    /// <summary>
    /// bytes扩展
    /// </summary>
    public static class BytesExtensions
    {
        /// <summary>
        /// 查找bytes
        /// <remarks>Boyer-Moore-Horspool 搜索, 来自 https://blog.csdn.net/lindexi_gd/article/details/100174714 </remarks>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<int> IndexesOf(byte[] source, int start, byte[] pattern)
        {
            var valueLength = source.Length;
            var patternLength = pattern.Length;

            if ((valueLength == 0) || (patternLength == 0) || (patternLength > valueLength))
            {
                yield break;
            }

            var badCharacters = new int[256];

            for (var i = 0; i < 256; i++)
            {
                badCharacters[i] = patternLength;
            }

            var lastPatternByte = patternLength - 1;

            for (var i = 0; i < lastPatternByte; i++)
            {
                badCharacters[pattern[i]] = lastPatternByte - i;
            }

            var index = start;

            while (index <= valueLength - patternLength)
            {
                for (var i = lastPatternByte; source[index + i] == pattern[i]; i--)
                {
                    if (i != 0) continue;
                    yield return index;
                    break;
                }

                index += badCharacters[source[index + lastPatternByte]];
            }
        }
    }

    /// <summary>
    /// Boyer-Moore
    /// <remarks>BoyerMoore类来自 https://gist.github.com/mjs3339/0772431281093f1bca1fce2f2eca527d</remarks>
    /// </summary>
    public class BoyerMoore
    {
        private int[] _jumpTable;
        private byte[] _pattern;
        public int PatternLength { get; private set; }
        public BoyerMoore()
        {
        }
        public BoyerMoore(byte[] pattern)
        {
            _pattern = pattern;
            _jumpTable = new int[256];
            PatternLength = _pattern.Length;
            for (var index = 0; index < 256; index++)
                _jumpTable[index] = PatternLength;
            for (var index = 0; index < PatternLength - 1; index++)
                _jumpTable[_pattern[index]] = PatternLength - index - 1;
        }
        public void SetPattern(byte[] pattern)
        {
            _pattern = pattern;
            _jumpTable = new int[256];
            PatternLength = _pattern.Length;
            for (var index = 0; index < 256; index++)
                _jumpTable[index] = PatternLength;
            for (var index = 0; index < PatternLength - 1; index++)
                _jumpTable[_pattern[index]] = PatternLength - index - 1;
        }
        public unsafe int Search(byte[] searchArray, int startIndex = 0)
        {
            if (_pattern == null)
                throw new Exception("Pattern has not been set.");
            if (PatternLength > searchArray.Length)
                throw new Exception("Search Pattern length exceeds search array length.");
            var index = 0;
            var limit = searchArray.Length - PatternLength;
            var patternLengthMinusOne = PatternLength - 1;
            fixed (byte* pointerToByteArray = searchArray)
            {
                var pointerToByteArrayStartingIndex = pointerToByteArray + startIndex;
                fixed (byte* pointerToPattern = _pattern)
                {
                    while (index <= limit)
                    {
                        var j = patternLengthMinusOne;
                        while (j >= 0 && pointerToPattern[j] == pointerToByteArrayStartingIndex[index + j])
                            j--;
                        if (j < 0)
                            return index;
                        index += Math.Max(_jumpTable[pointerToByteArrayStartingIndex[index + j]] - PatternLength + 1 + j, 1);
                    }
                }
            }
            return -1;
        }
        public unsafe List<int> SearchAll(byte[] searchArray, int startIndex = 0)
        {
            var index = 0;
            var limit = searchArray.Length - PatternLength;
            var patternLengthMinusOne = PatternLength - 1;
            var list = new List<int>();
            fixed (byte* pointerToByteArray = searchArray)
            {
                var pointerToByteArrayStartingIndex = pointerToByteArray + startIndex;
                fixed (byte* pointerToPattern = _pattern)
                {
                    while (index <= limit)
                    {
                        var j = patternLengthMinusOne;
                        while (j >= 0 && pointerToPattern[j] == pointerToByteArrayStartingIndex[index + j])
                            j--;
                        if (j < 0)
                            list.Add(index);
                        index += Math.Max(_jumpTable[pointerToByteArrayStartingIndex[index + j]] - PatternLength + 1 + j, 1);
                    }
                }
            }
            return list;
        }
        public int SuperSearch(byte[] searchArray, int nth, int start = 0)
        {
            var e = start;
            var c = 0;
            do
            {
                e = Search(searchArray, e);
                if (e == -1)
                    return -1;
                c++;
                e++;
            } while (c < nth);
            return e - 1;
        }
    }
    #endregion

    #region logo
    public static class Logo
    {
        public static void Show()
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string logo;
#if !NET20 && !NET30 && !NET35 && !NET40 && !NET45 && !NET451 && !NET452 && !NET46 && !NET461 && !NET462 && !NET47
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                logo = @"
██╗  ██╗███████  ███████╗ ██████╗  ██████╗██╗  ██╗███████╗████████╗   ███╗   ██╗███████╗████████╗
██║  ██║██╔══██╗ ██╔════╝██╔═══██╗██╔════╝██║ ██╔╝██╔════╝╚══██╔══╝   ████╗  ██║██╔════╝╚══██╔══╝
███████║███████║ ███████╗██║   ██║██║     █████╔╝ █████╗     ██║      ██╔██╗ ██║█████╗     ██║   
██╔══██║██╔════╝ ╚════██║██║   ██║██║     ██╔═██╗ ██╔══╝     ██║      ██║╚██╗██║██╔══╝     ██║   
██║  ██║██║      ███████║╚██████╔╝╚██████╗██║  ██╗███████╗   ██║   ██╗██║ ╚████║███████╗   ██║   
╚═╝  ╚═╝╚═╝      ╚══════╝ ╚═════╝  ╚═════╝╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝   
";
            }
            else
            {
                logo = @"
          _______  _______  _______  _______  _        _______ _________    _        _______ _________
|\     /|(  ____ )(  ____ \(  ___  )(  ____ \| \    /\(  ____ \\__   __/   ( (    /|(  ____ \\__   __/
| )   ( || (    )|| (    \/| (   ) || (    \/|  \  / /| (    \/   ) (      |  \  ( || (    \/   ) (   
| (___) || (____)|| (_____ | |   | || |      |  (_/ / | (__       | |      |   \ | || (__       | |   
|  ___  ||  _____)(_____  )| |   | || |      |   _ (  |  __)      | |      | (\ \) ||  __)      | |   
| (   ) || (            ) || |   | || |      |  ( \ \ | (         | |      | | \   || (         | |   
| )   ( || )      /\____) || (___) || (____/\|  /  \ \| (____/\   | |    _ | )  \  || (____/\   | |   
|/     \||/       \_______)(_______)(_______/|_/    \/(_______/   )_(   (_)|/    )_)(_______/   )_(   
";
            }
#else
            logo = @"
 ____  ____  _______    ______                  __             _      ____  _____        _    
|_   ||   _||_   __ \ .' ____ \                [  |  _        / |_   |_   \|_   _|      / |_  
  | |__| |    | |__) || (___ \_|  .--.   .---.  | | / ] .---.`| |-'    |   \ | |  .---.`| |-' 
  |  __  |    |  ___/  _.____`. / .'`\ \/ /'`\] | '' < / /__\\| |      | |\ \| | / /__\\| |   
 _| |  | |_  _| |_    | \____) || \__. || \__.  | |`\ \| \__.,| |, _  _| |_\   |_| \__.,| |,  
|____||____||_____|    \______.' '.__.' '.___.'[__|  \_]'.__.'\__/(_)|_____|\____|'.__.'\__/ 
";
#endif

            Console.WriteLine(logo);

            var repositories = @"
┌─Repositories─────────────────────────────────────────────┐
│ [github] https://github.com/int2e/HPSocket.Net           │
│ [gitee]  https://gitee.com/int2e/HPSocket.Net            │
└──────────────────────────────────────────────────────────┘
";
            Console.WriteLine(repositories);
            Console.ForegroundColor = oldColor;
        }
    }
    #endregion
}
