using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinValues 
{

    //public float xPos, yPos, zPos;

    public CoinValues(float xPos,float yPos,float zPos)
    {

        float x = Random.Range(60f, -50f);
        float z = Random.Range(-50f, 60f);
        float y = 1f;

        x = xPos;
        y = yPos;
        z = zPos;
    }

}
