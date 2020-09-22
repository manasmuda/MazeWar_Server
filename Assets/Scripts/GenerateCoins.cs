using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCoins
{
    public static Vector3[] GetCoinPosition()
    {
        MazeCell[,] m = new MazeCell[20, 20];
        int coinCount = 15;
        Vector3[] positionToInstantiate= new Vector3[coinCount];
        Vector3 previousPos = new Vector3();
        float xPos=0;
        float zPos=0;
        float yPos=0;

        float distance=5f;


        for (int i = 0; i < coinCount; i++)
        {
            // should be random bw 2,0 and 17,19 of maze cells
            xPos = Random.Range(m[2, 0].row, m[17, 0].row);
            zPos = Random.Range(m[0, 2].col, m[0, 19].col);
            yPos = 1;

            positionToInstantiate[i] = new Vector3(xPos, yPos, zPos);

            while (Vector3.Distance(positionToInstantiate[i],previousPos)<distance)
            {
                xPos = Random.Range(m[2, 0].row, m[17, 0].row);
                zPos = Random.Range(m[0, 2].col, m[0, 19].col);
                yPos = 1;

            }

            positionToInstantiate[i].x = xPos;
            positionToInstantiate[i].y = yPos;
            positionToInstantiate[i].z = zPos;

            previousPos = positionToInstantiate[i];
        }

        return positionToInstantiate;
    }

   


}
