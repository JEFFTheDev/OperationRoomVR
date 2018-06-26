﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour, IInteractable
{

    private Transform parentBeforeAttach;
    private Rigidbody rb;
    private bool unFreezeOnGrab;
    public bool isGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
  
    public void AttachTo(Transform to)
    {
        parentBeforeAttach = transform.parent;
        transform.SetParent(to);
        rb.useGravity = false;
        rb.isKinematic = true;
        isGrabbed = true;
    }

    public void Detach()
    {
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        isGrabbed = false;
    }

    public void OnGrab(Transform hand)
    {
        if (unFreezeOnGrab)
            Freeze(false);

        AttachTo(hand);
    }

    public void OnRelease(Transform hand)
    {
        Detach();
    }

    public void Freeze(bool freeze)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

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

    public void UnFreezeOnGrab(bool unFreeze)
    {
        unFreezeOnGrab = unFreeze;
    }
}