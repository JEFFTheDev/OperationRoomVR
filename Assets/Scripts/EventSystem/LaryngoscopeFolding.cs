using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LaryngoscopeFolding : MonoBehaviour {

	Animator anim;
	public GameObject scope;
	// Use this for initialization
	void Start () {
		anim = scope.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
		
		if (other.tag == "Player" && !anim.GetBool("unfold") && GetComponent<Throwable>().attached)
        {
			anim.SetBool("unfold",true);
            
        }
        else if (other.tag == "Player" && anim.GetBool("unfold") && GetComponent<Throwable>().attached)
        {
			anim.SetBool("unfold",false);

		}
			
            
    }
}
