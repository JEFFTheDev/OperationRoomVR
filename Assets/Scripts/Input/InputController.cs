using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour {

    public ViveControllerInput viveInput;
    public static IInput VRInput { get; private set; }

	// Use this for initialization
	void Start () {
        VRInput = viveInput;
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("OperatingTheatrePrototype");
        }
	}
}
