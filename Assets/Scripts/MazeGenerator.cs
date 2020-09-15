using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGenerator 
{

    private static MazeCell[,] randMaze;
    private static bool[,] visited;

    private static bool mazeComplete = false;

    private static int currentRow = 0;
    private static int currentColumn=0;

    private static int cellSize = 20;

    public static MazeCell[,] generateMaze()
    {
        randMaze = new MazeCell[cellSize,cellSize];
        visited = new bool[cellSize,cellSize];

        for(int i = 0; i < cellSize; i++)
        {
            for(int j = 0; j < cellSize; j++)
            {
                randMaze[i,j] = new MazeCell(i,j);
            }
        }

        mazeComplete = false;

        while (!mazeComplete)
        {
           // Debug.Log(currentRow);
            //Debug.Log(currentColumn);
            CreatePath();
            SearchPath();
        }
        Debug.Log("Basic Maze Created");
        for(int i = 0; i < cellSize; i++)
        {
            for(int j = 0; j < cellSize; j++)
            {
                int direction = Random.Range(1, 10);
                if (direction > 4)
                    continue;
                if(direction==1 && i > 0)
                {
                    randMaze[i, j].northWall = false;
                    randMaze[i - 1, j].southWall = false;
                }
                else if(direction==2 && i < cellSize-1)
                {
                    randMaze[i, j].southWall = false;
                    randMaze[i + 1, j].northWall = false;
                }
                else if(direction==3 && j < cellSize-1)
                {
                    randMaze[i, j].eastWall = false;
                    randMaze[i, j + 1].westWall = false;
                }
                else if(direction==4 && j > 0)
                {
                    randMaze[i, j].westWall = false;
                    randMaze[i, j - 1].eastWall = false;
                }

            }
        }
        Debug.Log("Modified Maze Created");
        bool redBase = false;
        bool blueBase = false;
        for(int i = 0; i < cellSize-1; i++)
        {
            if (!redBase)
            {
                if (!randMaze[0, i].eastWall && !randMaze[0, i + 1].westWall)
                {
                    redBase = true;
                }
            }
            if (!blueBase)
            {
                if (!randMaze[cellSize-1, i].eastWall && !randMaze[cellSize-1, i + 1].westWall)
                {
                    blueBase = true;
                }
            }
            if(redBase && blueBase)
            {
                break;
            }
        }
        Debug.Log("Maze Base Checked");
        if (!redBase)
        {
            randMaze[0, cellSize - 2].eastWall = false;
            randMaze[0, cellSize - 1].westWall = false;
        }
        if (!blueBase)
        {
            randMaze[cellSize-1, 0].eastWall = false;
            randMaze[cellSize-1, 1].westWall = false;
        }

        Debug.Log("Maze Generation finished");

        return randMaze;
    }

    private static void SearchPath()
    {
        mazeComplete = true;
        for(int i = 0; i < cellSize; i++)
        {
            for(int j = 0; j < cellSize; j++)
            {
                if(!visited[i,j] && HasAdjVisited(i,j))
                {
                    mazeComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    currentRow = i;
                    currentColumn = j;
                    DestroyAdjacentWall(currentRow, currentColumn);
                    visited[currentRow, currentColumn] = true;
                    return;
                }
            }
        }
    }

    private static void CreatePath()
    {
        //List<int> checkedDirections = new List<int> { };
        while (RouteStillAvailable(currentRow, currentColumn))
        {
            int direction = Random.Range (1, 5);
            //while (checkedDirections.Contains(direction))
            //{
            //  direction = Random.Range(1, 5);
            //}
            //checkedDirections.Add(direction);
            //int direction = ProceduralNumberGenerator.GetNextNumber();
            
            if (direction == 1 && CellIsAvailable(currentRow-1,currentColumn))
            {
                // North
                randMaze[currentRow, currentColumn].northWall = false;
                randMaze[currentRow - 1, currentColumn].southWall=false;
                //visited[currentRow, currentColumn] = true;
                currentRow--;
                //checkedDirections = new List<int> { };
            }
            else if (direction == 2 && CellIsAvailable(currentRow+1,currentColumn))
            {
                // South
                randMaze[currentRow, currentColumn].southWall=false;
                randMaze[currentRow + 1, currentColumn].northWall=false;
                //visited[currentRow, currentColumn] = true;
                currentRow++;
                //checkedDirections = new List<int> { };
            }
            else if (direction == 3 && CellIsAvailable(currentRow,currentColumn+1))
            {
                // east
                randMaze[currentRow, currentColumn].eastWall=false;
                randMaze[currentRow, currentColumn + 1].westWall=false;
                //visited[currentRow, currentColumn] = true;
                currentColumn++;
                //checkedDirections = new List<int> { };
            }
            else if (direction == 4 && CellIsAvailable(currentRow, currentColumn - 1))
            {
                // west
                randMaze[currentRow, currentColumn].westWall=false;
                randMaze[currentRow, currentColumn - 1].eastWall=false;
                //visited[currentRow, currentColumn] = true;
                currentColumn--;
                //checkedDirections = new List<int> { };
            }

            visited[currentRow, currentColumn] = true;
        }
    }

    private static bool HasAdjVisited(int row, int column)
    {
        int visitedCells = 0;

        // Look 1 row up (north) if we're on row 1 or greater
        if (row > 0 && visited[row - 1, column])
        {
            visitedCells++;
        }

        // Look one row down (south) if we're the second-to-last row (or less)
        if (row < cellSize-1 && visited[row + 1, column])
        {
            visitedCells++;
        }

        // Look one row left (west) if we're column 1 or greater
        if (column > 0 && visited[row, column - 1])
        {
            visitedCells++;
        }

        // Look one row right (east) if we're the second-to-last column (or less)
        if (column < cellSize-1 && visited[row, column + 1])
        {
            visitedCells++;
        }

        // return true if there are any adjacent visited cells to this one
        return visitedCells > 0;
    }

    private static void DestroyAdjacentWall(int row, int column)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
             int direction = Random.Range (1, 5);
            //int direction = ProceduralNumberGenerator.GetNextNumber();

            if (direction == 1 && row > 0 && visited[row - 1, column])
            {
                randMaze[row, column].northWall=false;
                randMaze[row - 1, column].southWall=false;
                wallDestroyed = true;
            }
            else if (direction == 2 && row < cellSize-1 && visited[row + 1, column])
            {
                randMaze[row, column].southWall=false;
                randMaze[row + 1, column].northWall=false;
                wallDestroyed = true;
            }
            else if (direction == 3 && column > 0 && visited[row, column - 1])
            {
                randMaze[row, column].westWall=false;
                randMaze[row, column - 1].eastWall=false;
                wallDestroyed = true;
            }
            else if (direction == 4 && column < cellSize-1 && visited[row, column + 1])
            {
                randMaze[row, column].eastWall=false;
                randMaze[row, column + 1].westWall=false;
                wallDestroyed = true;
            }
        }

    }

    private static bool RouteStillAvailable(int row, int column)
    {
        int availableRoutes = 0;

        if (row > 0 && !visited[row - 1, column] )
        {
            availableRoutes++;
        }

        if (row < cellSize-1 && !visited[row + 1, column] )
        {
            availableRoutes++;
        }

        if (column > 0 && !visited[row, column - 1] )
        {
            availableRoutes++;
        }

        if (column < cellSize-1 && !visited[row, column + 1] )
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private static bool CellIsAvailable(int row, int column)
    {
        if (row >= 0 && row < cellSize && column >= 0 && column < cellSize && !visited[row, column])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static List<object> ToObjectList(MazeCell[,] mazeCells)
    {
        List<object> list = new List<object> { };
        for (int i = 0; i < cellSize; i++)
        {
            for(int j = 0; j < cellSize; j++)
            {
                string[] temparr = {"1","1","1","1"};
                if (!mazeCells[i, j].northWall)
                {
                    temparr[0] = "0";
                }
                if (!mazeCells[i, j].southWall)
                {
                    temparr[1] = "0";
                }
                if (!mazeCells[i, j].eastWall)
                {
                    temparr[2] = "0";
                }
                if (!mazeCells[i, j].westWall)
                {
                    temparr[3] = "0";
                }
                string temp=string.Join(",", temparr);
                list.Add(((object)temp));
            }
        }
        return list;
    }

}
