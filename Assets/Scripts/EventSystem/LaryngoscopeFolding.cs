using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaryngoscopeFolding : MonoBehaviour {

    private bool unfold=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && unfold == false)
        {
            this.transform.Rotate(transform.up, 68.873f);
        }
            
    }
}
