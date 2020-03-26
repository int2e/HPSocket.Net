using System.Collections.Generic;

namespace HPSocket.WebSocket
{
    public class HttpSession
    {
        /// <summary>
        /// cookie list
        /// </summary>
        public List<NameValue> Cookies { get; set; }

        /// <summary>
        /// header list
        /// </summary>
        public List<NameValue> Headers { get; set; }

        /// <summary>
        /// query string
        /// </summary>
        public string QueryString { get; set; }
    }
}
