using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Aws.GameLift.Server;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Linq;

// *** MONOBEHAVIOUR TO MANAGE SERVER LOGIC *** //

public class Server : MonoBehaviour
{
    //We get events back from the NetworkServer through this static list
    public static List<SimpleMessage> messagesToProcess = new List<SimpleMessage>();

    public NetworkServer server;

    public int tick = 0;
    private float tickRate = 0.2f;
    private float tickCounter = 0f;

    private GameLift gameliftServer;

    public GameObject characterPrefab;

    public GameObject coinPrefab;

    public static Server instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameliftServer = GameObject.FindObjectOfType<GameLift>();
        server = new NetworkServer(gameliftServer, this);
    }

    // Update is called once per frame
    void Update()
    {
        this.tickCounter += Time.deltaTime;
        if (this.tickCounter >= tickRate)
        {
            if (gameliftServer.GameStarted() && tick > 0) {
                CharacterSyncUpdate();
            }
            this.tickCounter = 0.0f;
            tick++;
            if (tick % 5 == 0 && gameliftServer.GameStarted())
                server.SendServerTick();
            if (gameliftServer.GameStarted())
                CreateGameState();
            server.Update();
        }


        // Go through any messages to process (on the game world)
        foreach (SimpleMessage msg in messagesToProcess)
        {
            // NOTE: We should spawn players and set positions also on server side here and validate actions. For now we just pass this data to clients
        }
        messagesToProcess.Clear();
    }

    public void CharacterSyncUpdate()
    {
        GameState gs = GameHistory.recentState;
        UdpMsgPacket packet = new UdpMsgPacket(PacketType.GameState);
        packet.gameState = gs;
        for(int i = 0; i < GameHistory.recentState.redTeamState.Values.Count; i++)
        {
            ClientState clientState = GameHistory.recentState.redTeamState.Values.ElementAt(i);
            string tempPlayerId = clientState.playerId;
            //Debug.Log("Positon Update:" + GameHistory.recentState.redTeamState[i].position[0] + "+" + GameHistory.recentState.redTeamState[i].position[1] + "+" + GameHistory.recentState.redTeamState[i].position[2]);
            GameData.redTeamData.clientsData[tempPlayerId].character.GetComponent<CharacterSyncScript>().NewPlayerState(clientState);
            server.SendPacket(packet, GameData.redTeamData.clientsData[tempPlayerId].udpEndPoint);
        }

        for (int i = 0; i < GameHistory.recentState.blueTeamState.Values.Count; i++)
        {
            ClientState clientState = GameHistory.recentState.blueTeamState.Values.ElementAt(i);
            string tempPlayerId = clientState.playerId;
            //Debug.Log("Positon Update:" + GameHistory.recentState.redTeamState[i].position[0] + "+" + GameHistory.recentState.redTeamState[i].position[1] + "+" + GameHistory.recentState.redTeamState[i].position[2]);
            GameData.blueTeamData.clientsData[tempPlayerId].character.GetComponent<CharacterSyncScript>().NewPlayerState(clientState);
            server.SendPacket(packet, GameData.blueTeamData.clientsData[tempPlayerId].udpEndPoint);
        }
    }

    public void StartTick()
    {
        tick = 0;
        tickCounter = 0f;
    }

    public void GameReady()
    {
        StartCoroutine(StartLobbyTimer());
    }

    public void CreateGameState()
    {
        GameState state = new GameState();
        GameState newState=GameHistory.GetPredictedState(tick);
        // send this game state to all clients.
        //GameHistory.AddGameState(newState, tick);
    }

    IEnumerator StartLobbyTimer()
    {
        Debug.Log("Time Scale:" + Time.timeScale);
        Debug.Log("Lobby Timer Started");
        float time = 0f;
        while (time < 10f)
        {
            yield return new WaitForSeconds(2f);
            time = time + 2f;
            Debug.Log("Lobby waited for " + time + " seconds and time scale is "+Time.timeScale+" and time is "+Time.realtimeSinceStartup);
        }
        Debug.Log("Lobby Timer Ended");
        InitializeFirstGameState();
        StartTick();
        gameliftServer.StartGame();
        server.StartGame();
    }

    void InitializeFirstGameState()
    {
        Debug.Log("GameInitializtion Started");
        GameState state = new GameState();
        state.tick = 0;
        state.stateType = 0;
        foreach(KeyValuePair<string,ClientData> data in GameData.blueTeamData.clientsData)
        {
            ClientState clientState = new ClientState();
            clientState.position[0] = data.Value.character.transform.position.x;
            clientState.position[1] = data.Value.character.transform.position.y;
            clientState.position[2] = data.Value.character.transform.position.z;
            clientState.angle[0] = data.Value.character.transform.rotation.x;
            clientState.angle[1] = data.Value.character.transform.rotation.y;
            clientState.angle[2] = data.Value.character.transform.rotation.z;
            clientState.playerId = data.Key;
            clientState.team = "blue";
            clientState.tick = 0;
            clientState.stateType = 0;
            clientState.health = 100;
            clientState.coinsHolding = 0;
            clientState.tracersRemaining = 2;
            clientState.bulletsLeft = 10;
            state.blueTeamState.Add(data.Key,clientState);
        }
        foreach (KeyValuePair<string, ClientData> data in GameData.redTeamData.clientsData)
        {
            ClientState clientState = new ClientState();
            clientState.position[0] = data.Value.character.transform.position.x;
            clientState.position[1] = data.Value.character.transform.position.y;
            clientState.position[2] = data.Value.character.transform.position.z;
            clientState.angle[0] = data.Value.character.transform.rotation.x;
            clientState.angle[1] = data.Value.character.transform.rotation.y;
            clientState.angle[2] = data.Value.character.transform.rotation.z;
            clientState.playerId = data.Key;
            clientState.team = "red";
            clientState.tick = 0;
            clientState.stateType = 0;
            clientState.health = 100;
            clientState.coinsHolding = 0;
            clientState.tracersRemaining = 2;
            clientState.bulletsLeft = 10;
            state.redTeamState.Add(data.Key,clientState);
        }
        GameHistory.AddGameState(state);
        Debug.Log("Initial State Added");
    }

    public void DisconnectAll()
    {
        this.server.DisconnectAll();
    }

    public void ResetServer()
    {
        server = new NetworkServer(gameliftServer,this);
        tick = 0;
        tickCounter = 0f;
    }

}

