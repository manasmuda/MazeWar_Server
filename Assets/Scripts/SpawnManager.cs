using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager { 
    

    public static float[] GetSpawnPos(string team)
    {
        List<int> blueBases = new List<int>();
        List<int> redBases = new List<int>();
        float[] x = new float[3];
        for(int i=0; i < 19; i++)
        {
            if(!GameData.maze[0,i].eastWall && !GameData.maze[0, i + 1].westWall)
            {
                redBases.Add(i + 1);
            }
            if (!GameData.maze[19, i].eastWall && !GameData.maze[19, i + 1].westWall)
            {
                redBases.Add(i + 1);
            }
        }
        if (team == "blue")
        {
            int p = Random.Range(0, blueBases.Count - 1);
            int i = 19;
            int j = blueBases[p];
            x[0] = 6 * j - 57;
            x[1] = 2.0f;
            x[2]= 57 - 6 * i;
        }
        else
        {
            int p = Random.Range(0, redBases.Count - 1);
            int i = 0;
            int j = redBases[p];
            x[0] = 6 * j - 57;
            x[1] = 2.0f;
            x[2] = 57 - 6 * i;
        }
        return x;
    }
}
