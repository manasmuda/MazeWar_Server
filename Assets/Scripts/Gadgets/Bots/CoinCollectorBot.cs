using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class CoinCollectorBot : MonoBehaviour
{

    Animator anim;

    NavMeshAgent agent;

    int shortest;
    //private int coinCount=0;

    public Transform coinParent;
    public Vector3 spawn;

    public List<Transform> Coins = new List<Transform>();

    public bool isSearching = true;

    bool isStanding=false;

    public bool BotDataSet = false;

    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        
    }

    public void SetData(string team, string id)
    {
        GetComponent<BotInfo>().team = team;
        GetComponent<BotInfo>().playerID = id;
        float[] spa= SpawnManager.GetSpawnPos(team);
        spawn= new Vector3(spa[0], 1.5f, spa[2]);
        coinParent = GameObject.Find("CoinsParent").transform;
        BotDataSet = true;
    }

    private void Update()
    {
        if (BotDataSet)
        {
            CheckCoins();

            if (isStanding)
            {
                return;
            }

            if (Coins.Count == 0 || !isSearching)
            {
                agent.SetDestination(spawn);
            }


            if (isSearching)
            {
                CheckClosest(Coins);
            }

            if (Vector3.Distance(transform.position, spawn) <= 3f)
            {
                isSearching = true;
            }

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
        //when coin is instantiated it needs to be a child of coinParent
        Coins = new List<Transform>();
        //Transform[] list=coinParent.transform.GetComponentsInChildren<Transform>(false);
        for (int i = 0; i < coinParent.transform.childCount; i++)
        {
            if(coinParent.transform.GetChild(i).gameObject.activeSelf)
             Coins.Add(coinParent.transform.GetChild(i));
            //Debug.Log(i);
        }
    }

    void ResetState()
    {
        isStanding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            //need to make set active slower

            Coins.Remove(other.transform);
            other.transform.gameObject.SetActive(false);
            //Destroy(other.gameObject);
            //send message to all players
            SimpleMessage msg = new SimpleMessage(MessageType.CoinPicked);
            msg.intData = other.GetComponent<Coins>().coinId;
            Server.instance.server.TransmitMessage(msg);
            isSearching = false;
            isStanding = true;
            Invoke("ResetState", 2f);
        }
    }
}