using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour, IInteractable {


    //Event messages
    private const string grabMessage = "OnGrab";
    private const string releaseMessage = "OnRelease";
    private const string holdMessage = "OnHold";
    private const string touchMessage = "OnTouch";
    private const string gazeMessage = "OnGaze";

    public MonoBehaviour onGrabSendTo;
    public MonoBehaviour onReleaseSendTo;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGrab(Transform hand)
    {
        onGrabSendTo.SendMessage(grabMessage, hand, SendMessageOptions.DontRequireReceiver);
    }

    public void OnRelease(Transform hand)
    {
        onReleaseSendTo.SendMessage(releaseMessage, hand, SendMessageOptions.DontRequireReceiver);
    }
}
