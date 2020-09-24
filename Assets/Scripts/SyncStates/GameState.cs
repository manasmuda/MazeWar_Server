using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameState 
{
    public int tick;
    public int stateType;

    public List<ClientState> blueTeamState = new List<ClientState>();
    public List<ClientState> redTeamState = new List<ClientState>();

    public GameState()
    {
        
    }

    public GameState(GameState gameState)
    {
        this.tick = gameState.tick;
        this.stateType = gameState.stateType;
        for(int i = 0; i < gameState.blueTeamState.Count; i++)
        {
            this.blueTeamState.Add(new ClientState(gameState.blueTeamState[i]));
        }
        for (int i = 0; i < gameState.redTeamState.Count; i++)
        {
            this.redTeamState.Add(new ClientState(gameState.redTeamState[i]));
        }
    }

    public bool CompareState(GameState state)
    {
        for (int i = 0; i < state.blueTeamState.Count; i++)
        {
            if (this.blueTeamState[i].CompareState(state.blueTeamState[i]))
            {
                return true;
            }
        }
        for (int i = 0; i < state.redTeamState.Count; i++)
        {
            if (this.redTeamState[i].CompareState(state.redTeamState[i]))
            {
                return true;
            }
        }
        return false;
    }
}
