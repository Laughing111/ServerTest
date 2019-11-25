using NetCoreServer.NetUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class SocketHandle
{
    public string msg;
    public Socket socket;
    public SocketHandle(Socket socket)
    {
        this.socket = socket;
    }
}
public class NetMangaer
{
    private static NetMangaer ins;
    private NetMangaer() { }
    public static NetMangaer Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new NetMangaer();
            }
            return ins;
        }
    }
    private long lastPingTime;
    private long pingInterval;
    public long pingValue;
    private Socket clientSocket;
    private IPEndPoint iPEndPoint;
    private byte[] recvBuffer;
    public Action<string> receivedSucceed;
    public Action<string> sendSucceed;
    public Action connectSucceed;
    SocketHandle socketHandle;
    private bool hasConnected;
    public void Init(string serverIp, int port, long pingInterval)
    {
        iPEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
        recvBuffer = new byte[1024];
        this.pingInterval = pingInterval;
    }
    public void ConnectServer()
    {
        try
        {
            if (!hasConnected)
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //clientSocket.SetSocketOption()
                clientSocket.BeginConnect(iPEndPoint, ConnectCallBack, clientSocket);
                hasConnected = true;
               
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    private void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("服务器连接成功！");
            if (connectSucceed != null)
            {
                connectSucceed();
            }
            socket.BeginReceive(recvBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
            //连接成功后，开始心跳机制
            HeartSend();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }



    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int length = socket.EndReceive(ar);
            NetBufferReader netBufferReader = new NetBufferReader(recvBuffer);
            string msg = netBufferReader.GetString();
            Debug.Log(msg);
            if (msg == "pong")
            {
                pingValue = NetTimer.GetTimeStamp() - lastPingTime;
            }
            else
            {
                if (receivedSucceed != null)
                {
                    receivedSucceed(msg);
                }
            }
            //处理消息
            socket.BeginReceive(recvBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            DisConnect();
        }
    }

    private void HeartSend()
    {
        SendMsg("ping");
        lastPingTime = NetTimer.GetTimeStamp();
        while (true)
        {
            if (clientSocket.Connected)
            {
                if (NetTimer.GetTimeStamp() - lastPingTime >= pingInterval)
                {
                    SendMsg("ping");
                    lastPingTime = NetTimer.GetTimeStamp();
                }
            }
        }
    }

    public void SendMsg(string msg)
    {
        if (clientSocket.Connected)
        {
            try
            {
                if (socketHandle == null)
                {
                    socketHandle = new SocketHandle(clientSocket);
                    socketHandle.msg = msg;
                }
                NetBufferWriter netBufferWriter = new NetBufferWriter();
                byte[] sendBuffer = netBufferWriter.GetByte(msg);
                clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendCallBack, socketHandle);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void SendCallBack(IAsyncResult ar)
    {
        SocketHandle sendHandle = (SocketHandle)ar.AsyncState;
        sendHandle.socket.EndSend(ar);
        if (sendSucceed != null)
        {
            sendSucceed(sendHandle.msg);
        }
    }

    public void DisConnect()
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
            clientSocket.Dispose();
            clientSocket = null;
            Debug.Log("与服务器断开连接…");
            hasConnected = false;
        }

    }
}
