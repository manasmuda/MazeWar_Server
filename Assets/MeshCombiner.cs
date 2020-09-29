using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public MeshFilter myMeshFilter;
    void Start()
    {
        /*MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine,true,true);
        transform.gameObject.SetActive(true);*/
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<List<CombineInstance>> combines = new List<List<CombineInstance>>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        List<Material> materials=new List<Material>();
        foreach(Material material in renderers[1].materials)
        {
            materials.Add(material);
            combines.Add(new List<CombineInstance>());
        }
        int i = 1;
        while (i < meshFilters.Length)
        {
            Debug.Log(meshFilters[i].sharedMesh.subMeshCount);
            for(int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
            {
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilters[i].sharedMesh;
                combine.subMeshIndex = j;
                combine.transform= meshFilters[i].transform.localToWorldMatrix;
                combines[j].Add(combine);
            }
            meshFilters[i].mesh=null;
            Destroy(meshFilters[i].transform.GetComponent<MeshRenderer>());
            Destroy(meshFilters[i]);
            i++;
        }
        Mesh[] meshes = new Mesh[combines.Count];
        CombineInstance[] finalCombines = new CombineInstance[combines.Count];
        for(int j = 0; j < combines.Count; j++)
        {
            meshes[j] = new Mesh();
            meshes[j].indexFormat = IndexFormat.UInt32;
            meshes[j].CombineMeshes(combines[j].ToArray(), true);
            finalCombines[j] = new CombineInstance();
            finalCombines[j].mesh = meshes[j];
            finalCombines[j].subMeshIndex = 0;
            finalCombines[j].transform = Matrix4x4.TRS(transform.position,transform.rotation,transform.localScale);
            
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.indexFormat = IndexFormat.UInt32;
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(finalCombines,false);
        transform.GetComponent<MeshRenderer>().materials = materials.ToArray();
        transform.gameObject.SetActive(true);
        /*myMeshFilter = GetComponent<MeshFilter>();
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

        // All the meshes in our children (just a big list)
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); // <-- you can optimize this
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                    materials.Add(localMat);
        }

        // Each material will have a mesh for it.
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.transform == transform) continue;
                // The filter doesn't know what materials are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // Let's see if their materials are the one we want right now.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                        continue;
                    // This submesh is the material we're looking for right now.
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex;
                    ci.transform = Matrix4x4.identity;
                    combiners.Add(ci);
                }
            }
            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true);
            submeshes.Add(mesh);
        }

        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }
        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = IndexFormat.UInt32;
        finalMesh.CombineMeshes(finalCombiners.ToArray(), true);
        myMeshFilter.sharedMesh = finalMesh;
        Debug.Log("Final mesh has " + submeshes.Count + " materials.");*/
    }
}