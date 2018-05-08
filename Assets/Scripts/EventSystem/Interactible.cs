using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour {


    public UnityEvent onGrab;
    public UnityEvent onRelease;
    public UnityEvent onHold;
    public string[] tags;
    private GameObject collidedWith;
    private bool isGrabbed;
    public Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(isGrabbed)
        {
            if (IsGrabbing())
            {
                Hold();
            }
            else
            {
                Release();
            }
        }
	}

    private void OnTriggerEnter(Collider col)
    {
        if (HasTag(col.gameObject) && !isGrabbed && IsGrabbing())
        {
            Grab(col);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject == collidedWith)
        {
            Release();
        }
    }

    private void Grab(Collider col)
    {
        collidedWith = col.gameObject;
        isGrabbed = true;
        onGrab.Invoke();
        Debug.Log("Colliding with: " + col.gameObject.name);
    }

    private void Hold()
    {
        onHold.Invoke();
    }

    private void Release()
    {
        Debug.Log("Stopped colliding with: " + collidedWith.gameObject.name);
        collidedWith = null;
        isGrabbed = false;
        onRelease.Invoke();
    }

    private bool IsGrabbing()
    {
        return true;
    }

    private bool HasTag(GameObject hasTag)
    {
        foreach(string t in tags)
        {
            if(hasTag.tag == t)
                return true;
        }

        return false;
    }
}
