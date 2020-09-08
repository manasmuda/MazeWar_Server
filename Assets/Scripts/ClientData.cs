using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientData
{

    private string playerId;
    private string name;
    private int level;

    private GameObject character;

    private int coinsCollected;
    private int coinsLost;
    private int kills;
    private int deaths;
    private int coinsSnatched;

    public ClientData(string id)
    {
        this.playerId = id;
    }
    
}
