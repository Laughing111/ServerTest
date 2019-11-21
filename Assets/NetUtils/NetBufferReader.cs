using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreServer.NetUtils
{
    class NetBufferReader
    {
        private ushort msgLength;
        private MemoryStream memoryStream;
        private BinaryReader binaryReader;
        public NetBufferReader(byte[] buffer)
        {
            memoryStream = new MemoryStream(buffer);
            binaryReader = new BinaryReader(memoryStream);
            msgLength = GetUshrot();
        }

        public string GetString()
        {
            if (msgLength > 0)
            {
                //byte[] msgBytes = new byte[msgLength];
                byte[] msgBytes=binaryReader.ReadBytes(msgLength);
                return Encoding.UTF8.GetString(msgBytes);
            }
            else
            {
                return null;
            }
        }

        public ushort GetUshrot()
        {
            return binaryReader.ReadUInt16();
        }

        public void Dispose()
        {
            if (memoryStream != null)
            {
                memoryStream.Dispose();
                memoryStream.Close();
                memoryStream = null;
            }
        }
    }
}
