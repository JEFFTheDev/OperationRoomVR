using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public Vector3 openPos;
    public float animSpeed;
    private Vector3 closePos;
    private bool isClosed;

	// Use this for initialization
	void Start () {
        isClosed = true;
        closePos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.Lerp(transform.localPosition, isClosed ? closePos : openPos, animSpeed * Time.deltaTime);
	}

    public void OpenOrClose()
    {
        isClosed = !isClosed;
    }
}
