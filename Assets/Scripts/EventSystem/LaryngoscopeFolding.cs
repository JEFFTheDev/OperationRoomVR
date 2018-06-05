using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LaryngoscopeFolding : MonoBehaviour
{

    Animator animator;
    public GameObject scope;
    public Light spotlight;
    // Use this for initialization
    void Start()
    {
        animator = scope.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Hand" && !animator.GetBool("unfold") && GetComponent<Throwable>().attached && animator.GetCurrentAnimatorStateInfo(0).IsName("Folded"))
        {
            animator.SetBool("unfold", true);
            spotlight.enabled = true;


        }
        else if (other.tag == "Hand" && animator.GetBool("unfold") && GetComponent<Throwable>().attached && animator.GetCurrentAnimatorStateInfo(0).IsName("Unfolded"))
        {

            animator.SetBool("unfold", false);
            spotlight.enabled = false;
        }
    }
}
    
