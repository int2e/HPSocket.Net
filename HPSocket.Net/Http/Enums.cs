namespace HPSocket.Http
{

    /// <summary>
    /// http版本号
    /// </summary>
    public enum HttpVersion
    {
        /// <summary>
        /// http 1.0
        /// </summary>
        // ReSharper disable once InconsistentNaming
        v1_0 = 1,
        /// <summary>
        /// http 1.1
        /// </summary>
        // ReSharper disable once InconsistentNaming
        v1_1 = 257,
    }

    /// <summary>
    /// URL 域, HTTP 请求行中 URL 段位的域定义
    /// </summary>
    public enum HttpUrlField
    {
        Schema = 0,
        Host = 1,
        Port = 2,
        Path = 3,
        QueryString = 4,
        Fragment = 5,
        UserInfo = 6,
        /// <summary>
        /// Field Count
        /// </summary>
        Max = 7,
    }

    /// <summary>
    /// HTTP 解析结果标识, 指示 HTTP 解析器是否继续执行解析操作
    /// </summary>
    public enum HttpParseResult
    {
        /// <summary>
        /// 终止解析，断开连接
        /// </summary>
        Error = -1,
        /// <summary>
        /// 继续解析
        /// </summary>
        Ok = 0,
    }

    /// <summary>
    /// HTTP 解析结果标识, 指示 HTTP 解析器是否继续执行解析操作
    /// </summary>
    public enum HttpParseResultEx
    {
        /// <summary>
        /// 终止解析，断开连接
        /// </summary>
        Error = -1,
        /// <summary>
        /// 继续解析
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 跳过当前请求 BODY
        /// </summary>
        SkipBody = 1,
        /// <summary>
        /// 升级协议
        /// </summary>
        Upgrade = 2,
    }

    /// <summary>
    /// http upgrade type
    /// </summary>
    public enum HttpUpgradeType
    {
        /// <summary>
        /// 没有升级
        /// </summary>
        None = 0,
        /// <summary>
        /// WebSocket
        /// </summary>
        WebSocket = 1,
        /// <summary>
        /// HTTP 隧道
        /// </summary>
        HttpTunnel = 2,
        /// <summary>
        /// 未知类型
        /// </summary>
        Unknown = -1,
    }

    /// <summary>
    /// http status code
    /// </summary>
    public enum HttpStatusCode
    {
        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,

        Ok = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,

        MultipleChoices = 300,
        MovedPermanently = 301,
        MovedTemporarily = 302,
        SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        SwitchProxy = 306,
        TemporaryRedirect = 307,

        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        UnorderedCollection = 435,
        UpgradeRequired = 426,
        RetryWith = 449,

        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        BandwidthLimitExceeded = 509,
        NotExtended = 510,

        UnparseableResponseHeaders = 600,

    }

    /// <summary>
    /// HttpMethod
    /// </summary>
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Patch,
        Delete,
        Head,
        Trace,
        Options,
        Connect,
    }

}
