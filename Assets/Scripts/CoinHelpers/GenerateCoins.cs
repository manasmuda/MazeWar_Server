using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateCoins
{

    static int coinCount = 15;
    static Vector3[] positionToInstantiate = new Vector3[coinCount];
    static int[] coinPos = new int[coinCount];
    static List<int> wrongPos = new List<int>();

    public static Vector3[] GetCoinPosition()
    {
        positionToInstantiate = new Vector3[coinCount];
        coinPos = new int[coinCount];
        wrongPos = new List<int>();
        

        for (int x = 0; x < coinCount; x++)
        {
            int i = -1;
            int j = -1;

            int cellNo = -1;

            int ncellNo = -1;
            int scellNo = -1;
            int wcellNo = -1;
            int ecellNo = -1;

            bool found = false;
            do
            {
                // should be random bw 2,0 and 17,19 of maze cells
                i = Random.Range(2, 17);
                j = Random.Range(0, 19);


                cellNo = 20 * i + j;

                

                if (i > 2)
                {
                    ncellNo = (20 * (i - 1)) + j;
                }
                if (i < 17)
                {
                    scellNo = (20 * (i + 1)) + j;
                }
                if (j > 0)
                {
                    wcellNo = 20 * i + (j - 1);
                }
                if (j < 19)
                {
                    ecellNo = 20 * i + (j + 1);
                }

                if (!wrongPos.Contains(cellNo))
                {
                    coinPos[x] = cellNo;
                    wrongPos.Add(ncellNo);
                    wrongPos.Add(scellNo);
                    wrongPos.Add(wcellNo);
                    wrongPos.Add(ecellNo);
                    found = true;
                }
            } while (!found);
            positionToInstantiate[x] = new Vector3(6 * j - 57,2f, 57 - 6 * i);
        }

        return positionToInstantiate;
    }

   public static void InstantiateCoins(GameObject coinPrefab,Vector3[] coinPos)
    {
        GameObject coinsParent = new GameObject("CoinsParent");
        coinsParent.transform.position = Vector3.zero;
        GameObject[] coinObjects = new GameObject[coinCount];
        for (int i = 0; i < coinPos.Length; i++) {
            coinObjects[i]= GameObject.Instantiate(coinPrefab,coinPos[i],Quaternion.identity,coinsParent.transform);
        }
        GameData.coinObjects = coinObjects;
    }

}
