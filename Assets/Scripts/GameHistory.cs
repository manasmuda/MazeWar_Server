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
            AddGameState(new GameState(recentState), tick);
        }
        return recentState;
    }
    
    public static void AddGameState(GameState state,int tick)
    {
        if (tick > recentTick)
        {
            if (size != gameStates.Count)
            {
                gameStates.RemoveFirst();
                gameStateTicks.RemoveFirst();
            }
            gameStates.AddLast(state);
            gameStateTicks.AddLast(tick);
            recentState = state;
            recentTick = tick;
        }

    }

    public static int CheckClientState(ClientState state)
    {
        if (state.tick < recentTick - size)
        {
            return -1;
        }
        else if (state.tick == recentTick)
        {
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
                                    gameStates.Last.Value.blueTeamState[i] = new ClientState(state);
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
                                    gameStates.Last.Value.redTeamState[i] = new ClientState(state);
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
