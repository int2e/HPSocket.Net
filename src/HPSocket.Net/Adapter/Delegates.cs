using System;

namespace HPSocket.Adapter
{
    public delegate HandleResult ParseRequestBody<in TSender, in TRequestBodyType>(TSender sender, IntPtr connId, TRequestBodyType obj);
}
