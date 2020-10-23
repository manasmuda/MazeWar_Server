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

    public bool crouch;

    public String name;

    public int health;
    public int coinsHolding;
    public int tracersRemaining;
    public int bulletsLeft;

    public bool bulletHit;
    public string bulletHitId;
    public float[] bulletHitPosition;

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
        crouch = state.crouch;
    }

    public ClientState CompareAndCopy(ClientState newState)
    {
        playerId = newState.playerId;
        team = newState.team;
        tick = newState.tick;
        name = newState.name;
        position[0] = newState.position[0];
        position[1] = newState.position[1];
        position[2] = newState.position[2];
        angle[0] = newState.angle[0];
        angle[1] = newState.angle[1];
        angle[2] = newState.angle[2];
        stateType = newState.stateType;
        movementPressed = newState.movementPressed;
        crouch = newState.crouch;
        if (health > newState.health)
        {
            health = newState.health;
        }
        if (coinsHolding < newState.coinsHolding)
        {
            coinsHolding = newState.coinsHolding;
        }
        if (tracersRemaining > newState.tracersRemaining)
        {
            tracersRemaining = newState.tracersRemaining;
        }
        if (bulletsLeft > newState.bulletsLeft)
        {
            bulletsLeft = newState.bulletsLeft;
        }
        reloadStartTick = newState.reloadStartTick;
        
        return this;
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
        if (Math.Abs(angle[0] - state.angle[0]) > 10)
        {
            return true;
        }
        if (Math.Abs(angle[0] - state.angle[0]) > 10)
        {
            return true;
        }
        return false;
    }
}

