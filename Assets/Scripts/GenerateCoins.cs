using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateCoins
{
    public static Vector3[] GetCoinPosition()
    {
        int coinCount = 15;
        Vector3[] positionToInstantiate= new Vector3[coinCount];

        for (int i = 0; i < coinCount; i++)
        {
            float xPos = UnityEngine.Random.Range(60f, -50f);
            float zPos = UnityEngine.Random.Range(-50f, 60f);
            float yPos = 1;

            positionToInstantiate[i] = new Vector3(xPos, yPos, zPos);

        }

        return positionToInstantiate;
    }

   


}
