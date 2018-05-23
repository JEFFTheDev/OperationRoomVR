using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ViveControllerInput : MonoBehaviour, IInput {

    public SteamVR_TrackedController controller;
    private bool triggerDown;
    private bool triggerUp;

    // Use this for initialization
    void Start()
    {
        controller.TriggerClicked += delegate { triggerDown = true; triggerUp = false; };
        controller.TriggerUnclicked += delegate { triggerDown = false; triggerUp = true; };
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void say(object sender, ClickedEventArgs e)
    {
        Debug.Log("down or up");
    }

    public bool TriggerDown()
    {
        return triggerDown;
    }

    public bool TriggerUp()
    {
        return triggerUp;
    }
}
