using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSyncScript : MonoBehaviour
{

    public Queue<IEnumerator> movers = new Queue<IEnumerator>();

    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewPlayerState(ClientState state)
    {

    }

    public void AddNewMove(Vector3 end)
    {
        if (movers.Count > 10)
        {
            TransportPlayer(end);
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, float speed=500)
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
            isMoving = false;
        }
    }

    public void TransportPlayer(Vector3 end)
    {
        movers = new Queue<IEnumerator>();
        transform.position = end;
    }
}
