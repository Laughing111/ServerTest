using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreServer.NetUtils
{
    public class MessageHandle
    {
        private static MessageHandle ins;
        public static MessageHandle Ins
        {
            get
            {
                return new MessageHandle();
            }
        }
        private MessageHandle() { }

        public Queue<string> frameMsgPool = new Queue<string>();

        public void StoreFrameMsg(string frameMsg)
        {
            frameMsgPool.Enqueue(frameMsg);
            Console.WriteLine("存储消息,队列长度{0}",frameMsgPool.Count);
        }

        public void BroadFrameMsg()
        {
            while (frameMsgPool.Count > 0)
            {
                Console.WriteLine("真正广播关键帧");
                Server.BroadMsg(frameMsgPool.Dequeue());
            }
        }
    }
}
