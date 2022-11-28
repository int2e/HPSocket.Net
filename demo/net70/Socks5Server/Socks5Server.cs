using System.Net;
using System.Text;

using HPSocket;
using HPSocket.Tcp;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


// doc: https://wiyi.org/socks5-protocol-in-deep.html

namespace Socks5Server
{
    public class Socks5Server
    {
        #region 私有成员

        private readonly ITcpServer _server = new TcpServer();
        private readonly ITcpAgent _agent = new TcpAgent();

        #endregion

        /// <summary>
        /// 版本
        /// </summary>
        public const byte Version = 5;

        /// <summary>
        /// 本地绑定地址
        /// </summary>
        public string Address { get; set; } = null!;

        /// <summary>
        /// 本地绑定端口
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// 是否必须授权
        /// </summary>
        public bool AuthenticationRequired { get; set; }

        /// <summary>
        /// 帐号列表
        /// </summary>
        public List<Account>? Accounts { get; set; }


        public Socks5Server()
        {
            _server.OnAccept += ServerOnAccept;
            _server.OnReceive += ServerOnReceive;
            _server.OnClose += ServerOnClose;

            _agent.OnConnect += AgentOnConnect;
            _agent.OnReceive += AgentOnReceive;
            _agent.OnClose += AgentOnClose;
        }

        #region 共有方法

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                throw new InvalidOperationException($"属性'{nameof(Address)}'不可为空");
            }
            if (Port == 0)
            {
                throw new InvalidOperationException($"属性'{nameof(Port)}'不可为0");
            }

            if (AuthenticationRequired && Accounts?.Count == 0)
            {
                throw new InvalidOperationException($"属性'{nameof(AuthenticationRequired)}'为true时, 属性'{nameof(Accounts)}'不可为空或空列表");
            }

            if (_server.HasStarted)
            {
                return false;
            }
            _server.Address = Address;
            _server.Port = Port;

            var ok = _server.Start();
            if (!ok)
            {
                return false;
            }
            ok = _agent.Start();
            if (!ok)
            {
                _server.Stop();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            return _agent.Stop() && _server.Stop();
        }

        /// <summary>
        /// 等待服务结束
        /// </summary>
        /// <returns></returns>
        public Task<bool> WaitAsync()
        {
            return _server.WaitAsync();
        }

        #endregion

        #region 私有方法

        private HandleResult ServerOnAccept(IServer sender, nint connId, nint client)
        {
            var clientInfo = new ClientInfo
            {
                State = ProtocolState.Step1,
            };
            sender.SetExtra(connId, clientInfo);
            return HandleResult.Ok;
        }

