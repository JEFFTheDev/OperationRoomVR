using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHand : MonoBehaviour
{
    public ViveControllerInput input;
    private Dictionary<GameObject, IInteractable> interactable;
    private IInteractable currentGaze;
    private IInteractable currentGrabbed;

    // Use this for initialization
    void Start()
    {
        interactable = new Dictionary<GameObject, IInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.Grab() && currentGaze != null && currentGrabbed == null)
        {
            currentGaze.OnGrab(transform);
        }

        if(input.Release() && currentGrabbed != null)
        {
            currentGrabbed.OnRelease(transform);
            currentGrabbed = null;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        IInteractable interactable = GetIfInteractable(col.gameObject);

        if (interactable == null)
            return;


        if (!HasInteractable(col.gameObject))
        {
            this.interactable.Add(col.gameObject, interactable);
        }

        //TODO check if currentgaze != currentgrabbed
        currentGaze = interactable;
    }

    private void OnTriggerExit(Collider col)
    {
        currentGaze = null;
    }

    private bool HasInteractable(GameObject g)
    {
        return interactable.ContainsKey(g);
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