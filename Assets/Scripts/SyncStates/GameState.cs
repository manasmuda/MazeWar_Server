using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameState 
{
    public int tick;
    public int stateType;

    public Dictionary<string,ClientState> blueTeamState = new Dictionary<string, ClientState>();
    public Dictionary<string,ClientState> redTeamState = new Dictionary<string, ClientState>();

    public GameState()
    {
        
    }

    public GameState(GameState gameState)
    {
        this.tick = gameState.tick;
        this.stateType = gameState.stateType;
        foreach(ClientState state in gameState.blueTeamState.Values)
        {
            this.blueTeamState.Add(state.playerId,new ClientState(state));
        }
        foreach (ClientState state in gameState.redTeamState.Values)
        {
            this.redTeamState.Add(state.playerId, new ClientState(state));
        }
    }

    public bool CompareState(GameState state)
    {
        foreach(ClientState clientState in state.blueTeamState.Values)
        {
            if (this.blueTeamState[clientState.playerId].CompareState(clientState))
            {
                return true;
            }
        }
        foreach (ClientState clientState in state.redTeamState.Values)
        {
            if (this.redTeamState[clientState.playerId].CompareState(clientState))
            {
                return true;
            }
        }
        return false;
    }
}
