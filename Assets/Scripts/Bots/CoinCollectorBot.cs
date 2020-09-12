
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class CoinCollectorBot : MonoBehaviour
{

    public static CoinCollectorBot coinCollectorBot_instance;

    Animator anim;

    NavMeshAgent agent;
    
    int shortest;
    //private int coinCount=0;

    public Transform coinParent;
    public Transform spawn;

    public List<Transform> Coins = new List<Transform>();

    public bool isSearching= true;
    public bool isChecking=false;

    bool isStanding;
    
    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        
    }

    private void Awake()
    {
        if(coinCollectorBot_instance== null)
        {
            coinCollectorBot_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        if(isStanding)
        {
            return;
        }

        if (Coins.Count==0||!isSearching)
        {
            agent.SetDestination(spawn.position);
        }


        if (isSearching)
        {
            CheckClosest(Coins);
        }

        if(Vector3.Distance(transform.position,spawn.transform.position)<=3f)
        {
            isSearching = true;
        }

        if (isChecking)
        {
            CheckCoins();
        }

        Debug.Log(agent.pathStatus);

    }

    void CheckClosest(List<Transform> _coins)
    {

        if (_coins.Count != 0)
        {
            shortest = 0;

            for (int j = 0; j < _coins.Count; j++)
            {
                float a = Vector3.Distance(transform.position, Coins[shortest].transform.position);
                float b = Vector3.Distance(transform.position, Coins[j].transform.position);
                if (a > b)
                {
                    shortest = j;
                }
            }

            agent.SetDestination(Coins[shortest].position);

        }
    }

    public void CheckCoins()
    {
        int i;
        //when coin is instantiated it needs to be a child of coinParent
        for (i = 0; i < coinParent.transform.childCount; i++)
        {
            Coins.Add(coinParent.transform.GetChild(i));
            Debug.Log(i);
            
        }
            isChecking = false;
        

    }

    void ResetState()
    {
        isStanding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {

            anim.SetTrigger("isCollecting");
            Coins.Remove(other.gameObject.transform);
            other.gameObject.SetActive(false);
            isSearching = false;
            isStanding = true;
            Invoke("ResetState", 1f);
        }
    }

    
}
