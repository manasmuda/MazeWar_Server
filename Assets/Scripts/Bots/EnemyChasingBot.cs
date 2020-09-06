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

    public Transform playerPos;

    public Transform targetLookAt;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        currentState = CurrentState.checking;
    }

    void Update()
    {

        switch (currentState)
        {
            case CurrentState.checking:
                CheckPlayer();
                break;
            case CurrentState.patrol:
                Patrol();
                break;
            case CurrentState.chasing:
                agent.SetDestination(playerPos.position);
                
                break;
            case CurrentState.shooting:
                Debug.Log("shooting");
                agent.transform.LookAt(targetLookAt);
                break;
            default:
                break;
        }
    }

    void CheckPlayer()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider != null)
            {
                agent.SetDestination(hit.point);
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
                float wallAngle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg + 180;
                //Debug.Log(wallAngle);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, wallAngle, transform.localEulerAngles.z);
                currentState = CurrentState.checking;
            }

        }
    }

    public void ChasePlayer()
    {
        currentState = CurrentState.chasing;
    }

    public void ResetState()
    {
        currentState = CurrentState.checking;
    }


    public void PlayerDetected()
    {
        currentState = CurrentState.shooting;
    }

    public void ShootPlayer()
    {

    }

}
