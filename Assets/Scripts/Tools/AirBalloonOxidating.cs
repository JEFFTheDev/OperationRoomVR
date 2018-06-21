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
        
        if(other.gameObject == accept)
        {
            Debug.Log("balloon collision");
            a = GetComponent<Animator>();

            StartCoroutine(Timer(5));

        }
    }

    IEnumerator Timer(float time)
    {
        a.SetBool("readyToOxidate", true);
        yield return new WaitForSeconds(time);
        a.SetBool("readyToOxidate", false);
        headForMedicalSyringesBeforIntubation.GetComponent<SyringesInjection>().enabled = true;
        //foreach (Transform child in transform)
        //{
        //    child.gameObject.AddComponent<Rigidbody>();
        //    child.gameObject.AddComponent<Highlight>();
        //    child.gameObject.AddComponent<Grab>();
        //    child.GetComponent<Rigidbody>().useGravity = false;
        //    child.gameObject.AddComponent<BoxCollider>();
        //}
        //GetComponent<Highlight>().enabled = false;
        //GetComponent<Grab>().enabled = false;

        foreach(GameObject g in nextItems)
        {
            g.SetActive(true);
            g.GetComponent<Grab>().Freeze(true);
        }

        Destroy(this.gameObject);
        
    }

    //void Update()
    //{
    //    if (!a.GetBool("readyToOxidate"))
    //    {

    //    }
    //}
}
