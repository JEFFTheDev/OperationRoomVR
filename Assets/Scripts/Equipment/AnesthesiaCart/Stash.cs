using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash : MonoBehaviour, IInteractable {

    public GameObject spawnPrefab;

    public void OnGrab(Transform hand)
    {
        GameObject prefab = GameObject.Instantiate(spawnPrefab);
        prefab.GetComponent<Grab>().AttachTo(hand);
        prefab.transform.localPosition = Vector3.zero;
        hand.GetComponent<EventHand>().AttachManually(prefab);
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
