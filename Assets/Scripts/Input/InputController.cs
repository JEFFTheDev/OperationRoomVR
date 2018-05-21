using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public ViveControllerInput viveInput;
    public static IInput Input { get; private set; }

	// Use this for initialization
	void Start () {
        Input = viveInput;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
