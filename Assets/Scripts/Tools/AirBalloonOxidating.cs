using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBalloonOxidating : MonoBehaviour {

    public GameObject accept;
    public GameObject headForMedicalSyringesBeforIntubation;
    public GameObject[] nextItems;
    private Animator a;

	void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject == accept || other.gameObject.name.Substring(0, 8) == accept.name.Substring(0, 8))
        {
            Debug.Log("balloon collision");
            a = other.GetComponent<Animator>();
            Debug.Log("Name: " + other.gameObject.name);
            StartCoroutine(Timer(5));
        }
    }

    IEnumerator Timer(float time)
    {
        a.SetBool("readyToOxidate", true);
        Debug.Log("animating balloon...");
        yield return new WaitForSeconds(time);
        Debug.Log("done animating balloon...");
        a.SetBool("readyToOxidate", false);
        headForMedicalSyringesBeforIntubation.GetComponent<SyringesInjection>().enabled = true;

        foreach(GameObject g in nextItems)
        {
            g.SetActive(true);
            g.GetComponent<Grab>().Freeze(true);
        }

        Destroy(a.gameObject);
        Destroy(this.gameObject);
    }
}
