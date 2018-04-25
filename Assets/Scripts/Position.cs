using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
