using System.Net;

namespace HPSocket.Adapter
{
    /// <summary>
    /// 转发连接适配器
    /// </summary>
    public abstract class PortForwardingConnectAdapter
    {
        /// <summary>
        /// 获取目标IpEndPoint, 子类必须重写该方法
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public virtual IPEndPoint GetTargetIpEndPoint(ITcpPortForwarding sender)
        {
            return null;
        }
    }
}
