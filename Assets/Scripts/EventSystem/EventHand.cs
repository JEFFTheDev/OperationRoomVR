using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHand : MonoBehaviour {

    private Dictionary<GameObject, IInteractable> interactibles;

	// Use this for initialization
	void Start () {
        interactibles = new Dictionary<GameObject, IInteractable>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {

    }

    private void OnTriggerExit(Collider col)
    {
        
    }

    private IInteractable GetIfInteractable(GameObject go)
    {
        MonoBehaviour[] list = go.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is IInteractable)
            {
                IInteractable interactable = (IInteractable)mb;
                return interactable;
            }
        }

        return null;
    }

    public void SendEventMessage(GameObject sendTo, string message)
    {
        //Send event message to object and this hand transform with it
        sendTo.SendMessage(message, transform, SendMessageOptions.DontRequireReceiver);
    }
}