// *** SERVER NETWORK LOGIC *** //

public class NetworkServer
{
	private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<TcpClient> readyClients = new List<TcpClient>();
    private List<TcpClient> clientsToRemove = new List<TcpClient>();
    private List<IPEndPoint> endPoints = new List<IPEndPoint>();

    private Socket udpServer;
    private Dictionary<IPEndPoint, string> udpEndPoints;

    private UdpClient udpListener;

    private GameLift gamelift = null;
    private Server server;

    public NetworkServer(GameLift gamelift,Server server)
	{
        this.gamelift = gamelift;
        this.server = server;

        //Start the TCP server
        int port = this.gamelift.listeningPort;
        Debug.Log("Starting server on port " + port);
        listener = new TcpListener(IPAddress.Any, this.gamelift.listeningPort);
        Debug.Log("Listening at: " + listener.LocalEndpoint.ToString());
		listener.Start();

        endPoints = new List<IPEndPoint>();
        udpEndPoints = new Dictionary<IPEndPoint, string>();
        /*IPHostEntry host = Dns.Resolve(Dns.GetHostName());
        IPAddress ip = host.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ip, this.gamelift.listeningPort);
        udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpServer.Bind(endPoint);
        udpServer.Blocking = false;*/

        udpListener = new UdpClient(this.gamelift.listeningPort+20);
        udpListener.BeginReceive(UDPRecieveCallBack, null);

        //Thread t1 = new Thread(ListenMsg);
        //t1.Start();
	}

