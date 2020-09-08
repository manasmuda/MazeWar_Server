
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class CoinCollectorBot : MonoBehaviour
{
    NavMeshAgent agent;
    
    int shortest;
    private int coinCount=0;

    public Transform spawn;
    public List<Transform> Coins = new List<Transform>();

    public Transform coinParent;

    public Text coinText;

    public Vector3 dest;

    public bool isSearching= true;
    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        CheckCoins();
    }

    private void Update()
    {
        if(Coins.Count==0||!isSearching)
        {
            agent.SetDestination(spawn.position);
        }

        //coinText.text = "Coins Collected:" + coinCount + " ";

        if (isSearching)
        {
            CheckClosest(Coins);
        }

        if(Vector3.Distance(transform.position,spawn.transform.position)<=3f)
        {
            
            isSearching = true;
        }
        //Debug.Log("xyz:"+Vector3.Distance(transform.position, spawn.transform.position));
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
            dest = Coins[shortest].position;
        }
    }

    public void CheckCoins()
    {
        //when coin is instantiated it needs to be a child of coinParent

        for (int i = 0; i < coinParent.transform.childCount; i++)
        {
            Coins.Add(coinParent.transform.GetChild(i));
        }
        //CheckClosest(Coins);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            Coins.Remove(other.gameObject.transform);
            other.gameObject.SetActive(false);
            coinCount++;
            isSearching = false;
            //CheckClosest(Coins);
        }
    }
}
