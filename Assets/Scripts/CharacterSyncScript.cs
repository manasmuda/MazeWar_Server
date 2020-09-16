using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSyncScript : MonoBehaviour
{

    public Queue<IEnumerator> movers = new Queue<IEnumerator>();
    public int count;

    public bool isMoving = false;

    public Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movers.Count > 0)
        {
            isMoving = true;
        }
        count=movers.Count;
    }

    public void NewPlayerState(ClientState state)
    {
        AddNewMove(new Vector3(state.position[0], state.position[1], state.position[2]));
    }

    public void AddNewMove(Vector3 end)
    {
        float dist = Vector3.Distance(lastPos,end);
        if (dist > 0.1 && dist<5)
        {
            if (movers.Count == 5)
            {
                movers.Dequeue();
            }
            IEnumerator newMover = MoveOverSpeed(end);
            movers.Enqueue(newMover);
            lastPos = new Vector3(end.x,end.y,end.z);
        }
        else if (dist >= 5)
        {
            TransportPlayer(end);
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, float speed=300)
    {
        // speed should be 1 unit per second
        while (transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.2f);
        }
        if (movers.Count > 0)
        {
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
        movers = new Queue<IEnumerator>();
        transform.position = end;
        isMoving = false;
    }
}
