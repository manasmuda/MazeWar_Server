using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    //public Transform coinParent;
    void Start()
    {
        transform.parent = GameObject.Find("CoinParent").transform;
        CoinCollectorBot.coinCollectorBot_instance.isChecking=true;
    }

}
