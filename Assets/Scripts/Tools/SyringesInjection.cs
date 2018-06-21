using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class SyringesInjection : MonoBehaviour
{

    public GameObject[] lidsR;
    public GameObject[] lidsL;
    public GameObject propofol;
    public GameObject sufentanil;
    public GameObject rocuronium;
    public GameObject propofolTrans;
    public GameObject sufentanilTrans;
    public GameObject rocuroniumTrans;
    public GameObject posR;
    public GameObject posL;
    public float friction = .05f;
    //public bool preparationDone = false;
    private int counter = 0;

    void Start()
    {
        propofolTrans.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == propofol || other.gameObject.name.Substring(0, 5) == propofol.name.Substring(0, 5) &&
            //preparationDone && 
            counter == 0)

        {
            StartCoroutine(CloseEyes());
            Inject(other.gameObject, propofolTrans);
            FreezeObject(other.gameObject, propofolTrans);
            Debug.Log("Propofol injected.");
            StartCoroutine(Timer(2, delegate { Destroy(other.gameObject); sufentanilTrans.SetActive(true); }));
        }
        else if (other == sufentanil || other.gameObject.name.Substring(0, 5) == sufentanil.name.Substring(0, 5) && counter == 1)
        {
            Inject(other.gameObject, sufentanilTrans);
            Debug.Log("Sufentanil injected.");
            FreezeObject(other.gameObject, sufentanilTrans);
            StartCoroutine(Timer(2, delegate { Destroy(other.gameObject); rocuroniumTrans.SetActive(true); }));

        }
        else if (other == rocuronium || other.gameObject.name.Substring(0, 5) == rocuronium.name.Substring(0, 5) && counter == 2)
        {
            Inject(other.gameObject, rocuroniumTrans);
            Debug.Log("Rocuronium injected.");
            FreezeObject(other.gameObject, rocuroniumTrans);
            StartCoroutine(Timer(6, delegate { Destroy(other.gameObject); HeadAndMouth.moveable = true; }));
        }
    }

    private void FreezeObject(GameObject other, GameObject mimic)
    {
        other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        other.gameObject.GetComponent<Grab>().Detach();
        other.gameObject.transform.SetPositionAndRotation(mimic.transform.position, mimic.transform.rotation);
    }

    private IEnumerator CloseEyes()
    {
        while (true)
        {
            foreach (GameObject lid in lidsR)
            {
                lid.transform.position = Vector3.Lerp(lid.transform.position, posR.transform.position, friction);
            }
            foreach (GameObject lid in lidsL)
            {
                lid.transform.position = Vector3.Lerp(lid.transform.position, posL.transform.position, friction);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Timer(int time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }

    private void Inject(GameObject real, GameObject transparent)
    {
        counter++;
        real.transform.position = transparent.transform.position;
        transparent.SetActive(false);
    }
}
