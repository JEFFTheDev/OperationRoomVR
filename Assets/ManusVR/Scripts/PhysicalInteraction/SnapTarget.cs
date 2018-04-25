// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    [RequireComponent(typeof(Collider))]
    public class SnapTarget : MonoBehaviour
    {
        public SnappableObject CurrentlySnapped = null;
        private Renderer _rend;

        void Awake()
        {
            _rend = GetComponent<Renderer>();
        }
        public void OnSnap(SnappableObject snappableObject)
        {
            if(_rend)
                _rend.enabled = false;
            CurrentlySnapped = snappableObject;
        }

        public void OnRelease()
        {
            if (_rend)
                _rend.enabled = true;
            CurrentlySnapped = null;
        }
    }
}
