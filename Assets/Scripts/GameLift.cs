﻿using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Server;

public class GameLift : MonoBehaviour
{

    public int listeningPort = -1;

    // Game preparation state
    private bool gameSessionInfoReceived = false;
    private float waitingForPlayerTime = 0.0f;

    // Game state
    private bool gameStarted = false;
    private bool gameReady = false;
    private string gameSessionId;
    public string GetGameSessionID() { return gameSessionId; }

    // StatsD client for sending custom metrics to CloudWatch through the local StatsD agent
    

    // Get the port to host the server from the command line arguments
    private int GetPortFromArgs()
    {
        int defaultPort = 1935;
        int port = defaultPort; //Use default is arg not provided

        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log("ARG " + i + ": " + args[i]);
            if (args[i] == "-port")
            {
                port = int.Parse(args[i + 1]);
            }
        }

        return port;
    }

    // Called when the monobehaviour is created
    public void Awake()
    {
        //Initiate the simple statsD client

        //Get the port from command line args
        listeningPort = this.GetPortFromArgs();

        Debug.Log("Will be running in port: " + this.listeningPort);

        //InitSDK establishes a local connection with the Amazon GameLift agent to enable 
        //further communication.
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if (initSDKOutcome.Success)
        {
            ProcessParameters processParameters = new ProcessParameters(
                (gameSession) => {
                    //Respond to new game session activation request. GameLift sends activation request 
                    //to the game server along with a game session object containing game properties 
                    //and other settings.

                    // Activate the session
                    GameLiftServerAPI.ActivateGameSession();

                    //Start waiting for players
                    this.gameSessionInfoReceived = true;
                    this.gameSessionId = gameSession.GameSessionId;

                    Debug.Log("Game Session created ID:" +gameSession.GameSessionId);
                    Debug.Log("Game Session:" + gameSession);

                    //Set the game session tag (CloudWatch dimension) for custom metrics
                    string justSessionId = this.gameSessionId.Split('/')[2];

                   
                },
                () => {
                    //OnProcessTerminate callback. GameLift invokes this callback before shutting down 
                    //an instance hosting this game server. It gives this game server a chance to save
                    //its state, communicate with services, etc., before being shut down. 
                    //In this case, we simply tell GameLift we are indeed going to shut down.
                    GameLiftServerAPI.ProcessEnding();
                    Application.Quit();
                },
                () => {
                    //This is the HealthCheck callback.
                    //GameLift invokes this callback every 60 seconds or so.
                    //Here, a game server might want to check the health of dependencies and such.
                    //Simply return true if healthy, false otherwise.
                    //The game server has 60 seconds to respond with its health status. 
                    //GameLift will default to 'false' if the game server doesn't respond in time.
                    //In this case, we're always healthy!
                    return true;
                },
                //Here, the game server tells GameLift what port it is listening on for incoming player 
                //connections. We will use the port received from command line arguments
                listeningPort,
                new LogParameters(new List<string>()
                {
                    //Let GameLift know where our logs are stored. We are expecting the command line args to specify the server with the port in log file
                    "/local/game/logs/myserver"+listeningPort+".log"
                }));

            //Calling ProcessReady tells GameLift this game server is ready to receive incoming game sessions
            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if (processReadyOutcome.Success)
            {
                print("ProcessReady success.");
            }
            else
            {
                print("ProcessReady failure : " + processReadyOutcome.Error.ToString());
            }
        }
        else
        {
            print("InitSDK failure : " + initSDKOutcome.Error.ToString());
        }
    }

    // Ends the game session for all and disconnects the players
    public void TerminateGameSession()
    {
        Debug.Log("Terminating Game Session");
        GameObject.FindObjectOfType<Server>().DisconnectAll();
        GameLiftServerAPI.TerminateGameSession();
        this.gameSessionInfoReceived = false;
        this.gameStarted = false;
        this.gameReady = false;
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        this.gameStarted = true;
    }

    public void ReadyGame()
    {
        Debug.Log("Game Ready");
        this.gameReady = true;
    }

    public bool GameStarted()
    {
        return this.gameStarted;
    }

    public bool GameReady()
    {
        return this.gameReady;
    }
    // Called by Unity once a frame
    public void Update()
    {
        // Wait for players to join for 5 seconds max
        
        if (this.gameSessionInfoReceived && !this.gameReady)
        {
            this.waitingForPlayerTime += Time.deltaTime;
            //Debug.Log(waitingForPlayerTime);
            if (this.waitingForPlayerTime > 120.0f)
            {
                Debug.Log("No players in 60 seconds from starting the game, terminate game session");
                this.waitingForPlayerTime = 0.0f;
                this.gameSessionInfoReceived = false;
                TerminateGameSession();
            }
        }
    }

    void OnApplicationQuit()
    {
        //Make sure to call GameLiftServerAPI.Destroy() when the application quits. 
        //This resets the local connection with GameLift's agent.
        GameLiftServerAPI.Destroy();
    }

}
