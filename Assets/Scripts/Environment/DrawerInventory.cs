using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DrawerInventory : MonoBehaviour {

    private List<Transform> items;

	// Use this for initialization
	void Start () {
        items = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.transform.GetComponent<Rigidbody>();
        Debug.Log("enter");
        if (otherRb && !other.isTrigger && !HasItem(other.transform))
        {
            AddItem(other.transform);
            FreezeRigidbody(otherRb);
            other.transform.parent = transform;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody otherRb = other.transform.GetComponent<Rigidbody>();
        Debug.Log("exit");
        if (otherRb && !other.isTrigger && HasItem(other.transform))
        {
            RemoveItem(other.transform);
            otherRb.constraints = RigidbodyConstraints.None;
        }
    }

    private void AddItem(Transform t)
    {
        items.Add(t);
    }

    private void RemoveItem(Transform t)
    {
        items.Remove(t);
    }

    private bool HasItem(Transform t)
    {
        return items.Contains(t);
    }
    
    private void FreezeRigidbody(Rigidbody rb)
	{
        rb.constraints = RigidbodyConstraints.FreezeAll;
	}
}
