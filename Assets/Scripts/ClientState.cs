using System;
using System.Collections.Generic;

[Serializable]
public class ClientState
{
    public int tick = 0;

    public string playerId;
    public string team;

    public float[] position = new float[3];
    public float[] angle = new float[3];

    public String name;

    public int health;
    public int coinsHolding;
    public int tracersRemaining;
    public int bulletsLeft;

    public int reloadStartTick;

    public int stateType;
    public bool movementPressed;

    public ClientState()
    {

    }

    public ClientState(ClientState state)
    {
        playerId = state.playerId;
        team = state.team;
        tick = state.tick;
        position[0] = state.position[0];
        position[1] = state.position[1];
        position[2] = state.position[2];
        angle[0] = state.angle[0];
        angle[1] = state.angle[1];
        angle[2] = state.angle[2];
        name = state.name;
        health = state.health;
        coinsHolding = state.coinsHolding;
        tracersRemaining = state.tracersRemaining;
        bulletsLeft = state.bulletsLeft;
        reloadStartTick = state.reloadStartTick;
        stateType = state.stateType;
        movementPressed = state.movementPressed;
    }

    public bool CompareState(ClientState state)
    {
        if (Math.Abs(position[0] - state.position[0]) > 0.1)
        {
            return true;
        }
        if (Math.Abs(position[1] - state.position[1]) > 0.1)
        {
            return true;
        }
        if (Math.Abs(position[2] - state.position[2]) > 0.1)
        {
            return true;
        }
        if (Math.Abs(angle[0] - state.angle[0]) > 10)
        {
            return true;
        }
        if (Math.Abs(angle[0] - state.angle[0]) > 0.3)
        {
            return true;
        }
        if (Math.Abs(angle[0] - state.angle[0]) > 0.3)
        {
            return true;
        }
        return false;
    }
}

