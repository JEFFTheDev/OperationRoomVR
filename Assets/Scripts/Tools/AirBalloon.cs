using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBalloon : MonoBehaviour {

    public GameObject airBalloon;
    public GameObject balloon;
    public float animateSpeed;
    public 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Preperation.IsDonePreparing && !airBalloon.activeSelf)
        {
            airBalloon.SetActive(true);
        }
	}
}
