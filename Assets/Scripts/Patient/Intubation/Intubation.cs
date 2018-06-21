using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intubation : MonoBehaviour {
    public GameObject laryngoscope;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<HeadAndMouth>().headTilted && GetComponent<HeadAndMouth>().jawTilted && Preperation.IsDonePreparing && !laryngoscope.activeSelf)
        {
            laryngoscope.SetActive(true);
        }
	}
}
