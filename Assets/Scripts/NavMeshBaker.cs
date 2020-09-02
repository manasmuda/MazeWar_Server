
using UnityEngine;
using UnityEngine.AI;


public class NavMeshBaker : MonoBehaviour
{

    [SerializeField]
    NavMeshSurface[] navMeshSurfaces;
    
    //to bake navmesh to surface during runtime
    private void Awake()
    {
        for(int i=0;i<navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
