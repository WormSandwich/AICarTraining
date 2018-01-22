using System;
using System.Net.Sockets;
using System.Text;
using TestCode;

[Serializable]
public class TCPManager 
{
    private int PORT_NO;
    private String SERVER_IP;
    private TcpClient tcpClient;
    NetworkStream networkStream;

    public TCPManager(String ip, int port)
    {
        SERVER_IP = ip; PORT_NO = port;
        tcpClient = new TcpClient(SERVER_IP, PORT_NO);
        networkStream = tcpClient.GetStream();
    }

    public void SendReq(GameState request, out String response)
    {
        
        Google.Protobuf.CodedOutputStream codedOutputStream = new Google.Protobuf.CodedOutputStream(networkStream);
        request.WriteTo(codedOutputStream);
        codedOutputStream.Flush();

        byte[] toRead = new byte[tcpClient.ReceiveBufferSize];
        int bytesRead = networkStream.Read(toRead, 0, tcpClient.ReceiveBufferSize);
        response = Encoding.UTF8.GetString(toRead, 0, bytesRead);
    }

    public void Dispose()
    {
        networkStream.Dispose();
        tcpClient.Close();
    }

    ~TCPManager()
    {
        networkStream.Dispose();
        tcpClient.Close();
    }
}
