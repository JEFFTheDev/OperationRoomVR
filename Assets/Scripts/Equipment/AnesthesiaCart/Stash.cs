using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash : MonoBehaviour, IInteractable
{

    public GameObject spawnPrefab;
    public AudioSource audioSource;
    public AudioClip clip;

    private void Start()
    {
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
    }

    public void OnGrab(Transform hand)
    {
        if (spawnPrefab)
        {
            GameObject prefab = GameObject.Instantiate(spawnPrefab);
            prefab.GetComponent<Grab>().AttachTo(hand);
            prefab.transform.localPosition = Vector3.zero;
            hand.GetComponent<EventHand>().AttachManually(prefab);
            audioSource.PlayOneShot(clip, 0.5f);
        }
    }

    public void OnRelease(Transform hand)
    {
        //
    }

    public void OnTouch(Transform hand)
    {
        //
    }

    public void OnTouchStop(Transform hand)
    {
        //
    }
}