    /*public void ListenMsg()
    {
        while (true)
        {
            if (udpServer.Available!=0)
            {
                byte[] packet = new byte[64];
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                int rec = udpServer.ReceiveFrom(packet, ref sender);
                string info = Encoding.Default.GetString(packet);

                Debug.Log("Server received: " + info);
                Debug.Log(((IPEndPoint)sender).Address);

                if (info[0] == 's')
                {
                    this.endPoints.Add(sender);
                    ClientData client1 = new ClientData("");
                    this.udpClients.Add(sender, client1);

                    UdpMsgPacket msgPacket = new UdpMsgPacket(PacketType.Spawn,"Welcome to UDP");
                    SendPacket(msgPacket, ((IPEndPoint)sender));
                }
            }
        }
    }*/

    private void UDPRecieveCallBack(IAsyncResult result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
            UdpMsgPacket msgPacket = NetworkProtocol.getPacketfromBytes(data);


            udpListener.BeginReceive(UDPRecieveCallBack, null);
            //string msg=Encoding.Default.GetString(data);
            //Debug.Log("Recieved UDP msg:"+msgPacket.type);
            HandleUdpMsg(msgPacket, clientEndPoint);            

            if (data.Length < 4)
            {
                return;
            }

            //Debug.Log(msgPacket.message);

            //UdpMsgPacket msgPacket = new UdpMsgPacket(PacketType.Spawn, "Welcome to UDP");
            //SendPacket(msgPacket, clientEndPoint);

        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    // Checks if socket is still connected
    private bool IsSocketConnected(TcpClient client)
    {
        var bClosed = false;

        // Detect if client disconnected
        if (client.Client.Poll(0, SelectMode.SelectRead))
        {
            byte[] buff = new byte[1];
            if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
            {
                // Client disconnected
                bClosed = true;
            }
        }

        return !bClosed;
    }

    public void Update()
	{
		// Are there any new connections pending?
		if (listener.Pending())
		{
            Debug.Log("Client pending..");
			TcpClient client = listener.AcceptTcpClient();
            client.NoDelay = true; // Use No Delay to send small messages immediately. UDP should be used for even faster messaging
            Debug.Log("Client accepted.");

            // We have a maximum of 10 clients per game
            if(this.clients.Count < 10)
            {
                this.clients.Add(client);
                return;
            }
            else
            {
                // game already full, reject the connection
                try
                {
                    SimpleMessage message = new SimpleMessage(MessageType.Reject, "game already full");
                    NetworkProtocol.Send(client, message);
                }
                catch (SocketException) {

                }
            }

		}

        // Iterate through clients and check if they have new messages or are disconnected
        int playerIdx = 0;
        foreach (var client in this.clients)
		{
            try
            {
                if (client == null) continue;
                if (this.IsSocketConnected(client) == false)
                {
                    Debug.Log("Client not connected anymore");
                    this.clientsToRemove.Add(client);
                }
                var messages = NetworkProtocol.Receive(client);
                foreach(SimpleMessage message in messages)
                {
                    Debug.Log("Received message: " + message.message + " type: " + message.messageType);
                    bool disconnect = HandleMessage(playerIdx, client, message);
                    if (disconnect)
                        this.clientsToRemove.Add(client);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving from a client: " + e.Message);
                this.clientsToRemove.Add(client);
            }
            playerIdx++;
		}

        //Remove dead clients
        foreach (var clientToRemove in this.clientsToRemove)
        {
            try
            {
                this.RemoveClient(clientToRemove);
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't remove client: " + e.Message);
            }
        }
        this.clientsToRemove.Clear();

        //End game if no clients
        if(this.gamelift.GameStarted())
        {
            if(this.clients.Count <= 0)
            {
                Debug.Log("Clients gone, stop session");
                this.gamelift.TerminateGameSession();
            }
        }

        // Simple test for the the StatsD client: Send current amount of player online
    }

    public void DisconnectAll()
    {
        // warn clients
        SimpleMessage message = new SimpleMessage(MessageType.Disconnect);
        TransmitMessage(message);
        // disconnect connections
        foreach (var client in this.clients)
        {
            this.clientsToRemove.Add(client);
        }

        //Reset the client lists
        this.clients = new List<TcpClient>();
        this.readyClients = new List<TcpClient>();
        this.endPoints = new List<IPEndPoint> { };
        this.udpEndPoints = new Dictionary<IPEndPoint, string> { };
	}

    //Transmit message to multiple clients
	public void TransmitMessage(SimpleMessage msg, TcpClient excludeClient = null)
	{
        // send the same message to all players
        foreach (var client in this.clients)
		{
            //Skip if this is the excluded client
            if(excludeClient != null && excludeClient == client)
            {
                continue;
            }

			try
			{
				NetworkProtocol.Send(client, msg);
			}
			catch (Exception e)
			{
                this.clientsToRemove.Add(client);
			}
		}
    }

    public void TransmitPacket(UdpMsgPacket packet)
    {
        foreach(KeyValuePair<IPEndPoint,string> endpoint in udpEndPoints)
        {
            SendPacket(packet, endpoint.Key);
        }   
    }

    //Send message to single client
    private void SendMessage(TcpClient client, SimpleMessage msg)
    {
        try
        {
            NetworkProtocol.Send(client, msg);
        }
        catch (Exception e)
        {
            this.clientsToRemove.Add(client);
        }
    }

    private bool HandleMessage(int playerIdx, TcpClient client, SimpleMessage msg)
	{
        if (msg.messageType == MessageType.Connect)
        {
            HandleConnect(playerIdx, msg.message, client);
        }
        else if (msg.messageType == MessageType.Disconnect)
        {
            this.clientsToRemove.Add(client);
            return true;
        }
        else if (msg.messageType == MessageType.Ready)
        {
            HandleReady(client);
        }
        else if (msg.messageType == MessageType.PlayerGameReady)
        {
            HandlePlayerGameReady(client,msg);
        }
        else if (msg.messageType == MessageType.GadgetCallAction)
        {
            HandleGadgetCallAction(msg);
        }

        return false;
    }

    private void HandleUdpMsg(UdpMsgPacket packet,IPEndPoint clientEndPoint)
    {
        if (packet.type == PacketType.UDPConnect)
        {
            HandleUdpConnect(clientEndPoint,packet.playerId,packet.team);
        }
        if (packet.type == PacketType.ClientState)
        {
            HandleClientState(packet);
        }
        if (packet.type == PacketType.Shoot)
        {
            HandleShootAction(packet);
        }
    }

    private void HandleUdpConnect(IPEndPoint clientEndPoint,string playerId,string team)
    {
        if (!udpEndPoints.ContainsKey(clientEndPoint))
        {
            udpEndPoints.Add(clientEndPoint, playerId);
            endPoints.Add(clientEndPoint);
            Debug.Log("Team:"+team);
            if (team == "red")
            {
                Debug.Log("end point setting");
                GameData.redTeamData.clientsData[playerId].udpEndPoint = clientEndPoint;
                Debug.Log("end point set");
            }
            else if (team == "blue")
            {
                GameData.blueTeamData.clientsData[playerId].udpEndPoint = clientEndPoint;
            }
            UdpMsgPacket packet = new UdpMsgPacket(PacketType.UDPConnect, "Success", "", "");
            SendPacket(packet, clientEndPoint);
        }
    }

    private void HandleGadgetCallAction(SimpleMessage msg)
    {
        Debug.Log("Handle Gadget Call Action");
        if (msg.stringArrData[1] == "blue")
        {
            ClientData clientData = GameData.blueTeamData.clientsData[msg.stringArrData[0]];
            Gadget gadget = clientData.gadgets[msg.intData];
            if (gadget.CanCall())
            {
                SimpleMessage message = new SimpleMessage(MessageType.GadgetCallAction);
                message.intData = msg.intData;
                message.stringArrData = msg.stringArrData;
                gadget.CallAction(message);
                SendMessage(clientData.tcpClient, message);
                SimpleMessage message1 = new SimpleMessage(MessageType.GadgetCallAction);
                message1.stringData = gadget.gname;
                message1.stringArrData = new string[3] { msg.stringArrData[0], msg.stringArrData[1], message.stringData };
                message.floatArrData = new float[3] { gadget.character.transform.position.x, gadget.character.transform.position.x, gadget.character.transform.position.x };
                TransmitMessage(message1, clientData.tcpClient);
                //Send to other players for spawn
            }
        }
        else if (msg.stringArrData[1] == "red")
        {
            ClientData clientData = GameData.redTeamData.clientsData[msg.stringArrData[0]];
            Gadget gadget = clientData.gadgets[msg.intData];
            if (gadget.CanCall())
            {
                SimpleMessage message = new SimpleMessage(MessageType.GadgetCallAction);
                message.intData = msg.intData;
                message.stringArrData = msg.stringArrData;
                gadget.CallAction(message);
                SendMessage(clientData.tcpClient, message);
                SimpleMessage message1 = new SimpleMessage(MessageType.GadgetCallAction);
                message1.stringData = gadget.gname;
                message1.stringArrData = new string[3] { msg.stringArrData[0], msg.stringArrData[1], message.stringData };
                message.floatArrData = new float[3] { gadget.character.transform.position.x, gadget.character.transform.position.x, gadget.character.transform.position.x };
                TransmitMessage(message1, clientData.tcpClient);
                //Send to other players for spawn
            }
        }
    }

    private void HandleShootAction(UdpMsgPacket packet)
    {
        ClientState state = packet.clientState;
        if (server.tick - state.tick < 5)
        {
            Debug.Log("Shoot Action: Shoot Tick Accepted "+server.tick);
            LinkedListNode<GameState> llnGState = GameHistory.GetGameState(state.tick);
            if (llnGState != null)
            {
                Debug.Log("Shoot Action: Found Game State");
                GameState gState = llnGState.Value;

                ClientState hitClientState = null;
                ClientState shooterClientState = null;

                bool shootSuccess = true;
                int damage = 0;
                if (state.bulletHit)
                {
                    Debug.Log("Shoot Action: Bullet Hit");
                    if (state.team == "blue")
                    {
                        Debug.Log("Shoot Action: Blue");
                        damage = GameData.blueTeamData.clientsData[state.playerId].damage;
                        hitClientState = gState.redTeamState[state.bulletHitId];
                        if ((Mathf.Abs(hitClientState.position[0] - state.bulletHitPosition[0]) > 2) || (Mathf.Abs(hitClientState.position[2] - state.bulletHitPosition[2]) > 2))
                        {
                            shootSuccess = false;
                        }
                        shooterClientState = gState.blueTeamState[state.playerId];
                        Debug.Log("Shoot Action: shooterClientState is set");
                    }
                    else
                    {
                        Debug.Log("Shoot Action: Red");
                        damage = GameData.redTeamData.clientsData[state.playerId].damage;
                        hitClientState = gState.blueTeamState[state.bulletHitId];
                        if ((Mathf.Abs(hitClientState.position[0] - state.bulletHitPosition[0]) > 2) || (Mathf.Abs(hitClientState.position[2] - state.bulletHitPosition[2]) > 2))
                        {
                            shootSuccess = false;
                        }
                        shooterClientState = gState.redTeamState[state.playerId];
                        Debug.Log("Shoot Action: shooterClientState is set");
                    }
                }
                if (shooterClientState != null)
                {
                    Debug.Log("Shoot Action: shooterClientState is not null");
                    if (shooterClientState.bulletsLeft > 0)
                    {
                        Debug.Log("Shoot Action: Shot Started");
                        int bd = -1;
                        int hd = 0;
                        if (shootSuccess)
                        {
                            Debug.Log("Shoot Action: Shot Success");
                            hd = -1 * damage;
                        }
                        //shooterClientState.bulletsLeft = shooterClientState.bulletsLeft - 1;
                        LinkedListNode<GameState> cn = llnGState;
                        while (cn != null)
                        {
                            if (state.team == "blue")
                            {
                                cn.Value.blueTeamState[state.playerId].bulletsLeft += bd;
                                cn.Value.redTeamState[state.bulletHitId].health += hd;
                                if (cn.Value.redTeamState[state.bulletHitId].health < 0)
                                {
                                    //Die Action
                                }
                            }
                            else
                            {
                                cn.Value.redTeamState[state.playerId].bulletsLeft += bd;
                                cn.Value.blueTeamState[state.bulletHitId].health += hd;
                                if (cn.Value.blueTeamState[state.bulletHitId].health < 0)
                                {
                                    //Die Action
                                }
                            }
                            cn = cn.Next;
                        }
                    }
                }
                else
                {
                    Debug.Log("Shoot Action: shooterClientState is null");
                }
                UdpMsgPacket msgPacket = new UdpMsgPacket(PacketType.Shoot, "", "", "");
                msgPacket.gameState = gState;
                msgPacket.clientState = state;
                TransmitPacket(msgPacket);
            }
            else
            {
                Debug.Log("Shoot Action: Game State is null");
            }
        }
    }

    private void HandleGameState(UdpMsgPacket packet)
    {

    }

    private void HandleClientState(UdpMsgPacket packet)
    {
        //Debug.Log(packet.clientState.tick + "," + server.tick);
        int us = GameHistory.CheckClientState(packet.clientState);
        if (server.tick % 23 == 0)
        {
            Debug.Log(packet.playerId + " server tick: " + packet.clientState.tick + "," + server.tick);
        }
        //Debug.Log(us);
        //Debug.Log(packet.clientState.position[0] + "," + packet.clientState.position[1] + "," + packet.clientState.position[2]);
        /*if (us==0)
        {
            if (packet.team == "red")
            {
                GameData.redTeamData.clientsData[packet.playerId].character.GetComponent<CharacterSyncScript>().AddNewPlayerPos(new Vector3(packet.clientState.position[0], packet.clientState.position[1], packet.clientState.position[2]), Quaternion.identity);
            }
            else
            {
                GameData.blueTeamData.clientsData[packet.playerId].character.GetComponent<CharacterSyncScript>().AddNewPlayerPos(new Vector3(packet.clientState.position[0], packet.clientState.position[1], packet.clientState.position[2]), Quaternion.identity);
            }
        }*/
    }

	private void HandleConnect(int playerIdx, string json, TcpClient client)
	{
        // respond with the player id and the current state.
        //Connect player
        var outcome = GameLiftServerAPI.AcceptPlayerSession(json);
        if (outcome.Success)
        {
            Debug.Log(":) PLAYER SESSION VALIDATED");
            
            SimpleMessage message = new SimpleMessage(MessageType.PlayerAccepted,"Player Accepted by server");
            this.SendMessage(client, message);
        }
        else
        {
            Debug.Log(":( PLAYER SESSION REJECTED. AcceptPlayerSession() returned " + outcome.Error.ToString());
            this.clientsToRemove.Add(client);
        }
	}

	private void HandleReady(TcpClient client)
	{
        // start the game once all connected clients have requested to start (RETURN key)
        this.readyClients.Add(client);

        string team = "blue";
        // get client details from lambda like team, level, gadgets

        string hg1 = "CoinCollectingBot";
        string hg2= "CoinCollectingBot";
        string lg1 = "ShootingBot";
        string lg2 = "ShootingBot";

        Gadget lgadget1=null;
        Gadget hgadget1=null;
        Gadget lgadget2=null;
        Gadget hgadget2=null;

        try
        {
            GameObject lg1object = Resources.Load<GameObject>("GadgetControllers/"+lg1+"GadgetController");
            lg1object = GameObject.Instantiate(lg1object, Vector3.zero, Quaternion.identity);
            lgadget1 = lg1object.GetComponent<Gadget>();
            GameObject lg2object = Resources.Load<GameObject>("GadgetControllers/" + lg2 + "GadgetController");
            lg2object = GameObject.Instantiate(lg2object, Vector3.zero, Quaternion.identity);
            lgadget2 = lg2object.GetComponent<Gadget>();
            GameObject hg1object = Resources.Load<GameObject>("GadgetControllers/" + hg1 + "GadgetController");
            hg1object = GameObject.Instantiate(hg1object, Vector3.zero, Quaternion.identity);
            hgadget1 = hg1object.GetComponent<Gadget>();
            GameObject hg2object = Resources.Load<GameObject>("GadgetControllers/" + hg2 + "GadgetController");
            hg2object = GameObject.Instantiate(hg2object, Vector3.zero, Quaternion.identity);
            hgadget2 = hg2object.GetComponent<Gadget>();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            Debug.LogError(e.Message);
            Debug.LogError(e.InnerException);
        }
        Debug.Log("Gadget created");

        if (lgadget1 == null)
        {
            Debug.Log("Gadget are null");
        }

        //temp code start
        if (readyClients.Count % 2 == 0)
        {
            team = "blue";
        }
        else
        {
            team = "red";
        }
        //end
        Debug.Log("Team Decided");
        if (team == "blue")
        {
            Debug.Log("blue");
            ClientData tempClientData = GameData.blueTeamData.AddNewClient(client);
            string id = tempClientData.playerId;
            tempClientData.AddGadgets(hgadget1, hgadget2, lgadget1, lgadget2);
            Debug.Log("blue client added to game data");
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerData, "");
            msg.playerId = id;
            msg.team = team;
            msg.stringArrData = new string[] { hg1,hg2, lg1,lg2 };
            Debug.Log("blue:" + id);
            this.SendMessage(client, msg);
        }
        else if (team == "red")
        {
            Debug.Log("red");
            ClientData tempClientData = GameData.redTeamData.AddNewClient(client);
            string id = tempClientData.playerId;
            tempClientData.AddGadgets(hgadget1, hgadget2, lgadget1, lgadget2);
            Debug.Log("red client added to game data");
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerData, "");
            msg.playerId = id;
            msg.team = team;
            msg.stringArrData = new string[] { hg1, hg2, lg1, lg2 };
            Debug.Log("red:" + id);
            this.SendMessage(client, msg);
        }
        Debug.Log("Player Data Sent");
        if (readyClients.Count == 2) //players2 for temp
        {
            Debug.Log("Enough clients, let's start the game!");
            MazeCell[,] maze=MazeGenerator.generateMaze();
            Debug.Log("Maze Data Generated");
            GameData.maze = maze;
            Debug.Log("Maze Data Assigned");
            GameObject.Find("MazeController").GetComponent<MazeController>().InstantiateMaze(maze);
            Debug.Log("Maze Created");
            List<object> list=MazeGenerator.ToObjectList(maze);
            //GenerateCoins coins = GenerateCoins.GetCoinPosition();
            SimpleMessage msg = new SimpleMessage(MessageType.GameReady, "");
            Vector3[] coinPos = GenerateCoins.GetCoinPosition();
            GenerateCoins.InstantiateCoins(server.coinPrefab, coinPos);
            List<object> list1 = Converter.ToObjects(coinPos);
            msg.listData = list;
            msg.list1 = list1;
            TransmitMessage(msg);
            //server.StartTick();
            this.gamelift.ReadyGame();
            server.GameReady();
            //Initialize all team positions, team bases, container positions
            InitializePlayerGameData();
        }
    }

