using System;

namespace HPSocket
{
    public delegate HandleResult ServerSendEventHandler(IServer sender, IntPtr connId, byte[] data);
    public delegate HandleResult ServerReceiveEventHandler(IServer sender, IntPtr connId, byte[] data);
    public delegate HandleResult ServerCloseEventHandler(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode);
    public delegate HandleResult ServerShutdownEventHandler(IServer sender);
    public delegate HandleResult ServerPrepareListenEventHandler(IServer sender, IntPtr listen);
    /// <summary>
    /// 连接进入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="connId"></param>
    /// <param name="client">如果为 TCP 连接，pClient为 SOCKET 句柄；如果为 UDP 连接，pClient为 SOCKADDR 指针；</param>
    /// <returns></returns>
    public delegate HandleResult ServerAcceptEventHandler(IServer sender, IntPtr connId, IntPtr client);
    public delegate HandleResult ServerHandShakeEventHandler(IServer sender, IntPtr connId);
    public delegate HandleResult PullServerReceiveEventHandler(IServer sender, IntPtr connId, int length);




    public delegate HandleResult AgentPrepareConnectEventHandler(IAgent sender, IntPtr connId, IntPtr socket);
    public delegate HandleResult AgentConnectEventHandler(IAgent sender, IntPtr connId, IProxy proxy);
    public delegate HandleResult AgentSendEventHandler(IAgent sender, IntPtr connId, byte[] data);
    public delegate HandleResult AgentReceiveEventHandler(IAgent sender, IntPtr connId, byte[] data);
    public delegate HandleResult AgentCloseEventHandler(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode);
    public delegate HandleResult AgentShutdownEventHandler(IAgent sender);
    public delegate HandleResult AgentHandShakeEventHandler(IAgent sender, IntPtr connId);
    public delegate HandleResult PullAgentReceiveEventHandler(IAgent sender, IntPtr connId, int length);




    public delegate HandleResult ClientPrepareConnectEventHandler(IClient sender, IntPtr socket);
    public delegate HandleResult ClientConnectEventHandler(IClient sender);
    public delegate HandleResult ClientSendEventHandler(IClient sender, byte[] data);
    public delegate HandleResult ClientReceiveEventHandler(IClient sender, byte[] data);
    public delegate HandleResult ClientCloseEventHandler(IClient sender, SocketOperation socketOperation, int errorCode);
    public delegate HandleResult ClientHandShakeEventHandler(IClient sender);
    public delegate HandleResult PullClientReceiveEventHandler(IClient sender, int length);




    public delegate HandleResult UdpNodePrepareListenEventHandler(IUdpNode sender, IntPtr soListen);
    public delegate HandleResult UdpNodeSendEventHandler(IUdpNode sender, string remoteAddress, ushort remotePort, byte[] data);
    public delegate HandleResult UdpNodeReceiveEventHandler(IUdpNode sender, string remoteAddress, ushort remotePort, byte[] pData );
    public delegate HandleResult UdpNodeErrorEventHandler(IUdpNode sender, SocketOperation socketOperation, int errorCode, string remoteAddress, ushort remotePort, byte[] data);
    public delegate HandleResult UdpNodeShutdownEventHandler(IUdpNode sender);



    /// <summary>
    /// 代理预连接
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public delegate void ProxyPrepareConnectEventHandler(IAgent sender, IProxy proxy);
    /// <summary>
    /// 代理已连接
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="connId"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public delegate void ProxyConnectedEventHandler(IAgent sender, IntPtr connId, IProxy proxy);
}
