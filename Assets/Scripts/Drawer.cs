using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public bool lockLocX, lockLocY, lockLocZ;
    public Vector3 openPos;
    public float animSpeed;
    public bool instantOpen;
    private Vector3 closePos;
    private bool isClosed;
    private Vector3 holdOffset;
    private Vector3 localStartPos;
    
	// Use this for initialization
	void Start () {
        isClosed = true;
        closePos = transform.localPosition;
        localStartPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        if(instantOpen)
            transform.localPosition = Vector3.Lerp(transform.localPosition, isClosed ? closePos : openPos, animSpeed * Time.deltaTime);

        Lock();
    }

    public void GrabHandle(Transform t)
    {
        holdOffset = t.position - transform.position;
    }

    public void ReleaseHandle()
    {
        
    }

    public void OpenByObjectPos(Transform t)
    {
        transform.position = (t.position - holdOffset);
    }

    public void Lock()
    {
        Vector3 pos = transform.localPosition;
        pos.x = lockLocX ? localStartPos.x : pos.x;
        pos.y = lockLocY ? localStartPos.y : pos.y;
        pos.z = lockLocZ ? localStartPos.z : pos.z;
        transform.localPosition = pos;
    }

    public void OpenOrClose()
    {
        isClosed = !isClosed;
    }
}
