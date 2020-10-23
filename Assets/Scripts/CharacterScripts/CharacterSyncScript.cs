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
    public Vector3 Direction;
    public float lastY;

    public Animator anim;

    public bool inCrouch = false;

    public float speed;

    public IEnumerator moveTime;
    public float mt = 0f;

    public bool ms = true;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastY = transform.rotation.eulerAngles.y;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(isMoving && movers.Count>0 &&)
        count = movers.Count;
    }

    public void NewPlayerState(ClientState state)
    {
        if (state.crouch)
        {
            if (!inCrouch)
            {
                inCrouch = true;
                anim.SetBool("isCrouching", true);
            }
        }
        else
        {
            if (inCrouch)
            {
                inCrouch = false;
                anim.SetBool("isCrouching", false);
            }
        }
        if (ms)
        {
            AddNewMove(new Vector3(state.position[0], state.position[1], state.position[2]), new Vector3(state.angle[0], state.angle[1], state.angle[2]));
        }
        else
        {
            StartCoroutine(AddNAngle(new Vector3(state.angle[0], state.angle[1], state.angle[2])));
            AddNMove(new Vector3(state.position[0], state.position[1], state.position[2]));
        }

    }

    public void AddNewMove(Vector3 end, Vector3 fang)
    {
        float dist = Vector3.Distance(lastPos, end);
        float dist1 = Vector3.Distance(lastPos, transform.position);
        if ((dist > 0.1 && dist < 5) || Math.Abs(lastY - fang.y) > 10)
        {
            //Debug.Log("New Move Added");
            IEnumerator newMover = MoveOverSpeed(end, fang);
            movers.Enqueue(newMover);
            if (movers.Count > 10)
            {
                TransportPlayer(end, fang);
                return;
            }
            lastPos = new Vector3(end.x, end.y, end.z);
            lastY = fang.y;
            if (!isMoving && movers.Count > 0)
            {
                //Debug.Log("Movement Started freshly");
                StartCoroutine(movers.Dequeue());
                isMoving = true;
            }
            //Debug.Log("Last Pos Set");
        }
        else if (dist >= 5)
        {
            TransportPlayer(end, fang);
        }
        if (dist1 > 5)
        {
            TransportPlayer(end, fang);
        }
    }

    public void AddNMove(Vector3 end)
    {
        float dist = Vector3.Distance(lastPos, end);

        if ((dist > 0.1 && dist < 5))
        {
            IEnumerator newMover = MoveOverSecond(end);
            if (moveTime != null)
            {
                StopCoroutine(moveTime);
            }
            moveTime = newMover;
            StartCoroutine(moveTime);
            mt = mt + 0.2f;
        }
        else if (dist >= 5)
        {
            TransportPlayer(end, transform.rotation.eulerAngles);
        }
    }

    IEnumerator AddNAngle(Vector3 fang)
    {
        yield return new WaitForSeconds(mt);
        if (Math.Abs(lastY - fang.y) > 10)
        {
            lastY = fang.y;
            Quaternion q = new Quaternion();
            q.eulerAngles = fang;
            transform.rotation = q;
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, Vector3 fang)
    {
        // speed should be 1 unit per second
        //Debug.Log("Move Over Speed Started");
        float time = 0.2f;
        speed = 85f;

        Direction = (end - transform.position).normalized;
        //Debug.Log("Direction is created");
        if (inCrouch)
        {
            Crouching(Direction.x, Direction.z);
        }
        else
        {
            Walking(Direction.x, Direction.z);
            if (Direction.z > 0.3)
            {
                speed = 140f;
            }
        }

        //Debug.Log("w/c animation set");

        Quaternion q = new Quaternion();
        q.eulerAngles = fang;
        transform.rotation = q;

        //Debug.Log("Angle Set");
        int lc = 0;
        //Debug.Log("end:" + end);
        while (transform.position != end)
        {
            //Debug.Log("t:"+transform.position.z+",e:"+end.z);
            transform.position = Vector3.MoveTowards(transform.position, end, speed * 0.02f);
            //Debug.Log("lc:" + lc+", transformed");
            lc++;
            yield return new WaitForSeconds(0.02f);
            time = time - 0.02f;
            //Debug.Log("lc:" + lc);
            if (lc > 10)
            {

                transform.position = end;
                break;
            }
        }
        //Debug.Log("Transformation fixed");
        if (movers.Count > 0)
        {
           // Debug.Log("Next Move Enqued");
            StartCoroutine(movers.Dequeue());
        }
        else if (time > 0)
        {
            yield return new WaitForSeconds(time);
            time = 0;
            if (movers.Count > 0)
            {
                //Debug.Log("Next Move Enqued");
                StartCoroutine(movers.Dequeue());
            }
            else
            {
                Debug.Log("stopped");
                isMoving = false;
                movers.Clear();
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 0);
                anim.SetFloat("CrouchX", 0);
                anim.SetFloat("CrouchY", 0);
            }
        }
        else
        {
            Debug.Log("stopped");
            isMoving = false;
            movers.Clear();
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);
            anim.SetFloat("CrouchX", 0);
            anim.SetFloat("CrouchY", 0);
        }

    }

    public IEnumerator MoveOverSecond(Vector3 end, float seconds = 0.2f)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Direction = (end - transform.position).normalized;
        if (inCrouch)
        {
            Crouching(Direction.x, Direction.z);
        }
        else
        {
            Walking(Direction.x, Direction.z);
        }
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            mt -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end;
    }

    public void TransportPlayer(Vector3 end, Vector3 fang)
    {
        Debug.Log("Transported");
        movers.Clear();
        transform.position = end;
        Quaternion q = new Quaternion();
        q.eulerAngles = fang;
        transform.rotation = q;
        isMoving = false;
        lastPos = end;
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
        anim.SetFloat("CrouchX", 0);
        anim.SetFloat("CrouchY", 0);
    }

    void Walking(float X, float Y)
    {
        anim.SetFloat("MoveX", X);
        anim.SetFloat("MoveY", Y);
        anim.SetFloat("CrouchX", 0);
        anim.SetFloat("CrouchY", 0);
    }

    void Crouching(float X, float Y)
    {
        anim.SetFloat("CrouchX", X);
        anim.SetFloat("CrouchY", Y);
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
    }

}
