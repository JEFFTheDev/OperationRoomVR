// Copyright (c) 2018 ManusVR
using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEngine;

namespace Assets.ManusVR.Scripts.Extra
{
    public class P_Bucket : MonoBehaviour
    {
        public GameObject EffectAnchor;
        public GameObject BalloonPop;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Interactable>() != null)
            {
                Destroy(Instantiate(BalloonPop, EffectAnchor.transform.position, EffectAnchor.transform.rotation), 2f);
            }
        }
    }
}
