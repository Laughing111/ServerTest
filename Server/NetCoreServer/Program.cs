using System;

namespace NetCoreServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(8899);
            server.Start();
        }
    }
}
