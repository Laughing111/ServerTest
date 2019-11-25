
using ExitGames.Logging;
using Photon.SocketServer;
using System.IO;

namespace TestServer
{
    class ServerApp : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            log.Info("有客户端连接！");
            return new Client(initRequest);
        }

        protected override void Setup()
        {
            LogInit();
        }

        protected override void TearDown()
        {
            
        }

        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private void LogInit()
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(ExitGames.Logging.Log4Net.Log4NetLoggerFactory.Instance);//设置photon我们使用哪个日志插件
                log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);//让log4net这个插件读取配置文件
            }

            log.Info("Setup Completed!");//最后利用log对象就可以输出了
        }

    }
}
