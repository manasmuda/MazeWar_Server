using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientData
{

    public string playerId;
    public string team;

    public string name;
    public int level;
    public Texture2D playerImage;

    public TcpClient tcpClient;
    public IPEndPoint udpEndPoint;

    //Gadgets Data

    public GameObject character;
    public int damage;

    public int coinsCollected;
    public int coinsLost;
    public int kills;
    public int deaths;
    public int coinsSnatched;

    public Gadget LGadget1;
    public Gadget LGadget2;
    public Gadget HGadget1;
    public Gadget HGadget2;

    public Gadget[] gadgets = new Gadget[4];

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
        level = 1;
        damage = 10 + (level - 1) * 2;
    }

    public void AddGadgets(Gadget hgadget1,Gadget hgadget2,Gadget lgadget1,Gadget lgadget2)
    {
        gadgets = new Gadget[4] { hgadget1, hgadget2, lgadget1, lgadget2 };
        gadgets[0].playerId = playerId;
        gadgets[0].team = team;
        gadgets[1].playerId = playerId;
        gadgets[1].team = team;
        gadgets[2].playerId = playerId;
        gadgets[2].team = team;
        gadgets[3].playerId = playerId;
        gadgets[3].team = team;
    }
}
