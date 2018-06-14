using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour, IInteractable
{

    private Transform parentBeforeAttach;
    private Rigidbody rb;
    public bool isGrabbed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
  
    private void AttachTo(Transform to)
    {
        Debug.Log("grabbed: " + gameObject.name);
        parentBeforeAttach = transform.parent;
        transform.SetParent(to);
        rb.useGravity = false;
        rb.isKinematic = true;
        isGrabbed = true;
    }

    public void Detach()
    {
        Debug.Log("stopped grab: " + gameObject.name);

        //Maybe return to old parent?
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        isGrabbed = false;
    }

    public void OnGrab(Transform hand)
    {
        AttachTo(hand);
    }

    public void OnRelease(Transform hand)
    {
        Detach();
    }

    public void Freeze(bool freeze)
    {
        rb.constraints = freeze ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
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
