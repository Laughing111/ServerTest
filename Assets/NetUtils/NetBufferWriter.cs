using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreServer.NetUtils
{
    class NetBufferWriter
    {
        private MemoryStream memoryStream;
        private BinaryWriter binaryWriter;
        private byte[] data;
        public NetBufferWriter()
        {
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
        }

        public byte[] GetByte(string msg)
        {
            data = Encoding.UTF8.GetBytes(msg);
            byte[] msgLength = BitConverter.GetBytes((ushort)data.Length);
            binaryWriter.Write((ushort)data.Length);
            binaryWriter.Write(data);
            binaryWriter.Flush();
            return memoryStream.ToArray();
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
