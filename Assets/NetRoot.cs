using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageCode
{
    none,
    jump,
    move,
    create,
}
public class NetRoot : MonoBehaviour
{
    private MessageCode messageCode=MessageCode.none;
    public Text pingValue;
    public string serverIp = "127.0.0.1";
    public int port = 8899;
    private NetMangaer netMangaer;
    public bool isNetConnected;
    public static NetRoot Ins;
    // Start is called before the first frame update
    private void Awake()
    {
        Ins = this;
    }
    void Start()
    {
        netMangaer = NetMangaer.Ins;
        netMangaer.Init(serverIp, port, 5);
        netMangaer.connectSucceed += ConnectCallBack;
        netMangaer.receivedSucceed += MessageHandle;
        netMangaer.ConnectServer();
    }

    private void ConnectCallBack()
    {
        int id = ControllerManager.Ins.ownId;
       Vector3 startPos = ControllerManager.Ins.players[id].startPos;
        isNetConnected = true;
        string msg = MessageCode.create + "," + startPos.x + "," + startPos.y + "," + startPos.z + "@" + id;
        netMangaer.SendMsg(msg);
    }

    private void Update()
    {
        if (isNetConnected)
        {
            pingValue.text = netMangaer.pingValue.ToString() + "ms";
        }
    }

    public void SendMsg(string msg)
    {
        netMangaer.SendMsg(msg);
    }

    void MessageHandle(string msg)
    {
        string[] tempdata=msg.Split(',');
        int dataCount = tempdata.Length;
        if (dataCount >1)
        {
            if(Enum.TryParse<MessageCode>(tempdata[0],out messageCode))
            {
                switch (messageCode)
                {
                    case MessageCode.move:
                        float[] tempPos = new float[3];
                        for(int i = 1; i < dataCount; i++)
                        {
                            if (i == dataCount - 1)
                            {
                              string[] tempValue= tempdata[i].Split('@');
                               if (tempValue.Length >= 2)
                               {
                                    int id;
                                    if(!int.TryParse(tempValue[1], out id))
                                    {
                                        return;
                                    }
                                    if (!float.TryParse(tempValue[0], out tempPos[i - 1]))
                                    {
                                        tempPos[i - 1] = 0;
                                    }
                                    ControllerManager.Ins.MovePlayers(id, tempPos);
                                }
                            }
                            else
                            {
                                if(!float.TryParse(tempdata[i],out tempPos[i - 1]))
                                {
                                    tempPos[i - 1] = 0;
                                }
                            }
                        }
                        break;
                    case MessageCode.create:
                        float[] startPos = new float[3];
                        for (int i = 1; i < dataCount; i++)
                        {
                            if (i == dataCount - 1)
                            {
                                string[] tempValue = tempdata[i].Split('@');
                                if (tempValue.Length >= 2)
                                {
                                    int id;
                                    if (!int.TryParse(tempValue[1], out id))
                                    {
                                        return;
                                    }
                                    if (!float.TryParse(tempValue[0], out startPos[i - 1]))
                                    {
                                        startPos[i - 1] = 0;
                                    }
                                    ControllerManager.Ins.CreateOthers(id, startPos);
                                }
                            }
                            else
                            {
                                if (!float.TryParse(tempdata[i], out startPos[i - 1]))
                                {
                                    startPos[i - 1] = 0;
                                }
                            }
                        }
                        break;
                    case MessageCode.jump:
                        break;
                }
            }
            else
            {
                return;
            }
        }
    }

    public void OnDisable()
    {
        netMangaer.DisConnect();
    }
}
