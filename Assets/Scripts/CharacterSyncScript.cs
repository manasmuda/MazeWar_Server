using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSyncScript : MonoBehaviour
{

    public Queue<IEnumerator> movers = new Queue<IEnumerator>();
    public int count;

    public bool isMoving = false;

    public Vector3 lastPos;
    public float lastY;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewPlayerState(ClientState state)
    {
        AddNewMove(new Vector3(state.position[0], state.position[1], state.position[2]), new Vector3(state.angle[0], state.angle[1], state.angle[2]));
    }

    public void AddNewMove(Vector3 end,Vector3 fang)
    {
        float dist = Vector3.Distance(lastPos,end);
        float dist1 = Vector3.Distance(lastPos, transform.position);
           if ((dist > 0.1 && dist < 5) || Math.Abs(lastY-fang.y)>10)
            {
                Debug.Log("New Move Added");
                /*if (movers.Count == 10)
                {
                    movers.Dequeue();
                }*/
                IEnumerator newMover = MoveOverSpeed(end,fang);
                movers.Enqueue(newMover);
                lastPos = new Vector3(end.x, end.y, end.z);
                lastY = fang.y;
                if (!isMoving && movers.Count > 0)
                {
                    StartCoroutine(movers.Dequeue());
                }
                Debug.Log("Last Pos Set");
            }
            else if (dist >= 5)
            {
                TransportPlayer(end);
            }
    }

    public void AddNewAngle(Quaternion angle)
    {

    }

    public IEnumerator MoveOverSpeed(Vector3 end,Vector3 fang, float speed=200)
    {
        // speed should be 1 unit per second
        while (transform.position != end)
        {
            Debug.Log(transform.position.z+","+end.z);
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Quaternion q = new Quaternion();
        q.eulerAngles = fang;
        transform.rotation = q;
        if (movers.Count > 0)
        {
            Debug.Log("Next Move Enqued");
            StartCoroutine(movers.Dequeue());
        }
        else
        {
            Debug.Log("stopped");
            isMoving = false;
        }
    }

    public void TransportPlayer(Vector3 end)
    {
        Debug.Log("Transported");
        movers.Clear();
        transform.position = end;
        isMoving = false;
        lastPos = end;
    }
}
