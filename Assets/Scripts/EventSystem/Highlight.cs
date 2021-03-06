﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour, IInteractable {

    private bool isHighlighted;
    private Shader startShader;

    private void Start()
    {
        startShader = GetComponent<MeshRenderer>().material.shader;
    }
    
    public void OnTouch(Transform hand)
    {
        HighlightObject(true);
    }

    public void OnTouchStop(Transform hand)
    {
        HighlightObject(false);
    }

    private void HighlightObject(bool hightlight)
    {
        foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            Material m = mr.material;
            m.shader = hightlight ? Shader.Find("Outlined/Silhouetted Diffuse") : startShader;
            isHighlighted = hightlight;
        }
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
