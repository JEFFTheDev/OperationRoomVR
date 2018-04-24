using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour {

    public UnityEvent onGrab;
    public UnityEvent onRelease;
    public string[] tags;
    private GameObject collidedWith;
    private bool isGrabbed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        if (HasTag(col.gameObject) && !isGrabbed)
        {
            collidedWith = col.gameObject;
            isGrabbed = true;
            onGrab.Invoke();
            Debug.Log("Colliding with: " + col.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject == collidedWith)
        {
            collidedWith = null;
            isGrabbed = false;
            onRelease.Invoke();
            Debug.Log("Stopped colliding with: " + col.gameObject.name);
        }
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
