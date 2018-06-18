using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHand : MonoBehaviour
{

    public ViveControllerInput input;
    private GameObject currentTouched;
    private GameObject currentGrabbed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (input.TriggerDown() && currentGrabbed == null && currentTouched != null )
        {
            currentGrabbed = currentTouched;

            foreach (IInteractable i in GetInteractables(currentGrabbed))
            {
                i.OnGrab(transform);
            }

            foreach (IInteractable i in GetInteractables(currentTouched))
            {
                i.OnTouchStop(transform);
            }

            currentTouched = null;
        }
        
        if(input.TriggerUp() && currentGrabbed != null && currentTouched == null)
        {
            foreach (IInteractable i in GetInteractables(currentGrabbed))
            {
                i.OnRelease(transform);
                StartCoroutine(RetryCollision());
            }

            currentGrabbed = null;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(currentTouched == null && currentGrabbed == null)
        {
            IInteractable[] interactables = GetInteractables(col.gameObject);

            foreach (IInteractable i in interactables)
            {
                i.OnTouch(transform);
            }

            currentTouched = col.gameObject;
        }
        
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject == currentTouched && col.gameObject != currentGrabbed)
        {
            IInteractable[] interactables = GetInteractables(col.gameObject);

            foreach (IInteractable i in interactables)
            {
                i.OnTouchStop(transform);
            }

            currentTouched = null;
        }
    }

    private IInteractable[] GetInteractables(GameObject go)
    {
        MonoBehaviour[] list = go.GetComponents<MonoBehaviour>();
        List<IInteractable> interactables = new List<IInteractable>();

        foreach (MonoBehaviour mb in list)
        {
            if (mb is IInteractable)
            {
                IInteractable interactable = (IInteractable)mb;
                interactables.Add(interactable);
            }
        }

        return interactables.ToArray();
    }

    IEnumerator RetryCollision()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(.05f);
        GetComponent<BoxCollider>().enabled = true;
    }

    public void AttachManually(GameObject attach)
    {
        currentGrabbed = attach;
    }
}