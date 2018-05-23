using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour {

    public bool xPos, yPos, zPos;
    public Vector3 maxLocPos;
    private Vector3 startWorldPos;
    private Transform startParent;

	// Use this for initialization
	void Start () {
        startWorldPos = transform.position;
        startParent = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = transform.position;
        pos.x = xPos ? startWorldPos.x : pos.x;
        pos.y = yPos ? startWorldPos.y : pos.y;
        pos.z = zPos ? startWorldPos.z : pos.z;
        transform.position = pos;

    }
}
