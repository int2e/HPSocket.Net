namespace Models
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// 客户信息
        /// </summary>
        public ClientInfo Client { get; set; }

        /// <summary>
        /// 封包
        /// </summary>
        public Packet Packet { get; set; }
    }
}
