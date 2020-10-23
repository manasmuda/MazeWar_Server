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
            //Debug.Log("Prediction creation started");
            GameState state=new GameState(recentState);
            state.tick = tick;
            /*for(int i = 0; i < state.redTeamState.Count; i++)
            {
                if (state.redTeamState[i].movementPressed)
                {
                    state.redTeamState[i].movementPressed = false;
                    float ay=state.redTeamState[i].angle[1];
                    float dist = 15 * 0.2f;
                    state.redTeamState[i].position[0] = state.redTeamState[i].position[0] + dist * Mathf.Cos(ay);
                    state.redTeamState[i].position[2] = state.redTeamState[i].position[2] + dist * Mathf.Sin(ay);
                }
            }
            for (int i = 0; i < state.blueTeamState.Count; i++)
            {
                if (state.blueTeamState[i].movementPressed)
                {
                    state.blueTeamState[i].movementPressed = false;
                    float ay = state.blueTeamState[i].angle[1];
                    float dist = 15 * 0.2f;
                    state.blueTeamState[i].position[0] = state.blueTeamState[i].position[0] + dist * Mathf.Cos(ay);
                    state.blueTeamState[i].position[2] = state.blueTeamState[i].position[2] + dist * Mathf.Sin(ay);
                }
            }*/
            AddGameState(state, tick);
            //Debug.Log("Prediction State Created");
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
            //Debug.Log("Game State Added");

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
            //Debug.Log("tick matched");
            if (state.team == "blue")
            {
                ClientState tempState = new ClientState(recentState.blueTeamState[state.playerId]);
                tempState.CompareAndCopy(state);
                recentState.blueTeamState[state.playerId] = tempState;
            }
            else
            {
                ClientState tempState = new ClientState(recentState.redTeamState[state.playerId]);
                tempState.CompareAndCopy(state);
                recentState.redTeamState[state.playerId] = tempState;
            }
            return 2;

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
                        if (gsn.Value.blueTeamState[state.playerId].CompareState(state))
                        {
                            ClientState tempState = new ClientState(gsn.Value.blueTeamState[state.playerId]);
                            tempState.CompareAndCopy(state);
                            /*if (tempState.movementPressed)
                            {
                                tempState.movementPressed = false;
                                float ay = tempState.angle[1];
                                float dist = 15 * 0.2f;
                                tempState.position[0] = tempState.position[0] + dist * Mathf.Cos(ay);
                                tempState.position[2] = tempState.position[2] + dist * Mathf.Sin(ay);
                            }*/
                            recentState.blueTeamState[tempState.playerId] = tempState;
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    else
                    {
                        if (gsn.Value.redTeamState[state.playerId].CompareState(state))
                        {
                            ClientState tempState = new ClientState(gsn.Value.redTeamState[state.playerId]);
                            tempState.CompareAndCopy(state);
                            /*if (tempState.movementPressed)
                            {
                                tempState.movementPressed = false;
                                float ay = tempState.angle[1];
                                float dist = 15 * 0.2f;
                                tempState.position[0] = tempState.position[0] + dist * Mathf.Cos(ay);
                                tempState.position[2] = tempState.position[2] + dist * Mathf.Sin(ay);
                            }*/
                            recentState.redTeamState[tempState.playerId] = tempState;
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    
                }
                gsn = gsn.Next;
                gtn = gtn.Next;
            }
        }
        return 5;
    }

    public static LinkedListNode<GameState> GetGameState(int tick)
    {
        if (tick > recentTick - 10 && tick<recentTick)
        {
            bool found = false;
            LinkedListNode<GameState> cn = gameStates.Last;
            while (cn != null)
            {
                if (cn.Value.tick == tick)
                {
                    found = true;
                    break;
                }
                cn = cn.Previous;
            }
            if (found)
            {
                return cn;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
