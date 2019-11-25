using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;

namespace TestServer
{
    internal class Client : ClientPeer
    {
        public Client(InitRequest initRequest) : base(initRequest)
        {
            EventData ed = new EventData(1);
            Dictionary<byte, object> eventData = new Dictionary<byte, object>();
            eventData.Add(1, 300);
            eventData.Add(2, "客户端连接已被接受！"+RemoteIPAddress);
            ed.Parameters = eventData;
            SendEvent(ed,new SendParameters());
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case 1:
                    ServerApp.log.Info("收到一个客户端请求");
                    //收到客户端的数据，并打印到日志中
                    Dictionary<byte, object> recvData = operationRequest.Parameters;
                    object intValue;
                    recvData.TryGetValue(1, out intValue);
                    object stringValue;
                    recvData.TryGetValue(2, out stringValue);
                    ServerApp.log.Info("收到客户端请求:"+intValue+"\n"+stringValue);

                    //回复客户端
                    OperationResponse response = new OperationResponse(1);
                    Dictionary<byte, object> respData = new Dictionary<byte, object>();
                    respData.Add(1, 200);
                    respData.Add(2, "hi! Client! This is Server!");
                    response.SetParameters(respData);

                    SendOperationResponse(response, sendParameters);
                    break;
            }


        }
    }
}
