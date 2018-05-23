using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ViveControllerInput : MonoBehaviour, IInput {

    public Hand right;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(right.controller != null)
            Debug.Log("Grabbing: " + Grab());
    }

    public bool Grab()
    {
        return right.controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }

    public bool Release()
    {
        return false;
    }
}
