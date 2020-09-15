using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasingBot : MonoBehaviour
{
    NavMeshAgent agent;
    public enum CurrentState
    {
        checking,

        patrol,

        chasing,

        shooting
    }

    public CurrentState currentState;
    
    public RaycastHit hit;
    public Ray ray;

    public Transform playerPos;
    public Transform targetLookAt;
    public Transform gunPos;

    public GameObject bullet;

    public int bulletSpeed;

    public bool isVisible;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        currentState = CurrentState.checking;
        ray = new Ray();
    }

    void Update()
    {
        

        switch (currentState)
        {
            case CurrentState.checking:

                CheckPlayer();
                break;
            case CurrentState.patrol:
                CancelInvoke();
                Patrol();
                break;
            case CurrentState.chasing:
                agent.SetDestination(playerPos.position);
                break;
            case CurrentState.shooting:
                //agent.transform.LookAt(new Vector3(targetLookAt.position.z,transform.position.y,targetLookAt.position.z));
                agent.transform.LookAt(new Vector3(targetLookAt.position.x,0f,targetLookAt.position.z));
                
                break;
            default:
                break;
        }


        //if (hit.collider.tag=="Player")
        //{
        //    isVisible = true;
        //}
        //else
        //{
        //    isVisible = false;
        //}
    }

    void CheckPlayer()
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                agent.SetDestination(hit.point);
                Invoke("UpdateRotation", 0.5f);
                currentState = CurrentState.patrol;
            }
        }
    }

    private void Patrol()
    {
        if(agent.remainingDistance<1f)
        {
            if(hit.collider!=null)
            {
                agent.updateRotation = false;
                //float wallAngle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg-180;
                //float wallAngle = Vector3.Angle(hit.normal, -ray.direction);
                //Debug.Log(wallAngle);
                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, wallAngle, transform.localEulerAngles.z);
                transform.rotation = Quaternion.LookRotation(hit.normal);
                currentState = CurrentState.checking;
            }

        }
    }

    void UpdateRotation()
    {
        agent.updateRotation = true;
    }

    public void GoToChaseState()
    {
        currentState = CurrentState.chasing;
    }

    public void GoToCheckState()
    {
        currentState = CurrentState.checking;
    }


    public void GoToShootState()
    {
        if (currentState == CurrentState.chasing || currentState == CurrentState.checking)
        {
            currentState = CurrentState.shooting;
        }
    }

    public void ShootEnemy()
    {
        InvokeRepeating("ShootPlayer", 0.4f,0.4f);
    }
    public void ShootPlayer()
    {
        GameObject temp = Instantiate(bullet, gunPos.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().AddForce(gunPos.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        Destroy(temp, 1.5f);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward*4f, Color.red);
    }

}
