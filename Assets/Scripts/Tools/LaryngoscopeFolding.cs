using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LaryngoscopeFolding : MonoBehaviour, IInteractable
{

    Animator animator;
    public GameObject scope;
    public Light spotlight;
    public AudioSource audioSource;
    public AudioClip clip;
    // Use this for initialization
    void Start()
    {
        animator = scope.GetComponent<Animator>();
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Hand" && !animator.GetBool("unfold") && GetComponent<Grab>().isGrabbed && animator.GetCurrentAnimatorStateInfo(0).IsName("Folded"))
        {
            animator.SetBool("unfold", true);
            spotlight.enabled = true;


        }
        else if (other.tag == "Hand" && animator.GetBool("unfold") && GetComponent<Grab>().isGrabbed && animator.GetCurrentAnimatorStateInfo(0).IsName("Unfolded"))
        {

            animator.SetBool("unfold", false);
            spotlight.enabled = false;
        }
    }

    public void OnGrab(Transform hand)
    {
        audioSource.PlayOneShot(clip, 0.5f);
    }

    public void OnRelease(Transform hand)
    {
        //
    }

    public void OnTouch(Transform hand)
    {
        //
    }

    public void OnTouchStop(Transform hand)
    {
        //
    }
}
    
