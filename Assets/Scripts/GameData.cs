using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int cellSize = 20;
    public static MazeCell[,] maze = new MazeCell[20, 20];
    public static GameObject[] coinObjects;


    public static TeamData blueTeamData = new TeamData("blue");
    public static TeamData redTeamData = new TeamData("red");

    public static int blueTeamOrigin = -1;  //Identifies player origin box (left cell of player base)
    public static int redTeamOrigin = -1;

    public static void InitializeSpawns()
    {
        List<int> redpoints = new List<int> { };
        List<int> bluepoints = new List<int> { };
        for(int i = 0; i < cellSize-1; i++)
        {
            if(!maze[0,i].eastWall && !maze[0, i + 1].westWall)
            {
                redpoints.Add(i + 1);
            }

            if (!maze[cellSize-1, i].eastWall && !maze[cellSize-1, i + 1].westWall)
            {
                bluepoints.Add(i);
            }
        }

        int redPos = Random.Range(0, redpoints.Count - 1);
        int bluePos = Random.Range(0, bluepoints.Count - 1);

        blueTeamOrigin = cellSize * (cellSize - 1) + bluepoints[bluePos];
        redTeamOrigin = redpoints[redPos];
    }
}
