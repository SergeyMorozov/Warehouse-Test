using System.Collections.Generic;
using UnityEngine;

public class CombineMesh
{
    private static GameObject _staticRoot;
    private static Transform _parent;

    public static void Init(GameObject staticRoot, bool useMeshCollider = false)
    {
        if (staticRoot == null)
            return;

        _staticRoot = staticRoot;
        _parent = _staticRoot.transform.parent;
//        _staticRoot.transform.SetParent(null);

        CombineMeshes(_staticRoot);

        // and remove all children parts used for mesh combining from staticPart
        foreach (Transform child in staticRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //whether the mesh should also be used as a meshCollider shape in the collision component.
        if (useMeshCollider)
        {
            MeshCollider meshCollider = staticRoot.GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = staticRoot.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.sharedMesh = staticRoot.GetComponent<MeshFilter>().mesh;
        }
        
//        _staticRoot.transform.SetParent(_parent);
    }

    public static void CombineMeshes(GameObject objectWithMeshes)
    {
        // Move Object to zero position in world for les conflict with matrix calculation mistakes
        Vector3 position_original = objectWithMeshes.transform.position;
        Quaternion rotation_original = objectWithMeshes.transform.rotation;
        objectWithMeshes.transform.position = Vector3.zero;
        objectWithMeshes.transform.rotation = Quaternion.identity;

        // #1: Collect all meshFilters inside the staticRoot/objectWithMeshes its children,
        // these are the ones we are going to combine.
        MeshFilter[] meshFilterWorld = objectWithMeshes.GetComponentsInChildren<MeshFilter>(false);
        List<Mesh> submeshes = new List<Mesh>();
        List<Material> usedMaterials = new List<Material>();

        // Find all materials in objects
        List<Material> objectMaterials = new List<Material>();
        MeshRenderer[] meshRenderWorld = objectWithMeshes.GetComponentsInChildren<MeshRenderer>(false);
        foreach (MeshRenderer mr in meshRenderWorld)
        {
            Material[] localMaterials = mr.sharedMaterials;
            for (int indexMatLocal = 0; indexMatLocal < localMaterials.Length; indexMatLocal++)
            {
                if (objectMaterials.Contains(localMaterials[indexMatLocal]))
                    continue;
                
                objectMaterials.Add(localMaterials[indexMatLocal]);
            }
        }

        // #2: Step through the materials used on the object, these were set manually in the list sharedMaterialMultiple
        // NOTE: these can also be collected dynamically
        foreach (Material mat in objectMaterials)
        {
            // #3: Next create a list of CombineInstance's and step through each MeshFilter to match it on material type
            // and add it to the CombineInstance of the same material.
            // But also store each unique material used in the usedMaterial list.
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter m in meshFilterWorld)
            {
                MeshRenderer renderer = m.GetComponent<MeshRenderer>();
                Material[] localMaterials = renderer.sharedMaterials;
                for (int indexMatLocal = 0; indexMatLocal < localMaterials.Length; indexMatLocal++)
                {
                    if (localMaterials[indexMatLocal] == mat)
                    {
                        // add material to optimized list if it doesn have it yet
                        if (!usedMaterials.Contains(mat))
                        {
                            usedMaterials.Add(mat);
                        }

                        CombineInstance ci = new CombineInstance();
                        ci.mesh = m.sharedMesh;
                        ci.subMeshIndex = indexMatLocal;
                        ci.transform = m.transform.localToWorldMatrix; // copy its transform such as position/rotation from the original as world location.
                        combiners.Add(ci);
                    }
                }
            }

            // #4: Now call the highly anticipated CombineMeshes method on the new Mesh object for the current Material in the list.
            // This way we have one Mesh per Material
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(),
                true); // NOTE: if true it will fail when there are too many vertices it will create multiple submeshes, would require optmizing then!
            submeshes.Add(mesh);
        }
        
        // #5: Step through all the submeshes generated per material from the previous steps #1 - #4,
        // And collect them into a new CombineInstance list.
        List<CombineInstance> combinersFinal = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity; // use default identity matrix to give it a default transfrom
            combinersFinal.Add(ci);
        }

        // #6: use the CombineInstance list from #5, And combine them into one final mesh and set the unique used materials.
        Mesh meshFinal = new Mesh();
        meshFinal.CombineMeshes(combinersFinal.ToArray(),
            false); // NOTE: When true and merging submeshes it will only use a single material
        
        MeshFilter objMeshFilter = objectWithMeshes.GetComponent<MeshFilter>();
        if (objMeshFilter == null)
            objMeshFilter = objectWithMeshes.AddComponent<MeshFilter>();

        MeshRenderer objMeshRenderer = objectWithMeshes.GetComponent<MeshRenderer>();
        if (objMeshRenderer == null)
            objMeshRenderer = objectWithMeshes.AddComponent<MeshRenderer>();

        objMeshFilter.sharedMesh = meshFinal;
        objMeshRenderer.sharedMaterials = usedMaterials.ToArray();

        // Move object back to original position in world
        objectWithMeshes.transform.rotation = rotation_original;
        objectWithMeshes.transform.position = position_original;

//        Debug.Log("CombineMeshes Complete: " + _staticRoot.name);
    }
}

