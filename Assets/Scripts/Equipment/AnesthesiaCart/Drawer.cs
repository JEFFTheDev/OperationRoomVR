using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public bool lockLocX = true, lockLocY, lockLocZ = true;
    public bool lockRotX, lockRotY, lockRotZ;
    public Vector3 maxPos = new Vector3(0, -0.5f, 0);
    public Quaternion maxRot;
    public float maxDistanceFromGrabbed = .3f;
    private Vector3 minPos;
    private Quaternion minRot;
    private Vector3 closePos;
    private Vector3 holdOffset;
    private Vector3 localStartPos;
    private Quaternion localStartRot;
    private Transform grabbedBy;

    // Use this for initialization
    void Start() {
        closePos = transform.localPosition;
        localStartPos = transform.localPosition;
        localStartRot = transform.localRotation;

        minPos = transform.localPosition;
        minRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update() {

        if (grabbedBy != null)
            OpenByObjectPos(grabbedBy);

        Lock();
    }

    public void OnGrabEvent(Transform t)
    {
        holdOffset = t.position - transform.position;
        //rotateOffset = t.rotation * transform.rotation;
        grabbedBy = t;
    }

    public void OnReleaseEvent()
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
        pos.x = lockLocX ? localStartPos.x : Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = lockLocY ? localStartPos.y : Clamp(pos.y, minPos.y, maxPos.y);
        pos.z = lockLocZ ? localStartPos.z : Clamp(pos.z, minPos.z, maxPos.z);
        transform.localPosition = pos;


        if (grabbedBy)
        {
            Quaternion rot = transform.localRotation;
            rot.x = lockRotX ? localStartRot.x : Clamp(Map(GetDistance(), 0, maxDistanceFromGrabbed, minRot.x, maxRot.x), minRot.x, maxRot.x);
            rot.y = lockRotY ? localStartRot.y : Clamp(Map(GetDistance(), 0, maxDistanceFromGrabbed, minRot.y, maxRot.y), minRot.y, maxRot.y);
            rot.z = lockRotZ ? localStartRot.z : Clamp(Map(GetDistance(), 0, maxDistanceFromGrabbed, minRot.z, maxRot.z), minRot.z, maxRot.z);
            transform.localRotation = rot;
        }
    }

    private float GetDistance()
    {
        return Vector3.Distance(transform.position, grabbedBy.position);
    }
    

    private float Map(float value, float min1, float max1, float min2, float max2)
    {
        return (value - min1) * (max2 - min2) / (max1 - min1) + min2;
    }

    private float Clamp(float value, float min, float max)
    {
        float actualMin = min < max ? min : max;
        float actualMax = min > max ? min : max;
        return Mathf.Clamp(value, actualMin, actualMax);
    }
}