        private HandleResult ServerOnReceive(IServer sender, nint connId, byte[] data)
        {
            var clientInfo = sender.GetExtra<ClientInfo>(connId);
            if (clientInfo == null)
            {
                return HandleResult.Error;
            }

            HandleResult hr;

            switch (clientInfo.State)
            {
                case ProtocolState.Step1: // 握手,协商阶段
                {
                    /*
                    +----+----------+----------+
                    |VER | NMETHODS | METHODS  |
                    +----+----------+----------+
                    | 1  |    1     | 1 to 255 |
                    +----+----------+----------+
                    VER: 协议版本，socks5为0x05
                    NMETHODS: METHODS长度
                    METHODS: 对应NMETHODS，NMETHODS的值为多少，METHODS就有多少个字节。RFC预定义了一些值的含义，内容如下:
                    X’00’ NO AUTHENTICATION REQUIRED
                    X’01’ GSSAPI
                    X’02’ USERNAME/PASSWORD
                    X’03’ to X’7F’ IANA ASSIGNED
                    X’80’ to X’FE’ RESERVED FOR PRIVATE METHODS
                    X’FF’ NO ACCEPTABLE METHODS
                    */

                    LogInfo(connId, "请求握手/协商");

                    if (data.Length < 3)
                    {
                        LogError(connId, $"握手包长度'{data.Length}'太短了");
                        return HandleResult.Error;
                    }

                    var offset = 0;

                    var VER = data[offset];
                    if (VER != Version)
                    {
                        LogError(connId, $"握手包协议版本号'{VER}'不是'{Version}'");
                        return HandleResult.Error;
                    }

                    offset++; // VER

                    var NMETHODS = data[offset];
                    if (NMETHODS != 1)
                    {
                        LogError(connId, "握手包'NMETHODS'值错误, 目前只支持'1'");
                        return HandleResult.Error;
                    }

                    if (data.Length != 2 + NMETHODS)
                    {
                        LogError(connId, $"握手包总长度错误, 应为'{2 + NMETHODS}', 实际长度为'{data.Length}'");
                        return HandleResult.Error;
                    }

                    offset++; // NMETHODS

                    var METHODS = data[offset];
                    byte[] bytes;
                    if (METHODS == 0x00) // 客户端要求无需认证
                    {
                        if (AuthenticationRequired)// 是否需要认证
                        {
                            bytes = new byte[] { Version, 0x02 }; // 需要认证
                            clientInfo.State = ProtocolState.Step2;
                        }
                        else
                        {
                            bytes = new byte[] { Version, 0x00 }; // 无需认证
                            clientInfo.State = ProtocolState.Step3;
                        }
                    }
                    else if (METHODS == 0x02)
                    {
                        if (!AuthenticationRequired || Accounts?.Count == 0)
                        {
                            bytes = new byte[] { Version, 0x00 }; // 无需认证
                            clientInfo.State = ProtocolState.Step3;
                        }
                        else
                        {
                            bytes = new byte[] { Version, 0x02 }; // 需要认证
                            clientInfo.State = ProtocolState.Step2;
                        }
                    }
                    else
                    {
                        bytes = new byte[] { Version, 0xff };
                    }

                    LogInfo(connId, $"握手/协商{(bytes[1] == 0xff ? "失败" : "成功")}");

                    hr = ServerSend(connId, bytes);
                    break;
                }
                case ProtocolState.Step2: // 认证阶段, 子协商
                {
                    /*
                    +----+------+----------+------+----------+
                    |VER | ULEN |  UNAME   | PLEN |  PASSWD  |
                    +----+------+----------+------+----------+
                    | 1  |  1   | 1 to 255 |  1   | 1 to 255 |
                    +----+------+----------+------+----------+
                    VER: 版本，通常为0x01
                    ULEN: 用户名长度
                    UNAME: 对应用户名的字节数据
                    PLEN: 密码长度
                    PASSWD: 密码对应的数据
                    */

                    LogInfo(connId, "请求子协商/鉴权");

                    if (data.Length < 5)
                    {
                        LogError(connId, $"子协商包长度'{data.Length}'太短了");
                        return HandleResult.Error;
                    }

                    var VER = data[0];
                    if (VER != 0x01)
                    {
                        LogError(connId, $"子协商协议版本号'VER'应为'0x01', 实际为{VER}");
                        return HandleResult.Error;
                    }

                    var offset = 1; // 第一个字节
                    if (data.Length < offset + 1)
                    {
                        LogError(connId, "子协商包长度错误, 不满足'ULEN'的长度'1'");
                        return HandleResult.Error;
                    }

                    // 取帐号长度
                    var ULEN = data[offset];
                    if (ULEN == 0)
                    {
                        LogError(connId, "子协商'ULEN'的值不能是'0'");
                        return HandleResult.Error;
                    }

                    offset++; // 加上帐号ULEN
                    if (data.Length < offset + ULEN)
                    {
                        LogError(connId, $"子协商包长度错误, 不满足'UNAME'的长度'{ULEN}'");
                        return HandleResult.Error;
                    }

                    // 取帐号
                    var UNAME = Encoding.ASCII.GetString(data, offset, ULEN);
                    if (string.IsNullOrWhiteSpace(UNAME))
                    {
                        LogError(connId, "子协商包'UNAME'错误, 不能为空");
                        return HandleResult.Error;
                    }

                    offset += ULEN; // 加上帐号长度
                    if (data.Length < offset + 1)
                    {
                        LogError(connId, $"子协商包长度错误, 不满足'PLEN'的长度'{ULEN}'");
                        return HandleResult.Error;
                    }

                    // 取密码长度
                    var PLEN = data[offset];
                    if (PLEN == 0)
                    {
                        LogError(connId, "子协商'PLEN'的值不能是'0'");
                        return HandleResult.Error;
                    }

                    offset++; // 加上PLEN

                    if (data.Length < offset + PLEN)
                    {
                        LogError(connId, $"子协商包长度错误, 不满足'PASSWORD'的长度'{PLEN}'");
                        return HandleResult.Error;
                    }

                    // 取密码
                    var PASSWD = Encoding.ASCII.GetString(data, offset, PLEN);
                    if (string.IsNullOrWhiteSpace(PASSWD))
                    {
                        LogError(connId, "子协商包'PASSWORD'错误, 不能为空");
                        return HandleResult.Error;
                    }

                    var acc = Accounts?.FirstOrDefault(p => p.UserName == UNAME && p.Password == PASSWD);
                    byte[] bytes;
                    if (acc != null)
                    {
                        bytes = new byte[] { VER, 0x00 }; // 认证成功
                        clientInfo.Account = acc;
                        clientInfo.State = ProtocolState.Step3;
                    }
                    else
                    {
                        bytes = new byte[] { VER, 0xff }; // 认证失败
                    }

                    LogInfo(connId, $"子协商/鉴权{(bytes[1] == 0xff ? "失败" : "成功")}, 帐号: '{UNAME}', 密码: '{PASSWD}'");
                    hr = ServerSend(connId, bytes);
                    break;
                }
                case ProtocolState.Step3: // 请求阶段
                {
                    /*
                    +----+-----+-------+------+----------+----------+
                    |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
                    +----+-----+-------+------+----------+----------+
                    | 1  |  1  | X'00' |  1   | Variable |    2     |
                    +----+-----+-------+------+----------+----------+
                    VER 版本号，socks5的值为0x05
                    CMD
                        0x01表示CONNECT请求
                        0x02表示BIND请求
                        0x03表示UDP转发
                    RSV 保留字段，值为0x00
                    ATYP 目标地址类型，DST.ADDR的数据对应这个字段的类型。
                        0x01表示IPv4地址，DST.ADDR为4个字节
                        0x03表示域名，DST.ADDR是一个可变长度的域名
                        0x04表示IPv6地址，DST.ADDR为16个字节长度
                    DST.ADDR 一个可变长度的值
                    DST.PORT 目标端口，固定2个字节
                    */

                    LogInfo(connId, "请求连接");

                    if (data.Length < 8)
                    {
                        LogError(connId, $"请求连接包长度'{data.Length}'太短了");
                        return HandleResult.Error;
                    }

                    var bytes = new byte[10];
                    Array.Copy(data, 0, bytes, 0, 4);

                    var VER = data[0];
                    if (VER != Version)
                    {
                        LogError(connId, $"请求连接包协议版本号'{VER}'不是'{Version}'");
                        return HandleResult.Error;
                    }

                    var offset = 1; // VER

                    var CMD = data[offset];
                    if (CMD != 0x01)
                    {
                        LogError(connId, $"请求连接CMD的值不支持'{CMD}'");
                        bytes[1] = 0x07;
                        hr = ServerSend(connId, bytes);
                        break;
                    }

                    offset++; // CMD
                    offset++; // RSV 保留字段, 未使用

                    var ATYP = data[offset];
                    offset++; // ATYP
                    string targetAddr;
                    ushort targetPort;
                    if (ATYP == 0x01) // ipv4
                    {
                        if (data.Length < offset + 4)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.ADDR'的ipv4长度'4'");
                            return HandleResult.Error;
                        }

                        targetAddr = $"{data[offset]}.{data[offset + 1]}.{data[offset + 2]}.{data[offset + 3]}";

                        offset += 4; // DADDR

                        if (data.Length < offset + 2)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.PORT'的长度'2'");
                            return HandleResult.Error;
                        }

                        targetPort = BitConverter.ToUInt16(data, offset);
                    }
                    else if (ATYP == 0x03) // DOMAIN_NAME
                    {
                        if (data.Length < offset + 2)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.ADDR'的DNLEN长度'2'");
                            return HandleResult.Error;
                        }

                        var DNLEN = data[offset];

                        offset++; // DNLEN

                        if (data.Length < offset + DNLEN)
                        {
                            LogError(connId, $"请求连接包长度错误, 不满足'DST.ADDR'的DNAME长度'{DNLEN}'");
                            return HandleResult.Error;
                        }

                        targetAddr = Encoding.ASCII.GetString(data, offset, DNLEN);

                        offset += DNLEN; // DADDR

                        if (data.Length < offset + 2)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.PORT'的长度'2'");
                            return HandleResult.Error;
                        }

