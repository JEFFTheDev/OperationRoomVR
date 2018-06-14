using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAndMouth : MonoBehaviour {
    public GameObject headGrab;
    public GameObject head;
    int headAux = 0;
    public GameObject jawGrab;
    public GameObject jaw;
    int jawAux = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(headGrab.GetComponent<Grab>().isGrabbed && headAux < 45)
        {
            Vector3 headRotation = new Vector3(0, 1, 0);
            head.transform.Rotate(headRotation);
            headAux++;
        }

        if (jawGrab.GetComponent<Grab>().isGrabbed && jawAux < 18)
        {
            Vector3 jawRotation = new Vector3(0, -1, 0);
            jaw.transform.Rotate(jawRotation);
            jawAux++;
        }
    }
}
