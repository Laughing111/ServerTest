using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransport : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendRequest();
        }
    }

    void SendRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add(1, 100);
        data.Add(2, "server，this is client!");
        PhotonManager.peer.OpCustom(1, data, true);

    }
}
