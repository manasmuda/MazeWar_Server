using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHistory 
{

    public static LinkedList<GameState> gameStates = new LinkedList<GameState>();
    public static LinkedList<int> gameStateTicks = new LinkedList<int>();

    public static GameState recentState;
    public static int recentTick;

    public static int size = 10;

    public static GameState GetPredictedState(int tick)
    {
        if (tick > recentTick)
        {
            Debug.Log("Prediction creation started");
            GameState state=new GameState(recentState);
            state.tick = tick;
            for(int i = 0; i < state.redTeamState.Count; i++)
            {
                if (state.redTeamState[i].movementPressed)
                {
                    state.redTeamState[i].movementPressed = false;
                    float ay=state.redTeamState[i].angle[1];
                    float dist = 150 * 0.2f;
                    state.redTeamState[i].position[0] = state.redTeamState[i].position[0] + dist * Mathf.Sin(ay);
                    state.redTeamState[i].position[2] = state.redTeamState[i].position[2] + dist * Mathf.Cos(ay);
                }
            }
            for (int i = 0; i < state.blueTeamState.Count; i++)
            {
                if (state.blueTeamState[i].movementPressed)
                {
                    state.blueTeamState[i].movementPressed = false;
                    float ay = state.blueTeamState[i].angle[1];
                    float dist = 150 * 0.2f;
                    state.blueTeamState[i].position[0] = state.blueTeamState[i].position[0] + dist * Mathf.Sin(ay);
                    state.blueTeamState[i].position[2] = state.blueTeamState[i].position[2] + dist * Mathf.Cos(ay);
                }
            }
            AddGameState(state, tick);
            Debug.Log("Prediction State Created");
        }
        return recentState;
    }
    
    public static void AddGameState(GameState state,int tick)
    {
            if (size == gameStates.Count)
            {
                gameStates.RemoveFirst();
                gameStateTicks.RemoveFirst();
            }
            gameStates.AddLast(state);
            gameStateTicks.AddLast(tick);
            recentState = state;
            recentTick = tick;
            Debug.Log("Game State Added");

    }

    public static int CheckClientState(ClientState state)
    {
        Debug.Log(state.playerId+":"+state.tick + ":" + recentTick);
        if (state.tick < recentTick - size)
        {
            return -1;
        }
        else if (state.tick >= recentTick)
        {
            Debug.Log("tick matched");
            if (state.team == "blue")
            {
                for (int i = 0; i < recentState.blueTeamState.Count; i++)
                {
                    if (recentState.blueTeamState[i].playerId == state.playerId)
                    {
                        recentState.blueTeamState[i] = new ClientState(state);
                        return 2;
                        /*if (recentState.blueTeamState[i].CompareState(state))
                        {
                            recentState.blueTeamState[i] = new ClientState(state);
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }*/
                    }
                }
            }
            else
            {
                Debug.Log("team matched");
                for (int i = 0; i < recentState.redTeamState.Count; i++)
                {
                    if (recentState.redTeamState[i].playerId == state.playerId)
                    {
                        recentState.redTeamState[i] = new ClientState(state);
                        return 2;
                        /*if (recentState.redTeamState[i].CompareState(state))
                        {
                            recentState.redTeamState[i] = new ClientState(state);
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }*/
                    }
                }
            }

        }
        else
        {
            LinkedListNode<GameState> gsn = gameStates.First;
            LinkedListNode<int> gtn = gameStateTicks.First;
            while(gsn!=null && gtn != null)
            {
                if (gtn.Value == state.tick)
                {
                    if (state.team == "blue")
                    {
                        for (int i = 0; i < gsn.Value.blueTeamState.Count; i++)
                        {
                            if (gsn.Value.blueTeamState[i].playerId == state.playerId)
                            {
                                if(gsn.Value.blueTeamState[i].CompareState(state)){
                                    ClientState tempState= new ClientState(state);
                                    if (tempState.movementPressed)
                                    {
                                        tempState.movementPressed = false;
                                        float ay = tempState.angle[1];
                                        float dist = 150 * 0.2f;
                                        tempState.position[0] = tempState.position[0] + dist * Mathf.Sin(ay);
                                        tempState.position[2] = tempState.position[2] + dist * Mathf.Cos(ay);
                                    }
                                    recentState.blueTeamState[i] = tempState;
                                    return 3;
                                }
                                else
                                {
                                    return 4;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < gsn.Value.redTeamState.Count; i++)
                        {
                            if (gsn.Value.redTeamState[i].playerId == state.playerId)
                            {
                                if (gsn.Value.redTeamState[i].CompareState(state))
                                {
                                    ClientState tempState = new ClientState(state);
                                    if (tempState.movementPressed)
                                    {
                                        tempState.movementPressed = false;
                                        float ay = tempState.angle[1];
                                        float dist = 150 * 0.2f;
                                        tempState.position[0] = tempState.position[0] + dist * Mathf.Sin(ay);
                                        tempState.position[2] = tempState.position[2] + dist * Mathf.Cos(ay);
                                    }
                                    
                                    recentState.redTeamState[i] = tempState;
                                    return 3;
                                }
                                else
                                {
                                    return 4;
                                }
                            }
                        }
                    }
                    
                }
                gsn = gsn.Next;
                gtn = gtn.Next;
            }
        }
        return 5;
    }
}
