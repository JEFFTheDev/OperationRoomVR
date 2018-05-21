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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(lookObj.position);
    }
}