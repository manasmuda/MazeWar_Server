using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager { 
    

    public static float[] GetSpawnPos(string team)
    {
       
        float[] x = new float[3];
        //Change spawn manager
        if (team == "blue")
        {
            int i = GameData.blueTeamOrigin/20;
            int j = GameData.blueTeamOrigin % 20;
            x[0] = 6 * j - 57;
            x[1] = 0.5f;
            x[2]= 57 - 6 * i;
        }
        else
        {
            int i = GameData.redTeamOrigin / 20;
            int j = GameData.redTeamOrigin % 20;
            x[0] = 6 * j - 57;
            x[1] = 0.5f;
            x[2] = 57 - 6 * i;
        }
        return x;
    }
}
