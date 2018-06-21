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
            return Preperation.preperationCount >= 6;
        }

        private set
        {
            //Do nothing
        }
    }
    public GameObject accept;
    public AudioClip impact;
    public bool stayLocked;
    private Vector3 lockPos;
    private Quaternion lockRot;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = impact;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
        

    }

    private void Update()
    {
        if (Preperation.IsDonePreparing && isInPlace && !stayLocked && !audioSource.isPlaying)
        {
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
        if (accept)
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

                audioSource.PlayOneShot(impact, 0.5F);


                Preperation.preperationCount++;

                lockPos = transform.position;
                lockRot = transform.rotation;
                Debug.Log("Lock pos: " + lockPos);
                DisableAllRenderers();
            }
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
