using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetTest : MonoBehaviour
{
    public string serverIp="127.0.0.1";
    public int port = 8899;
    private NetMangaer netMangaer;
    private Queue<string> msgQueue;
    public Text text;
    void Start()
    {
        netMangaer = NetMangaer.Ins;
        netMangaer.Init(serverIp, port,5);

        netMangaer.receivedSucceed += PushRecvMsg;
        netMangaer.sendSucceed += PushSendMsg;
    }
     void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            netMangaer.ConnectServer();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            netMangaer.SendMsg("this is unity client");
        }

        if (msgQueue != null&&msgQueue.Count>0)
        {
            string str = msgQueue.Dequeue();
            text.text += str + "\n";
        }
    }

    private void PushRecvMsg(string msg)
    {
        if (msgQueue == null)
        {
            msgQueue = new Queue<string>();
        }
        msgQueue.Enqueue("【收到】"+msg);
    }

    private void PushSendMsg(string msg)
    {
        if (msgQueue == null)
        {
            msgQueue = new Queue<string>();
        }
        msgQueue.Enqueue("【已发送】" + msg);
    }

    public void OnDisable()
    {
        netMangaer.DisConnect();
    }
}
