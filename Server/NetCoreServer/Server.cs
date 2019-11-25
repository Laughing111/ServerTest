using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using NetCoreServer.NetUtils;
using System.Threading;

namespace NetCoreServer
{
    class Server
    {
        private Socket serverSocket;
        private IPEndPoint ipEndPoint;
        private static List<ClientSocket> clientSocketPool;

        private long frameInterval;
        private long lastFrame;
        public Server(int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(99);
            clientSocketPool = new List<ClientSocket>();
            frameInterval = 1;
        }

        public void Start()
        {
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
            Console.WriteLine("服务器启动成功…");
            //lastFrame = NetTimer.GetTimeStamp();
            startBroadFrame();
        }

        private void startBroadFrame()
        {
            while (true)
            {
                //Console.WriteLine(NetTimer.GetTimeStamp());
                //距离上次广播满足一帧的间隔
                long nowT = NetTimer.GetTimeStamp();
                if (nowT - lastFrame >= frameInterval)
                {
                    //重新广播
                    Console.WriteLine("开始广播关键帧");
                    MessageHandle.Ins.BroadFrameMsg();
                    lastFrame = NetTimer.GetTimeStamp();
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socketSer = (Socket)ar.AsyncState;
            ClientSocket client = null;
            socketSer.BeginAccept(AcceptCallback, socketSer);
            try
            {
                Socket socketClient = socketSer.EndAccept(ar);
                if (socketClient != null)
                {
                    client = new ClientSocket(clientSocketPool.Count + 1,socketClient);
                    clientSocketPool.Add(client);
                    Console.WriteLine("连接上id为{0}客户端:{1}",client.id.ToString(), client.socket.RemoteEndPoint.ToString());
                    client.SendBytes("this is Server!");
                    //处理客户端Socket
                    client.StartRecv(RemoveClient);
                    client.StartHeartCheck(RemoveClient);
                }
            }
            catch(Exception e)
            {
                if(client != null&&client.socket!=null)
                {
                    client.Disconnect();
                    RemoveClient(client);
                }
            }
            
        }

        public static void RemoveClient(ClientSocket client)
        {
            if (clientSocketPool.Contains(client))
            {
                clientSocketPool.Remove(client);
                Console.WriteLine("编号为{0}的客户端已断开...",client.id);
            }
        }

        public static void BroadMsg(string msg)
        {
            if (clientSocketPool.Count > 0)
            {
                int clientCount = clientSocketPool.Count;
                if (clientCount > 0)
                {
                    for (int i = 0; i < clientCount; i++)
                    {
                        clientSocketPool[i].SendBytes(msg, RemoveClient);
                    }
                }
            }
        }

        private void DisposeServices()
        {
            if (serverSocket != null)
            {
                serverSocket.Close();
                serverSocket.Dispose();
                serverSocket = null;
            }
        }
    }
}
