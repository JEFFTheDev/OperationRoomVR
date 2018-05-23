using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (hand.controller == null)
            return false;

        return hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }

    public bool TriggerUp()
    {
        if (hand.controller == null)
            return false;

        return !hand.controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }
}
