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
        return recentState;
    }
    
    public static void AddGameState(GameState state,int tick)
    {
        if (tick > recentTick)
        {
            if (size != gameStates.Count)
            {
                gameStates.RemoveFirst();
            }
            gameStates.AddFirst(state);
        }
    }

    public static int CheckClientState(ClientState state)
    {
        if (state.tick < recentTick - size)
        {
            return -1;
        }
        else
        {
            return -2;
        }
    }
}
