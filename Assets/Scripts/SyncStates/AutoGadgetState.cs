using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AutoGadgetState
{
    public int tick = 0;

    public string team;

    public string name;
    public string id;

    public int health;
    public float[] position;
    public float[] angle;

    public AutoGadgetState()
    {

    }

    public AutoGadgetState(AutoGadgetState autoGadgetState)
    {
        tick = autoGadgetState.tick;
        name = autoGadgetState.name;
        team = autoGadgetState.team;
        id = autoGadgetState.id;
        health = autoGadgetState.health;
        position = new float[3] { autoGadgetState.position[0], autoGadgetState.position[1], autoGadgetState.position[2]};
        angle = new float[3] { autoGadgetState.angle[0], autoGadgetState.angle[1], autoGadgetState.angle[2] };
    }

    public void CompareAndCopy(AutoGadgetState newState)
    {
        tick = newState.tick;
        name = newState.name;
        team = newState.team;
        id = newState.id;
        position = new float[3] { newState.position[0], newState.position[1], newState.position[2] };
        angle = new float[3] { newState.angle[0], newState.angle[1], newState.angle[2] };
        if (health > newState.health)
        {
            health = newState.health;
        }
    }
}