    public void SendServerTick()
    {
        SimpleMessage msg = new SimpleMessage(MessageType.ServerTick);
        msg.intData = server.tick;
        TransmitMessage(msg);
    }

    void InitializePlayerGameData()
    {
        Debug.Log("InitializePlayerGameData Called");
        GameData.InitializeSpawns();
        Debug.Log("Spawn Initialized");
        Dictionary<string,float[]> blueList = new Dictionary<string, float[]>();
        Dictionary<string,float[]> redList = new Dictionary<string, float[]>();
        SimpleMessage msg = new SimpleMessage(MessageType.PlayerGameData);
        foreach(KeyValuePair<string,ClientData> clientData in GameData.blueTeamData.clientsData)
        {
            //SimpleMessage msg = new SimpleMessage(MessageType.PlayerGameData);
            //msg.playerId = clientData.Key;
            float[] sp=SpawnManager.GetSpawnPos("blue");
            //msg.floatArrData = sp;
            TcpClient tcpClient=clientData.Value.tcpClient;
            GameObject character=GameObject.Instantiate(server.characterPrefab, new Vector3(sp[0], sp[1], sp[2]), Quaternion.identity);
            character.GetComponent<CharacterData>().playerId = clientData.Key;
            //clientData.Value.character = character;
            GameData.blueTeamData.clientsData[clientData.Value.playerId].character = character;
            //SendMessage(tcpClient, msg);
            blueList.Add(clientData.Key,sp);
            Debug.Log("Player SpawnMessage Sent");
        }
        Debug.Log("blue team spawn message sent");
        foreach (KeyValuePair<string, ClientData> clientData in GameData.redTeamData.clientsData)
        {
            //Debug.Log("red team client");
            //SimpleMessage msg = new SimpleMessage(MessageType.PlayerGameData);
            //msg.playerId = clientData.Key;
            float[] sp = SpawnManager.GetSpawnPos("red");
            //Debug.Log("Player spawn created");                                                                 
            //msg.floatArrData = sp;
            TcpClient tcpClient = clientData.Value.tcpClient;
            GameObject character = GameObject.Instantiate(server.characterPrefab, new Vector3(sp[0], sp[1], sp[2]), Quaternion.identity);
            character.GetComponent<CharacterData>().playerId = clientData.Key;
            //clientData.Value.character = character;
            GameData.redTeamData.clientsData[clientData.Value.playerId].character=character;
            //SendMessage(tcpClient, msg);
            redList.Add(clientData.Key, sp);
            Debug.Log("Player SpawnMessage Sent");
        }
        msg.redSpanData = redList;
        msg.blueSpanData = blueList;
        TransmitMessage(msg);
        Debug.Log("red team spawn message sent");
    }

