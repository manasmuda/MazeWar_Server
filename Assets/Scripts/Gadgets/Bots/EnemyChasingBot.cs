using UnityEngine;
using UnityEngine.AI;

public enum CurrentState
{
    checking,

    patrol,

    chasing,

    shooting,

    nostate
}

public class EnemyChasingBot : MonoBehaviour
{
    NavMeshAgent agent;
    

    public CurrentState currentState;
    
    public RaycastHit hit;
    public Ray ray;

    public Transform playerPos;
    public Transform targetLookAt;
    public Transform gunPos;

    public GameObject bullet;

    public int bulletSpeed;

    public bool isVisible;

    public bool canShoot = true;

    public bool BotDataSet = false;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        currentState = CurrentState.checking;
        ray = new Ray();
    }

    public void SetData(string team, string id)
    {
        GetComponent<BotInfo>().team = team;
        GetComponent<BotInfo>().playerID = id;
        BotDataSet = true;
    }


    void Update()
    {
        if (BotDataSet)
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
                    if (playerPos != null)
                    {
                        canShoot = true;
                        agent.SetDestination(playerPos.position);
                    }
                    else
                    {
                        currentState = CurrentState.patrol;
                    }
                    break;
                case CurrentState.shooting:
                    agent.SetDestination(transform.position);
                    agent.transform.LookAt(new Vector3(targetLookAt.position.x, 0f, targetLookAt.position.z));
                    ShootEnemy();
                    break;
                default:
                    break;
            }
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

    public void GoToPatrolState()
    {
        currentState = CurrentState.patrol;
    }

    public void GoToShootState()
    {
        if (currentState == CurrentState.chasing || currentState == CurrentState.checking)
        {
            currentState = CurrentState.shooting;
        }
    }

    public bool CheckState(CurrentState state)
    {
        return currentState == state;
    }

    public void UpdateState(CurrentState state)
    {
        currentState = state;
    }

    public void ShootEnemy()
    {
        currentState = CurrentState.shooting;
        RaycastHit hit2;

        Debug.DrawRay(transform.position +transform.up, transform.forward*5f,Color.red);
        if (Physics.Raycast(transform.position + transform.up, transform.forward, out hit2, 6f))
        {
            //Debug.Log(hit2.collider.gameObject.name);

            if (hit2.collider.CompareTag("Player"))
            {
                if (canShoot)
                {
                    Debug.Log("1");
                    Invoke("ShootPlayer", 0.5f);
                    canShoot = false;
                }
            }
            else
            {
                currentState = CurrentState.chasing;
            }
        }
        
    }
    public void ShootPlayer()
    {
        canShoot = true;
        GameObject temp = Instantiate(bullet, gunPos.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().AddForce(gunPos.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        Destroy(temp, 1f);
    }
}
