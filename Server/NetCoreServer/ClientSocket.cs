using NetCoreServer.NetUtils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetCoreServer
{
    
    class ClientSocket
    {

        public byte[] recvData;
        public object sendObject { get; private set; }
        public Socket socket;
        public int id { get; private set; }
        public ClientSocket(int id, Socket socket)
        {
            this.id = id;
            this.socket = socket;
            recvData = new byte[1024];
        }

        public void StartRecv(Action<ClientSocket> errorHandle)
        {
            try
            {
                if (!socket.Poll(10, SelectMode.SelectRead))
                {
                    socket.BeginReceive(recvData, 0, 1024, SocketFlags.None, ReceiveCallback, errorHandle);
                }
                else
                {
                    Disconnect();
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect(errorHandle);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int length = socket.EndReceive(ar);
            if (length <= 0)
            {
                //说明客户端断开连接
                Disconnect((Action<ClientSocket>)ar.AsyncState);
                return;
            }
            else
            {
                //todo 封装消息队列  显示方法
                //string msg= Encoding.UTF8.GetString(data);
                NetBufferReader netBufferReader = new NetBufferReader(recvData);
                string msg = netBufferReader.GetString();
                Console.WriteLine("【收 到{0}】ID {1}：{2}", socket.RemoteEndPoint.ToString(),id, msg);
                SendBytes(string.Format("服务器反馈：收到来自ID为{0}的消息_{1}", id, msg));
            }
            if (!socket.Poll(10, SelectMode.SelectRead))
            {
                socket.BeginReceive(recvData, 0, 1024, SocketFlags.None, ReceiveCallback, ar.AsyncState);
            }
            else
            {
                Disconnect((Action<ClientSocket>)ar.AsyncState);
            }
                
        }

        public void SendBytes(string value, Action<ClientSocket> errorHandle =null)
        {
            NetBufferWriter writer = new NetBufferWriter(value);
            byte[] sendData = writer.GetByte();
            try
            {
                socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallBack, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //断开连接
                Disconnect(errorHandle);
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
                string msg = (string)ar.AsyncState;
                socket.EndSend(ar);
                Console.WriteLine("【已发送{0}】ID {1}：{2}", socket.RemoteEndPoint.ToString(),id,msg);
        }

        public void SendBytes(object value)
        {
            sendObject = value;
        }

        public void Disconnect(Action<ClientSocket> disconnectHandle=null)
        {
            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
                socket = null;
            }
            if (disconnectHandle != null)
            {
                disconnectHandle(this);
            }
        }

    }
}
