using System;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;

[Serializable]
public class TCPManager 
{
    private int PORT_NO;
    private String SERVER_IP;
    TcpClient tcpClient;

    public TCPManager(String ip, int port)
    {
        SERVER_IP = ip; PORT_NO = port;
        tcpClient = new TcpClient(SERVER_IP, PORT_NO);
    }

    public void SendReq(String request, out String response)
    {
        NetworkStream networkStream = tcpClient.GetStream();
        byte[] toSend = UTF8Encoding.UTF8.GetBytes(request);

        networkStream.Write(toSend, 0, toSend.Length);

        byte[] toRead = new byte[tcpClient.ReceiveBufferSize];

        int bytesRead = networkStream.Read(toRead, 0, tcpClient.ReceiveBufferSize);
        response = Encoding.UTF8.GetString(toRead, 0, bytesRead);
    }

    ~TCPManager()
    {
        tcpClient.Close();
    }
}
