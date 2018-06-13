using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ViveControllerInput : MonoBehaviour, IInput {

    public Hand hand;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool TriggerDown()
    {
        if (hand.controller != null)
            return hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);

        return false;
    }

    public bool TriggerUp()
    {
        if (hand.controller != null)
            return hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

        return false;
    }
}
