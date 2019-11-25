using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager Ins;
    public Dictionary<int, BaseControl> players;
    public int ownId;
    public Shader shader;
    private void Awake()
    {
        players = new Dictionary<int, BaseControl>();
        Ins = this;
    }

    public void CreateOthers(int id,float[] startPos)
    {
        if (!players.ContainsKey(id))
        {
           GameObject asset=Resources.Load<GameObject>("Player");
           GameObject player=Instantiate(asset);
           player.GetComponent<BaseControl>().startPos = new Vector3(startPos[0], startPos[1], startPos[2]);
           Material ma = new Material(shader);
           ma.color = Color.cyan;
           player.GetComponent<BaseControl>().ma = ma;
        }
    }

    public void RegisterController(int id, BaseControl player)
    {
        if (!players.ContainsKey(id))
        {
            players.Add(id, player);
        }
    }

    public void MovePlayers(int id, float[] pos)
    {
        if (players.ContainsKey(id))
        {
            Vector3 position = new Vector3(pos[0], pos[1], pos[2]);
            if (id == ownId)
            {
                players[id].CallMoveInstant(position);
            }
            else
            {
                players[id].CallMove(position);
            }
        }
    }
}
