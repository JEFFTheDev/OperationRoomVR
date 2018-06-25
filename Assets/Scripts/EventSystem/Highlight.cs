using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour, IInteractable
{

    private bool isHighlighted;
    private GameObject combinedObject;
    private Dictionary<Material, Shader> materialLibrary;
    private Shader highlightShader;
    private Shader highlightOnlyShader;
    public GameObject[] ignoreThese;

    private void Start()
    {
        materialLibrary = new Dictionary<Material, Shader>();
        highlightShader = Shader.Find("Outlined/Silhouetted Diffuse");
        highlightOnlyShader = Shader.Find("Outlined/Silhouette Only");
    }

    public void OnTouch(Transform hand)
    {
        if (HasMoreMeshes() && combinedObject == null)
            combinedObject = CreateCombinedObject();

        HighlightObject(true, HasMoreMeshes() ? combinedObject : this.gameObject, HasMoreMeshes() ? highlightOnlyShader : highlightShader);
    }

    public void OnTouchStop(Transform hand)
    {
        HighlightObject(false, HasMoreMeshes() ? combinedObject : this.gameObject, HasMoreMeshes() ? highlightOnlyShader : highlightShader);
    }

    private void HighlightObject(bool hightlight, GameObject obj, Shader s)
    {
        foreach (MeshRenderer mr in obj.GetComponentsInChildren<MeshRenderer>())
        {
            foreach(Material m in mr.materials)
            {
                if (!materialLibrary.ContainsKey(m) && m.shader != s)
                    materialLibrary.Add(m, m.shader);

                if (hightlight)
                {
                    if (HasMoreMeshes())
                        combinedObject.SetActive(true);

                    if(m.color != null)
                    {
                        Color c = m.color;
                        m.shader = s;
                        m.SetColor("_MainColor", c);
                    }
                    
                    
                }
                else
                {
                    if (HasMoreMeshes())
                        combinedObject.SetActive(false);

                    m.shader = materialLibrary[m];
                }

                m.shader = hightlight ? s : materialLibrary[m];
            }
            
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
            if (IgnoreThis(meshFilters[i].gameObject))
            {
                i++;
                continue;
            }

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
        //g.transform.rotation = transform.rotation;
        g.transform.SetParent(this.transform);
        //g.transform.position = transform.position;
        return g;
    }

    private bool HasMoreMeshes()
    {

        int meshCount = 0;

        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {

            meshCount++;

            foreach(GameObject g in ignoreThese)
            {
                MeshRenderer gMr = g.GetComponent<MeshRenderer>();

                if (gMr & gMr == mr)
                {
                    meshCount--;
                }
            }
            
        }

        return GetComponentsInChildren<MeshRenderer>().Length > 1;
    }

    private bool IgnoreThis(GameObject obj)
    {
        foreach (GameObject g in ignoreThese)
        {
            if (obj == g)
                return true;
        }

        return false;
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
