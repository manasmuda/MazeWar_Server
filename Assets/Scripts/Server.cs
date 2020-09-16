using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Aws.GameLift.Server;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Text;

// *** MONOBEHAVIOUR TO MANAGE SERVER LOGIC *** //

public class Server : MonoBehaviour
{
    //We get events back from the NetworkServer through this static list
    public static List<SimpleMessage> messagesToProcess = new List<SimpleMessage>();

    NetworkServer server;

    public int tick = 0;
    private float tickRate = 0.2f;
    private float tickCounter = 0f;

    private GameLift gameliftServer;

    public GameObject characterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameliftServer = GameObject.FindObjectOfType<GameLift>();
        server = new NetworkServer(gameliftServer,this);    
    }

    // Update is called once per frame
    void Update()
    {
        this.tickCounter += Time.deltaTime;
        if (this.tickCounter >= tickRate)
        {
            Debug.Log("Update");
            if (gameliftServer.GameStarted() && tick>0) {
                CharacterSyncUpdate();
            }
            this.tickCounter = 0.0f;
            tick++;
            if(gameliftServer.GameStarted())
                CreateGameState();
            server.Update();
        }
        

        // Go through any messages to process (on the game world)
        foreach(SimpleMessage msg in messagesToProcess)
        {
            // NOTE: We should spawn players and set positions also on server side here and validate actions. For now we just pass this data to clients
        }
        messagesToProcess.Clear();
    }

    public void CharacterSyncUpdate()
    {
        for (int i = 0; i < GameHistory.recentState.redTeamState.Count; i++)
        {
            Debug.Log("Positon Update:"+GameHistory.recentState.redTeamState[i].position[0]+"+"+GameHistory.recentState.redTeamState[i].position[1] + "+"+ GameHistory.recentState.redTeamState[i].position[2]);
            GameData.redTeamData.clientsData[GameHistory.recentState.redTeamState[i].playerId].character.GetComponent<CharacterSyncScript>().NewPlayerState(GameHistory.recentState.redTeamState[i]);
        }
        for (int i = 0; i < GameHistory.recentState.blueTeamState.Count; i++)
        {
            GameData.blueTeamData.clientsData[GameHistory.recentState.blueTeamState[i].playerId].character.GetComponent<CharacterSyncScript>().NewPlayerState(GameHistory.recentState.blueTeamState[i]);
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
        yield return new WaitForSeconds(10);

        StartTick();
        InitializeFirstGameState();
        server.StartGame();
    }

    void InitializeFirstGameState()
    {
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
            state.blueTeamState.Add(clientState);
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
            state.redTeamState.Add(clientState);
        }
        GameHistory.AddGameState(state, 0);
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
    private List<EndPoint> endPoints = new List<EndPoint>();

    private Socket udpServer;
    private Dictionary<EndPoint, string> udpEndPoints;

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

        udpEndPoints = new Dictionary<EndPoint, string>();
        /*IPHostEntry host = Dns.Resolve(Dns.GetHostName());
        IPAddress ip = host.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ip, this.gamelift.listeningPort);
        udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpServer.Bind(endPoint);
        udpServer.Blocking = false;*/

        udpListener = new UdpClient(this.gamelift.listeningPort);
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
            Debug.Log("Recieved UDP msg:"+msgPacket.type);
            HandleUdpMsg(msgPacket, clientEndPoint);            

            if (data.Length < 4)
            {
                return;
            }

            Debug.Log(msgPacket.message);

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
        this.endPoints = new List<EndPoint> { };
        this.udpEndPoints = new Dictionary<EndPoint, string> { };
	}

    //Transmit message to multiple clients
	private void TransmitMessage(SimpleMessage msg, TcpClient excludeClient = null)
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
    }

    private void HandleUdpConnect(IPEndPoint clientEndPoint,string playerId,string team)
    {
        if (!udpEndPoints.ContainsKey(clientEndPoint))
        {
            udpEndPoints.Add(clientEndPoint, playerId);
            if (team == "red")
            {
                GameData.redTeamData.clientsData[playerId].udpEndPoint = clientEndPoint;
            }
            else if (team == "blue")
            {
                GameData.redTeamData.clientsData[playerId].udpEndPoint = clientEndPoint;
            }
        }
    }

    private void HandleGameState(UdpMsgPacket packet)
    {

    }

    private void HandleClientState(UdpMsgPacket packet)
    {
        //if(packet.clientState.tick>)
        int us = GameHistory.CheckClientState(packet.clientState);
        Debug.Log(us);
        Debug.Log(packet.clientState.position[0] + "," + packet.clientState.position[1] + "," + packet.clientState.position[2]);
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
            string id=GameData.blueTeamData.AddNewClient(client);
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerData, "");
            msg.playerId = id;
            msg.team = team;
            this.SendMessage(client, msg);
        }
        else if (team == "red")
        {
            Debug.Log("red");
            string id = GameData.redTeamData.AddNewClient(client);
            Debug.Log("client added to game data");
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerData, "");
            msg.playerId = id;
            msg.team = team;
            this.SendMessage(client, msg);
        }
        Debug.Log("Player Data Sent");
        if (readyClients.Count == 1) //players2 for temp
        {
            Debug.Log("Enough clients, let's start the game!");
            MazeCell[,] maze=MazeGenerator.generateMaze();
            Debug.Log("Maze Data Generated");
            GameData.maze = maze;
            Debug.Log("Maze Data Assigned");
            GameObject.Find("MazeController").GetComponent<MazeController>().InstantiateMaze(maze);
            Debug.Log("Maze Created");
            List<object> list=MazeGenerator.ToObjectList(maze);
            SimpleMessage msg = new SimpleMessage(MessageType.GameReady, "");
            msg.listData = list;
            TransmitMessage(msg);
            //server.StartTick();
            this.gamelift.ReadyGame();
            server.GameReady();
            //Initialize all team positions, team bases, container positions
            InitializePlayerGameData();
        }
    }

    void InitializePlayerGameData()
    {
        Debug.Log("InitializePlayerGameData Called");
        GameData.InitializeSpawns();
        Debug.Log("Spawn Initialized");
        foreach(KeyValuePair<string,ClientData> clientData in GameData.blueTeamData.clientsData)
        {
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerGameData);
            msg.playerId = clientData.Key;
            float[] sp=SpawnManager.GetSpawnPos("blue");
            msg.floatArrData = sp;
            TcpClient tcpClient=clientData.Value.tcpClient;
            GameObject character=GameObject.Instantiate(server.characterPrefab, new Vector3(sp[0], sp[1], sp[2]), Quaternion.identity);
            clientData.Value.character = character;
            GameData.blueTeamData.clientsData[clientData.Value.playerId] = clientData.Value;
            SendMessage(tcpClient, msg);
        }
        Debug.Log("blue team spawn message sent");
        foreach (KeyValuePair<string, ClientData> clientData in GameData.redTeamData.clientsData)
        {
            Debug.Log("red team client");
            SimpleMessage msg = new SimpleMessage(MessageType.PlayerGameData);
            msg.playerId = clientData.Key;
            float[] sp = SpawnManager.GetSpawnPos("red");
            Debug.Log("Player spawn created");                                                                 
            msg.floatArrData = sp;
            TcpClient tcpClient = clientData.Value.tcpClient;
            GameObject character = GameObject.Instantiate(server.characterPrefab, new Vector3(sp[0], sp[1], sp[2]), Quaternion.identity);
            //clientData.Value.character = character;
            GameData.redTeamData.clientsData[clientData.Value.playerId].character=character;
            SendMessage(tcpClient, msg);
            Debug.Log("Player SpawnMessage Sent");
        }
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

    void SendPacket(UdpMsgPacket msgPacket, IPEndPoint addr)
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
