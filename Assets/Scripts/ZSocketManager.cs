using System.Collections;
using System.Collections.Generic;
using NetMQ;
using TestCode;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class ZSocketManager{
    private NetMQ.Sockets.RequestSocket client;
    private string connectionString;
    BinaryFormatter bf;
	
    public ZSocketManager(string connString)
    {
        bf = new BinaryFormatter();
        connectionString = connString;
        client = new NetMQ.Sockets.RequestSocket();
        client.Connect(connectionString);
    }

    public void SendReq(GameState gameState, out string recievedMessage)
    {
        byte[] bytes = new byte[gameState.CalculateSize()];
        Google.Protobuf.CodedOutputStream codedOutputStream = new Google.Protobuf.CodedOutputStream(bytes);
        gameState.WriteTo(codedOutputStream);
        codedOutputStream.Flush();

        client.SendFrame(bytes, false);
        //recievedMessage = client.ReceiveFrameString();
        if(!client.TryReceiveFrameString(new TimeSpan(0, 0, 1), out recievedMessage))
        {
            recievedMessage = "";
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void Dispose()
    {
        client.Disconnect(connectionString);
        client.Dispose();
        NetMQ.NetMQConfig.Cleanup();
    }
}
