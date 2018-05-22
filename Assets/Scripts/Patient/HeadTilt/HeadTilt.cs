using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class HeadTilt : MonoBehaviour
{
    
    protected Animator animator;
    public Transform lookObj = null;
    private Animator[] layers;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject trans = GameObject.Find("TransparentLayer");
        GameObject rend = GameObject.Find("RenderLayer");
        //layers = GetComponentsInChildren<Animator>();
        layers[0] = animator;
        layers[1] = trans.GetComponent<Animator>();
        layers[2] = rend.GetComponent<Animator>();
        
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        foreach(Animator a in layers)
        {
            a.SetLookAtWeight(1);
            a.SetLookAtPosition(lookObj.position);

        }
    }
}