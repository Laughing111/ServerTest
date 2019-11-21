using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using NetCoreServer.NetUtils;

namespace NetCoreServer
{
    class Server
    {
        private Socket serverSocket;
        private IPEndPoint ipEndPoint;
        private List<ClientSocket> clientSocketPool;
        public Server(int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(99);
            clientSocketPool = new List<ClientSocket>();
        }

        public void Start()
        {
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
            Console.WriteLine("服务器启动成功…");
            Console.ReadLine();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socketSer = (Socket)ar.AsyncState;
            ClientSocket client = null;
            try
            {
                Socket socketClient = socketSer.EndAccept(ar);
                if (socketClient != null)
                {
                    client = new ClientSocket(clientSocketPool.Count + 1,socketClient);
                    clientSocketPool.Add(client);
                    Console.WriteLine("连接上id为{0}客户端:{1}", client.id.ToString(), client.socket.RemoteEndPoint.ToString());
                    client.SendBytes("this is Server!");
                    //处理客户端Socket
                    client.StartRecv(RemoveClient);
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
            socketSer.BeginAccept(AcceptCallback, socketSer);
        }

        public void RemoveClient(ClientSocket client)
        {
            if (clientSocketPool.Contains(client))
            {
                clientSocketPool.Remove(client);
                Console.WriteLine("编号为{0}的客户端已断开...",client.id);
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
