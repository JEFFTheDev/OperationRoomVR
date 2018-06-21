using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {



	// Use this for initialization
	void Start () {
        //StartCoroutine(GoToOperatingTheatre());
        Invoke("GoToOperatingTheatre", 3f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GoToOperatingTheatre()
    {
        SceneManager.LoadScene(0);
    }
}
