using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour, IInteractable {


    //Event messages
    private const string grabMessage = "OnGrab";
    private const string releaseMessage = "OnRelease";
    private const string onTouchMessage = "OnTouch";
    private const string onTouchStopMessage = "OnTouchStop";

    public Transform onGrabSendTo;
    public Transform onTouchSendTo;

    public bool sendToParentIfNull;

    // Use this for initialization
    void Start () {
        if (sendToParentIfNull)
        {
            if (onGrabSendTo == null)
                onGrabSendTo = transform.parent;
            
            if (onTouchSendTo == null)
                onTouchSendTo = transform.parent;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //OnGrab and OnRelease send their message to the same object
    public void OnGrab(Transform hand)
    {
        SendEventMessage(grabMessage, onGrabSendTo, hand);
    }

    public void OnRelease(Transform hand)
    {
        SendEventMessage(releaseMessage, onGrabSendTo, hand);
    }

    //OnTouch and OnTouchStop send their message to the same object
    public void OnTouch(Transform hand)
    {
        SendEventMessage(onTouchMessage, onTouchSendTo, hand);
    }

    public void OnTouchStop(Transform hand)
    {
        SendEventMessage(onTouchStopMessage, onTouchSendTo, hand);
    }
    
    //Sends message to a receiver
    private void SendEventMessage(string message, Transform receiver, Transform hand)
    {
        if (receiver == null)
            return;

        receiver.SendMessage(message, hand, SendMessageOptions.DontRequireReceiver);
    }
}
