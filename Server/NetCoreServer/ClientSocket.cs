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

        public long lastPingTime;


        public static long aliveTime = 8;
        public ClientSocket(int id, Socket socket)
        {
            this.id = id;
            this.socket = socket;
            recvData = new byte[1024];
        }

        /// <summary>
        /// 处理心跳
        /// </summary>
        public void StartHeartCheck(Action<ClientSocket> errorHandle)
        {
            lastPingTime = NetTimer.GetTimeStamp();
            while (true)
            {
                long nowT = NetTimer.GetTimeStamp();
                if (nowT - lastPingTime > aliveTime)
                {
                    //处理客户端断线
                    Disconnect(errorHandle);
                }
            }
        }

        public void StartRecv(Action<ClientSocket> errorHandle)
        {
            try
            {
               socket.BeginReceive(recvData, 0, 1024, SocketFlags.None, ReceiveCallback, errorHandle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect(errorHandle);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
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
                    RecvMsgHandle(msg);
                }
                socket.BeginReceive(recvData, 0, 1024, SocketFlags.None, ReceiveCallback, ar.AsyncState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect((Action<ClientSocket>)ar.AsyncState);
            }
            
        }

        private void RecvMsgHandle(string msg)
        {
            switch (msg)
            {
                case "ping":
                    lastPingTime = NetTimer.GetTimeStamp();
                    Console.WriteLine("【心跳{0}】ID {1}：{2}", socket.RemoteEndPoint.ToString(), id, msg);
                    SendBytes(string.Format("pong"));
                    break;
                default:
                    Console.WriteLine("【收 到{0}】ID {1}：{2}", socket.RemoteEndPoint.ToString(), id, msg);
                    //SendBytes(string.Format("服务器反馈：收到来自ID为{0}的消息_{1}", id, msg));
                    //将服务器收到的消息缓存起来
                    MessageHandle.Ins.StoreFrameMsg(msg);
                    break;
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
