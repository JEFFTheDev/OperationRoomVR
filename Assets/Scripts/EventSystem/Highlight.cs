using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour, IInteractable {

    private bool isHighlighted;
    private GameObject combinedObject;
    private Dictionary<Material, Shader> materialLibrary;

    private void Start()
    {
        materialLibrary = new Dictionary<Material, Shader>();
    }
    
    public void OnTouch(Transform hand)
    {
        Debug.Log("test");

        if (HasMoreMeshes() && combinedObject == null)
            combinedObject = CreateCombinedObject();

        HighlightObject(true, HasMoreMeshes() ? combinedObject : this.gameObject);
    }

    public void OnTouchStop(Transform hand)
    {
        HighlightObject(false, HasMoreMeshes() ? combinedObject : this.gameObject);
    }

    private void HighlightObject(bool hightlight, GameObject obj)
    {
        foreach (MeshRenderer mr in obj.GetComponentsInChildren<MeshRenderer>())
        {
            Material m = mr.material;

            if (!materialLibrary.ContainsKey(m))
                materialLibrary.Add(m, m.shader);

            m.shader = hightlight ? Shader.Find("Outlined/Silhouetted Diffuse") : materialLibrary[m];
            isHighlighted = hightlight;
        }
    }

    private Mesh GetCombinedMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        return combinedMesh;
    }

    private GameObject CreateCombinedObject()
    {
        GameObject g = new GameObject("Highlight object: " + this.gameObject.name);
        g.AddComponent<MeshFilter>().mesh = GetCombinedMesh();
        g.AddComponent<MeshRenderer>();
        g.transform.localScale = transform.localScale;
        g.transform.rotation = transform.rotation;
        g.transform.position = transform.position;
        g.transform.SetParent(this.transform);
        return g;
    }

    private bool HasMoreMeshes()
    {
        Debug.Log("Mesh count: " + GetComponentsInChildren<MeshRenderer>().Length);
        return GetComponentsInChildren<MeshRenderer>().Length > 1;
    }

    public void OnGrab(Transform hand)
    {
        //No implementation needed
    }

    public void OnRelease(Transform hand)
    {
        //No implementation needed
    }

}
