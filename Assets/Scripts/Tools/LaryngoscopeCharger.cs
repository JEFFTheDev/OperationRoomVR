using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaryngoscopeCharger : MonoBehaviour {

    public GameObject accept;
    public Material charged;
    public Material neutral;
    public MeshRenderer circle;
    private GameObject currentObj;
    private Vector3 chargerPos;
    private Quaternion chargerRot;
    private Grab grabFromOther;

	// Use this for initialization
	void Start () {
        chargerPos = transform.position;
        chargerRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        if (currentObj && !grabFromOther.isGrabbed)
            currentObj.transform.SetPositionAndRotation(chargerPos, chargerRot);

        if (grabFromOther)
        {
            if (grabFromOther.isGrabbed)
            {

                currentObj = null;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == accept)
        {
            currentObj = other.gameObject;
            circle.material = charged;
            Debug.Log("Log sth");
            grabFromOther = other.GetComponent<Grab>();
            grabFromOther.Detach();
            currentObj.transform.SetPositionAndRotation(chargerPos, chargerRot);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == accept)
        {
            circle.material = neutral;
        }
    }
}
