using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Position : MonoBehaviour {

    public GameObject[] nextItems;
    public string[] tags;

    private void OnTriggerEnter(Collider other)
    {
        if(HasTag(other.gameObject))
        {
            other.transform.rotation = this.transform.rotation;
            other.transform.position = this.transform.position;
            other.GetComponent<Rigidbody>().isKinematic = true;
			other.GetComponent<Rigidbody>().useGravity = false;
			FreezeRigidbody (other.GetComponent<Rigidbody>());
            other.GetComponent<Throwable>().enabled = false;
            //other.GetComponent<Interactable>().enabled = false;
            other.transform.SetParent(null);
            for (int i = 0; i < nextItems.Length; i++)
            {
                nextItems[i].gameObject.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }

    private bool HasTag(GameObject hasTag)
    {
        foreach (string t in tags)
        {
            if (hasTag.tag == t)
                return true;
        }
        return false;
    }

	private void FreezeRigidbody(Rigidbody rb)
	{
		rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
}
