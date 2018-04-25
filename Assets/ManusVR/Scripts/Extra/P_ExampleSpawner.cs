// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.Extra
{
    public class P_ExampleSpawner : MonoBehaviour {
        public GameObject Spawnable;
        public Transform SpawnTransform;

        public void SpawnObject()
        {
            Instantiate(Spawnable, SpawnTransform.position, SpawnTransform.rotation);
        }
    }
}
