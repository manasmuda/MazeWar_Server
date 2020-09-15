using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientData
{

    public string playerId;
    public string name;
    public int level;
    public Texture2D playerImage;

    public TcpClient tcpClient;
    public IPEndPoint udpEndPoint;

    //Gadgets Data

    public GameObject character;

    public int coinsCollected;
    public int coinsLost;
    public int kills;
    public int deaths;
    public int coinsSnatched;

    public ClientData(string id,TcpClient tcpClient)
    {
        //Debug.Log("Creating client data");
        this.playerId = id;
        this.tcpClient = tcpClient;
        coinsCollected = 0;
        coinsLost = 0;
        kills = 0;
        deaths = 0;
        coinsSnatched = 0;
    }

    
}