    void HandlePlayerGameReady(TcpClient client,SimpleMessage msg)
    {

    }

    public void StartGame()
    {
        SimpleMessage msg = new SimpleMessage(MessageType.GameStarted, "");
        TransmitMessage(msg);
    } 

    public void SendPacket(UdpMsgPacket msgPacket, IPEndPoint addr)
    {
        try
        {
            byte[] arr = NetworkProtocol.getPacketBytes(msgPacket);
            udpListener.BeginSend(arr, arr.Length,addr,null,null);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void RemoveClient(TcpClient client)
    {
        //Let the other clients know the player was removed
        int clientId = this.clients.IndexOf(client);

        SimpleMessage message = new SimpleMessage(MessageType.PlayerLeft);
        message.clientId = clientId;
        TransmitMessage(message, client);

        // Disconnect and remove
        int clientIndex=this.clients.IndexOf(client);
        this.udpEndPoints.Remove(this.endPoints[clientIndex]);
        this.DisconnectPlayer(client);
        this.clients.Remove(client);
        this.readyClients.Remove(client);
        this.endPoints.RemoveAt(clientIndex);
    }

	private void DisconnectPlayer(TcpClient client)
	{
        try
        {
            // remove the client and close the connection
            if (client != null)
            {
                NetworkStream stream = client.GetStream();
                stream.Close();
                client.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to disconnect player: " + e.Message);
        }
	}

    public void ResetNetworkServer()
    {

    }
}
