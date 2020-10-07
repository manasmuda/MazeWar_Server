using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static LinkedList<int> FindPath(int si,int sj,int di,int dj)
    {
        LinkedList<int> path = new LinkedList<int> { };
        bool[,] visited = new bool[20, 20];
        LinkedList<PathNode> pathNodes = new LinkedList<PathNode> { };
        PathNode init = new PathNode(si,sj,0, 0,null);
        pathNodes.AddLast(init);
        PathNode destNode;
        while(true)
        {
            PathNode curNode = GetLeastF(pathNodes);
            if(curNode.i==di && curNode.j == dj)
            {
                destNode = curNode;
                break;
            }
            AddAdjNodes(pathNodes, curNode, di, dj, visited);
        }
        PathNode curNode1 = destNode;
        while (curNode1 != null)
        {
            path.AddFirst(curNode1.i * 20 + curNode1.j);
            curNode1 = curNode1.prevNode;
        }
        return path;
    }

    public static void AddAdjNodes(LinkedList<PathNode> pathNodes,PathNode curNode,int di,int dj,bool[,] visited)
    {
        MazeCell curCell = GameData.maze[curNode.i, curNode.j];
        int hi = di - curNode.i;
        int hj = dj - curNode.j;
        if(curNode.i>0 && !curCell.northWall && !visited[curNode.i-1,curNode.j])
        {
            PathNode node = new PathNode(curNode.i - 1, curNode.j, curNode.gValue + 1, hi + hj + 1 + curNode.gValue + 1,curNode);
            pathNodes.AddLast(node);
            visited[curNode.i - 1, curNode.j] = true;
        }
        if (curNode.i < 19 && !curCell.southWall && !visited[curNode.i + 1, curNode.j])
        {
            PathNode node = new PathNode(curNode.i + 1, curNode.j, curNode.gValue + 1, hi + hj - 1 + curNode.gValue + 1,curNode);
            pathNodes.AddLast(node);
            visited[curNode.i + 1, curNode.j] = true;
        }
        if (curNode.j > 0 && !curCell.westWall && !visited[curNode.i, curNode.j-1])
        {
            PathNode node = new PathNode(curNode.i, curNode.j-1, curNode.gValue + 1, hi + hj + 1 + curNode.gValue + 1,curNode);
            pathNodes.AddLast(node);
            visited[curNode.i, curNode.j-1] = true;
        }
        if (curNode.j<19 && !curCell.eastWall && !visited[curNode.i, curNode.j + 1])
        {
            PathNode node = new PathNode(curNode.i, curNode.j + 1, curNode.gValue + 1, hi + hj - 1 + curNode.gValue + 1,curNode);
            pathNodes.AddLast(node);
            visited[curNode.i, curNode.j + 1] = true;
        }
    }

    public static PathNode GetLeastF(LinkedList<PathNode> pathNodes)
    {
        LinkedListNode<PathNode> cn= pathNodes.First;
        int min = cn.Value.fValue;
        LinkedListNode<PathNode> minNode=cn;
        while (cn != null)
        {
            if (cn.Value.fValue < min)
            {
                min = cn.Value.fValue;
                minNode = cn;
            }
            cn = cn.Next;
        }
        PathNode minPathNode = minNode.Value;
        pathNodes.Remove(minNode);
        return minPathNode;
    }
}
