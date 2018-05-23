using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Respawning item: " + other);

        //Rigidbody rb = other.GetComponent<Rigidbody>();
        ////Interactible i = other.GetComponent<Interactible>();

        //if(rb && i)
        //{
        //    rb.velocity = Vector3.zero;
        //    other.transform.position = i.startPos;
        //}
    }
}
