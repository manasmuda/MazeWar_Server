using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestEditor : MonoBehaviour
{
    public LinkedList<int> path = new LinkedList<int>();
    public int[] pathList;
    public int dest = 0;

    public bool start = false;
    void Start()
    {
        
    }

    public void StartIt()
    {
        int si = Mathf.FloorToInt((60 - transform.position.z) / 6);
        int sj = Mathf.FloorToInt((transform.position.x + 60) / 6);
        int di = dest / 20;
        int dj = dest % 20;
        path = PathFinder.FindPath(si, sj, di, dj);
        Debug.Log(path.Count);
        printList();
        start = true;
    }

    void Update()
    {
        if (start)
        {
            int ci = Mathf.FloorToInt((60 - transform.position.z) / 6);
            int cj = Mathf.FloorToInt((transform.position.x + 60) / 6);
            if (path.First.Value == (ci * 20 + cj))
            {
                //nothing the player is still in first cell so no change
            }
            else if (path.First.Next.Value == (ci * 20 + cj))
            {
                //the player just moved to second cell
                path.RemoveFirst();
                printList();
            }
            else
            {
                //the player is not in path anymore
                int di = dest / 20;
                int dj = dest % 20;
                path = PathFinder.FindPath(ci, cj, di, dj);
                printList();
            }
        }
    }

    void printList()
    {
        LinkedListNode<int> cn = path.First;
        pathList = new int[path.Count];
        int i = 0;
        while (cn != null)
        {
            pathList[i] = cn.Value;
            cn = cn.Next;
            i++;
        }
    }
}
