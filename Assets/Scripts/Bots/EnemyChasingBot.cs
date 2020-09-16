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
                CancelInvoke();
                agent.SetDestination(playerPos.position);
                break;
            case CurrentState.shooting:
                agent.transform.LookAt(new Vector3(targetLookAt.position.x,0f,targetLookAt.position.z));
                ShootEnemy();
                
                break;
            default:
                break;
        }

        

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
        currentState = CurrentState.shooting;
        RaycastHit hit2;

        Debug.DrawRay(transform.position +transform.up, transform.forward*6f,Color.red);
        if (Physics.Raycast(transform.position + transform.up, transform.forward, out hit2, 6f))
        {
            //Debug.Log(hit2.collider.gameObject.name);

            if (hit2.collider.CompareTag("Player"))
            {
                
                InvokeRepeating("ShootPlayer", 0.4f, 0.4f);
            }
        }
        
    }
    public void ShootPlayer()
    {
        GameObject temp = Instantiate(bullet, gunPos.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().AddForce(gunPos.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        Destroy(temp, 1.5f);
    }

  
}
