using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public int planeMask;
    public float speed;
    public bool isMove;
    public Vector3 targetPos;
    public Vector3 startPos;
    public int id;
    public Material ma;
    public void Start()
    {
        transform.position = startPos;
        transform.GetComponent<MeshRenderer>().material = ma;
        ControllerManager.Ins.RegisterController(id,this);
    }

    private void Update()
    {
        if (NetRoot.Ins.isNetConnected)
        {
            if (Input.GetMouseButton(1))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.layer == planeMask)
                    {
                        if (transform.position != hit.point)
                        {
                            CallMove(hit.point);
                            Send2NetServer(hit.point);
                        }
                    }
                }
                //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 1.5f);
            }
            Move2Target();
        }
    }

    private void Send2NetServer(Vector3 point)
    {
        string msg = MessageCode.move + "," + point.x + "," + point.y + "," + point.z + "@" + id;
        NetRoot.Ins.SendMsg(msg);
    }

    public void CallMove(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        this.targetPos.y = transform.position.y;
        isMove = true;
    }

    public void CallMoveInstant(Vector3 targetPos)
    {
        transform.position = targetPos;
    }
    private void Move2Target()
    {
        if (isMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (transform.position == targetPos)
            {
                isMove = false;
            }
        }
        
    }
}
