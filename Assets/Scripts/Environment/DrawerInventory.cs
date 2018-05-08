using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DrawerInventory : MonoBehaviour {

    private List<GameObject> items;
    public GameObject[] ignoreIfParent;

	// Use this for initialization
	void Start () {
        items = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();
        Debug.Log("enter");
        if (otherRb && !other.isTrigger)
        {
            FreezeRigidbody(otherRb);
            other.transform.parent = transform;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();
        Debug.Log("exit");
        if (otherRb && !other.isTrigger)
        {
            otherRb.constraints = RigidbodyConstraints.None;
        }
    }

    private void AddItem(Rigidbody item, bool add)
    {
        item.transform.SetParent(add ? transform : null);
        //item.useGravity = !add;
        item.isKinematic = add;
    }

    private bool HasItem(GameObject g)
    {
        return items.Contains(g);
    }

    private bool Ignore(GameObject go)
    {
        foreach(GameObject g in ignoreIfParent)
        {
            if (go.transform.parent == g.transform)
                return true;
        }

        return false;
    }

    private void FreezeRigidbody(Rigidbody rb)
	{
        rb.constraints = RigidbodyConstraints.FreezeAll;
	}
}
