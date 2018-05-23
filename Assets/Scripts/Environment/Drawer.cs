using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public bool lockLocX = true, lockLocY, lockLocZ = true;
    public bool lockRotX, lockRotY, lockRotZ;
    public Vector3 maxPos = new Vector3(0, -0.5f, 0);
    public Quaternion maxRot;
    private Vector3 minPos;
    private Quaternion minRot;
    private Vector3 closePos;
    private Vector3 holdOffset;
    private Quaternion rotateOffset;
    private Vector3 localStartPos;
    private Quaternion localStartRot;
    private Transform grabbedBy;
    
	// Use this for initialization
	void Start () {
        closePos = transform.localPosition;
        localStartPos = transform.localPosition;
        localStartRot = transform.localRotation;

        minPos = transform.localPosition;
        minRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {

        if (grabbedBy != null)
            OpenByObjectPos(grabbedBy);

		Lock();
    }

    public void OnGrabEvent(Transform t)
    {
        holdOffset = t.position - transform.position;
        rotateOffset = t.rotation * transform.rotation;
        grabbedBy = t;
    }

    public void OnReleaseEvent()
    {
        grabbedBy = null;
    }

    public void OpenByObjectPos(Transform t)
    {
        transform.position = (t.position - holdOffset);
        transform.rotation = (t.rotation * rotateOffset);
    }

    public void Lock()
    {
        Vector3 pos = transform.localPosition;
        pos.x = lockLocX ? localStartPos.x : Mathf.Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = lockLocY ? localStartPos.y : Mathf.Clamp(pos.y, minPos.y, maxPos.y);
        pos.z = lockLocZ ? localStartPos.z : Mathf.Clamp(pos.z, minPos.z, maxPos.z);
        transform.localPosition = pos;

        Quaternion rot = transform.localRotation;
        rot.x = lockRotX ? localStartRot.x : Mathf.Clamp(rot.x, minRot.x, maxRot.x);
        rot.y = lockRotY ? localStartRot.y : Mathf.Clamp(rot.y, minRot.y, maxRot.y);
        rot.z = lockRotZ ? localStartRot.z : Mathf.Clamp(rot.z, minRot.z, maxRot.z);
        transform.localRotation = rot;
    }
}
