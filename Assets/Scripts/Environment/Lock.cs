using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour {

    public bool lockLocX, lockLocY, lockLocZ;
    public Vector3 maxPos;
    public Vector3 minPos;
    private Vector3 localStartPos;

	// Use this for initialization
	void Start () {
        localStartPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        
        //Lock position to set variables
        Vector3 pos = transform.localPosition;
        pos.x = lockLocX ? localStartPos.x : Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = lockLocY ? localStartPos.y : Clamp(pos.y, minPos.y, maxPos.y);
        pos.z = lockLocZ ? localStartPos.z : Clamp(pos.z, minPos.z, maxPos.z);
        transform.localPosition = pos;

    }

    private float Clamp(float value, float min, float max)
    {
        float actualMin = min < max ? min : max;
        float actualMax = min > max ? min : max;
        return Mathf.Clamp(value, actualMin, actualMax);
    }
}
