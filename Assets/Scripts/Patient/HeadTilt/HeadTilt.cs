using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class HeadTilt : MonoBehaviour
{

    Animator animator;
    public Transform lookObj = null;
    private List<Animator> layers;
    //Animator trans;
    //Animator rend;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        //layers = GetComponentsInChildren<Animator>();

    }
    private void Update()
    {
        //trans = GameObject.Find("TransparentLayer").GetComponent<Animator>();
        //rend = GameObject.Find("RenderLayer").GetComponent<Animator>();
    }
    //a callback for calculating IK
    void OnAnimatorIK()
    {

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, lookObj.position);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, lookObj.rotation);
        //animator.SetLookAtPosition(lookObj.position);

        //trans.SetLookAtWeight(1);
        //trans.SetLookAtPosition(lookObj.position);

        //rend.SetLookAtWeight(1);
        //rend.SetLookAtPosition(lookObj.position);
    }
}
