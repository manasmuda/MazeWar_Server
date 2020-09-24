using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell 
{
    public bool northWall;
    public bool southWall;
    public bool eastWall;
    public bool westWall;

    public int row, col;

    public GameObject northWallObj;
    public GameObject southWallObj;
    public GameObject eastWallObj;
    public GameObject westWallObj;

    public MazeCell(int row, int col)
    {
        northWall = true;
        southWall = true;
        eastWall = true;
        westWall = true;
        this.row = row;
        this.col = col;
    }
}
