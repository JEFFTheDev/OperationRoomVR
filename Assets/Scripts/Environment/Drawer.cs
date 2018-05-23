using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public bool lockLocX, lockLocY, lockLocZ;
    public Vector3 openPos;
    public Vector3 maxPos;
    public Vector3 minPos;
    private Vector3 closePos;
    private Vector3 holdOffset;
    private Vector3 localStartPos;
    private Transform grabbedBy;
    
	// Use this for initialization
	void Start () {
        closePos = transform.localPosition;
        localStartPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (grabbedBy != null)
            OpenByObjectPos(grabbedBy);

		Lock();
    }

    public void OnGrab(Transform t)
    {
        holdOffset = t.position - transform.position;
        grabbedBy = t;
    }

    public void OnRelease()
    {
        grabbedBy = null;
    }

    public void OpenByObjectPos(Transform t)
    {
        transform.position = (t.position - holdOffset);
    }

    public void Lock()
    {
        Vector3 pos = transform.localPosition;
        pos.x = lockLocX ? localStartPos.x : Mathf.Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = lockLocY ? localStartPos.y : Mathf.Clamp(pos.y, minPos.y, maxPos.y);
        pos.z = lockLocZ ? localStartPos.z : Mathf.Clamp(pos.z, minPos.z, maxPos.z);
        
        transform.localPosition = pos;
    }
}
