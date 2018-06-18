using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Preperation : MonoBehaviour
{

    public GameObject[] nextItems;
    private GameObject other;
    private bool isInPlace;
    public static int preperationCount;
    public static bool IsDonePreparing
    {
        get
        {
            return ObjectSlot.preperationCount >= 5;
        }

        private set
        {
            //Do nothing
        }
    }
    public GameObject accept;
    private Vector3 lockPos;
    private Quaternion lockRot;

    private void Update()
    {
        if (ObjectSlot.IsDonePreparing && isInPlace)
        {
            Debug.Log("Done preparing! Items can be picked up again!");
            Grab g = other.GetComponent<Grab>();
            g.Freeze(false);
            Destroy(this.gameObject);
        }


        if (other)
        {
            other.transform.position = lockPos;
            other.transform.rotation = lockRot;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (accept == other.gameObject || other.gameObject.name.Substring(0, 5) == accept.name.Substring(0, 5))
        {
            this.other = other.gameObject;

            other.transform.rotation = this.transform.rotation;
            other.transform.position = this.transform.position;
            Grab g = other.GetComponent<Grab>();
            g.Detach();
            g.Freeze(true);


            if (nextItems.Length > 0)
            {
                foreach (GameObject next in nextItems)
                {
                    next.SetActive(true);
                }
            }


            isInPlace = true;

            ObjectSlot.preperationCount++;

            lockPos = transform.position;
            lockRot = transform.rotation;
            Debug.Log("Lock pos: " + lockPos);
            DisableAllRenderers();
        }
    }

    private void DisableAllRenderers()
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }
    }
}
