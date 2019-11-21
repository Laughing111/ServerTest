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
        public NetBufferWriter(string msg)
        {
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
            data = Encoding.UTF8.GetBytes(msg);
            byte[] msgLength = BitConverter.GetBytes((ushort)data.Length);
            binaryWriter.Write((ushort)data.Length);
            binaryWriter.Write(data);
            binaryWriter.Flush();
        }

        public byte[] GetByte()
        {
            byte[] result = memoryStream.ToArray();
            Dispose();
            return result;
        }

       public void Dispose()
        {
            if (binaryWriter != null)
            {
                binaryWriter.Dispose();
                binaryWriter.Close();
                binaryWriter = null;
            }
            if (memoryStream != null)
            {
                memoryStream.Dispose();
                memoryStream.Close();
                memoryStream = null;
            }
        }
    }
}
