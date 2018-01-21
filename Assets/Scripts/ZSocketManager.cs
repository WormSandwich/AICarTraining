using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using ZMQ;
using System.Text;

[Serializable]
public class ZSocketManager
{
    private Context ctx;
    private Socket socket;

    public ZSocketManager(String address)
    {
        ctx = new Context();
        socket = ctx.Socket(SocketType.REQ);
        socket.Bind(address);
    }

    public void SendRequest(string request, out string response)
    {
        socket.Send(request, Encoding.UTF8);
        byte[] reply = socket.Recv();
        response = reply.ToString();
    }

    ~ZSocketManager()
    {
        socket.Dispose();
        ctx.Dispose();
    }
}
