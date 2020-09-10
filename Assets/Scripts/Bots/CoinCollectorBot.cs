
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class CoinCollectorBot : MonoBehaviour
{

    public static CoinCollectorBot coinCollectorBot_instance;

    NavMeshAgent agent;
    
    int shortest;
    //private int coinCount=0;

    public Transform spawn;
    public List<Transform> Coins = new List<Transform>();

    public Transform coinParent;

    //public Text coinText;

    public bool isSearching= true;

    public bool isChecking=false;
    
    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        //CheckCoins();
        
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            Coins.Remove(other.gameObject.transform);
            other.gameObject.SetActive(false);
            isSearching = false;
        }
    }
}
