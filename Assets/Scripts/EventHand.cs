using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHand : MonoBehaviour {

    //Event messages
    private const string grabMessage = "OnGrab";
    private const string releaseMessage = "OnRelease";
    private const string holdMessage = "OnHold";
    private const string touchMessage = "OnTouch";
    private const string gazeMessage = "OnGaze";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        SendEventMessage(col.gameObject, touchMessage);
    }

    private void OnTriggerExit(Collider col)
    {
        
    }

    public void SendEventMessage(GameObject sendTo, string message)
    {
        //Send event message to object and this hand transform with it
        sendTo.SendMessage(message, transform, SendMessageOptions.DontRequireReceiver);
    }
}