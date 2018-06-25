using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour, IInteractable {


    public void OnGrab(Transform hand)
    {
        SceneManager.LoadScene(1);
    }

    public void OnRelease(Transform hand)
    {
        //
    }

    public void OnTouch(Transform hand)
    {
        //
    }

    public void OnTouchStop(Transform hand)
    {
        //
    }
}