                        targetPort = BitConverter.ToUInt16(data, offset);
                    }
                    else if (ATYP == 0x04) // ipv6
                    {
                        if (data.Length < offset + 16)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.ADDR'的ipv6长度'16'");
                            return HandleResult.Error;
                        }

                        targetAddr = Encoding.ASCII.GetString(data, offset, 16);

                        offset += 16; // DADDR

                        if (data.Length < offset + 2)
                        {
                            LogError(connId, "请求连接包长度错误, 不满足'DST.PORT'的长度'2'");
                            return HandleResult.Error;
                        }

                        targetPort = BitConverter.ToUInt16(data, offset);
                    }
                    else
                    {
                        LogError(connId, $"请求连接包ATYP的值不支持'{ATYP}'");
                        bytes[1] = 0x08;
                        hr = ServerSend(connId, bytes);
                        break;
                    }

                    targetPort = (ushort)IPAddress.NetworkToHostOrder((short)targetPort);
                    LogInfo(connId, $"请求连接目标服务器: '{targetAddr}:{targetPort}'");

                    clientInfo.ExtraData = bytes;
                    clientInfo.ServerConnId = connId;

                    if (!_agent.Connect(targetAddr, targetPort, connId))
                    {
                        LogError(connId, $"请求连接失败, 网络无法访问'0x03'");
                        bytes[1] = 0x03;
                        hr = ServerSend(connId, bytes);
                    }
                    else
                    {
                        hr = HandleResult.Ok;
                    }

                    break;
                }
                case ProtocolState.Step4: // Relay阶段
                {
                    LogInfo(connId, $"Server转发数据包, 长度: '{data.Length}'");
                    hr = AgentSend(clientInfo.AgentConnId, data);
                    break;
                }
                default:
                {
                    hr = HandleResult.Error;
                    LogError("未知状态");
                    break;
                }
            }

            return hr;

        }
        private HandleResult ServerOnClose(IServer sender, nint connId, SocketOperation socketOperation, int errorCode)
        {
            sender.RemoveExtra(connId);
            LogInfo(connId, $"Sever连接已关闭, socketOperation: '{socketOperation}', errorCode: '{errorCode}'");
            return HandleResult.Ok;
        }


        private HandleResult AgentOnConnect(IAgent sender, nint connId, IProxy proxy)
        {
            if (!sender.GetConnectionExtra(connId, out var serverConnId))
            {
                return HandleResult.Error;
            }

            var clientInfo = _server.GetExtra<ClientInfo>(serverConnId);
            if (clientInfo == null)
            {
                return HandleResult.Error;
            }

            if (clientInfo.ExtraData is not byte[] bytes)
            {
                return HandleResult.Error;
            }

            LogInfo(connId, "目标服务器连接成功");

            clientInfo.AgentConnId = connId;
            clientInfo.ExtraData = null;
            clientInfo.State = ProtocolState.Step4;
            sender.SetExtra(connId, clientInfo);

            bytes[1] = 0x00;
            return ServerSend(clientInfo.ServerConnId, bytes);
        }

        private HandleResult AgentOnReceive(IAgent sender, nint connId, byte[] data)
        {
            var clientInfo = sender.GetExtra<ClientInfo>(connId);
            if (clientInfo == null)
            {
                return HandleResult.Error;
            }

            // LogInfo(connId, $"Agent转发数据包, 长度: '{data.Length}'");
            // Console.WriteLine("============================================================");
            // LogInfo(connId, Encoding.UTF8.GetString(data));
            // Console.WriteLine("============================================================");
            return ServerSend(clientInfo.ServerConnId, data);
        }

        private HandleResult AgentOnClose(IAgent sender, nint connId, SocketOperation socketOperation, int errorCode)
        {
            var clientInfo = sender.GetExtra<ClientInfo>(connId);
            if (clientInfo == null)
            {
                return HandleResult.Error;
            }

            sender.RemoveExtra(connId);

            if (clientInfo.State == ProtocolState.Step3)
            {
                if (clientInfo.ExtraData is not byte[] bytes)
                {
                    return HandleResult.Error;
                }

                bytes[1] = socketOperation == SocketOperation.TimedOut
                    ? (byte)0x04 // X’04’ Host unreachable
                    : (byte)0x05; // X’05’ Connection refused

                return ServerSend(clientInfo.ServerConnId, bytes);
            }

            LogInfo(connId, $"Agent连接已关闭, socketOperation: '{socketOperation}', errorCode: '{errorCode}'");

            return HandleResult.Ok;
        }

        private HandleResult ServerSend(nint connId, byte[] data)
        {
            return _server.Send(connId, data, data.Length) ? HandleResult.Ok : HandleResult.Error;
        }

        private HandleResult AgentSend(nint connId, byte[] data)
        {
            return _agent.Send(connId, data, data.Length) ? HandleResult.Ok : HandleResult.Error;
        }

        private void LogError(nint id, string log)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <ERR> id: '{id}', {log}");
        }
        private void LogError(string log)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <ERR> {log}");
        }

        private void LogInfo(nint id, string log)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <INFO> id: '{id}', {log}");
        }

        private void LogInfo(string log)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <INFO> {log}");
        }

        #endregion
    }
}
