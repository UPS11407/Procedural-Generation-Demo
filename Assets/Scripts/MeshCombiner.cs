using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCombiner : MonoBehaviour
{
    //this script is a combination of a few I found online to combine meshes.
    //while I didn't devise the algorithm I did repurpose some of it for my own needs.
    [SerializeField] private List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();
    [SerializeField] private GameObject chunkMeshObj;

    public void CombineMeshes()
    {
        Dictionary<Material, List<CombineInstance>> combineMeshInstanceDictionary = new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter filter in sourceMeshFilters)
        {
            Mesh mesh = filter.sharedMesh;
            Material[] mats = filter.GetComponent<MeshRenderer>().sharedMaterials;
            int subCount = mesh.subMeshCount;

            for (int i = 0; i < subCount; i++)
            {
                Material mat = mats[i];

                if (!combineMeshInstanceDictionary.ContainsKey(mat))
                {
                    combineMeshInstanceDictionary.Add(mat, new List<CombineInstance>());
                }

                CombineInstance combineInstance = new CombineInstance
                { transform = filter.transform.localToWorldMatrix, mesh = filter.sharedMesh };
                combineMeshInstanceDictionary[mat].Add(combineInstance);
            }
        }

        foreach(var kvp in combineMeshInstanceDictionary)
        {
            GameObject newObject = Instantiate(chunkMeshObj, transform);
            newObject.name = kvp.Key.name;
            newObject.tag = "ChunkMesh";
            newObject.transform.position = Vector3.zero;

            newObject.GetComponent<MeshRenderer>().material = kvp.Key;
            var mesh = new Mesh();
            //with large chunks this is necessary to keep all objects of the same material combined into 1 mesh.
            //this is not a supported setting on all GPUs
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.CombineMeshes(kvp.Value.ToArray());

            newObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }

    public void AddMeshToCombine(MeshFilter mesh)
    {
        sourceMeshFilters.Add(mesh);
    }        
}
