using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class FixPosition : MonoBehaviour {

    private Vector3 pos;
    private Quaternion rot;
	// Use this for initialization
	void Start () {
        pos = transform.position;
        rot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = pos;
        transform.rotation = rot;
	}
}
