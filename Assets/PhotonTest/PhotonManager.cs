using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviour, IPhotonPeerListener
{

    private static PhotonManager Instance;

    public static PhotonPeer peer//让外界可以访问我们的PhotonPeer
    { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // Use this for initialization
    void Start()
    {
        //连接服务器端
        //通过Listender连接服务器端的响应
        //第一个参数 指定一个Licensed(监听器) ,第二个参数使用什么协议
        peer = new PhotonPeer(this, ConnectionProtocol.Udp);
        //连接 UDP的 Ip地址：端口号，Application的名字
        peer.Connect("127.0.0.1:5055", "TestServer");

    }

    // Update is called once per frame
    void Update()
    {
        peer.Service();//需要一直调用Service方法,时时处理跟服务器端的连接
    }
    

    private void OnDestroy()
    {
        //如果peer不等于空并且状态为正在连接
        if (peer != null && peer.PeerState == PeerStateValue.Connected)
        {
            peer.Disconnect();//断开连接
            peer = null;
        }
    }

    //
    public void DebugReturn(DebugLevel level, string message)
    {

    }
    //如果客户端没有发起请求，但是服务器端向客户端通知一些事情的时候就会通过OnEvent来进行响应 
    public void OnEvent(EventData eventData)
    {

    }
    //当我们在客户端向服务器端发起请求后，服务器端接受处理这个请求给客户端一个响应就会在这个方法里进行处理
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case 1:
                Debug.Log("收到了服务器响应！");
                Dictionary<byte, object> respData = operationResponse.Parameters;
                object intValue;
                object stringValue;
                respData.TryGetValue(1, out intValue);
                respData.TryGetValue(2, out stringValue);
                Debug.Log("收到服务器数据：" + intValue + "\n" + stringValue);
                break;
        }
    }
    //如果连接状态发生改变的时候就会触发这个方法。
    //连接状态有五种
    //正在连接中(PeerStateValue.Connecting)
    //已经连接上（PeerStateValue.Connected）
    //正在断开连接中( PeerStateValue.Disconnecting)
    //已经断开连接(PeerStateValue.Disconnected)
    //正在进行初始化(PeerStateValue.InitializingApplication)
    public void OnStatusChanged(StatusCode statusCode)
    {

    }
}
