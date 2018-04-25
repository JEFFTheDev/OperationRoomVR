// Copyright (c) 2018 ManusVR
using System.Collections;
using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ManusVR.Scripts.ManusInterface
{
    public class PhysicsSlider : Slider
    {
        [SerializeField, HideInInspector]
        public Vector2 MinMaxMovement;

        private Vector3 _minPosition, _maxPosition;
        private Rigidbody _rb;
        private Vector3 _initialLocalPosition;

        protected override void Awake()
        {
            base.Awake();
            _initialLocalPosition = transform.localPosition;
        }

        protected override void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _minPosition = transform.localPosition;
            _minPosition.z += MinMaxMovement.x;
            _maxPosition = transform.localPosition;
            _maxPosition.z += MinMaxMovement.y;

            base.Start();
#if UNITY_EDITOR
            if (Application.isPlaying && GetComponent<RectTransform>() != null)
            {
                Debug.LogError("Object with PhysicsSlider component can't be part of canvas. Please use the InterfaceSlider component instead!");
                EditorApplication.isPlaying = false;
            }
#endif
            
        }

        void LateUpdate()
        {
            var locPos = _initialLocalPosition;
            locPos.z = transform.localPosition.z;
            transform.localPosition = locPos;
        }

        void FixedUpdate()
        {
            if (transform.localPosition.z <= _minPosition.z - 0.001f)
            {
                _rb.isKinematic = true;
                transform.localPosition = _minPosition;
                StartCoroutine(IgnoreCollisionWhileColliding());
            }
            if (transform.localPosition.z >= _maxPosition.z + 0.001f)
            {
                _rb.isKinematic = true;
                transform.localPosition = _maxPosition;
                StartCoroutine(IgnoreCollisionWhileColliding());
            }
        }

        IEnumerator IgnoreCollisionWhileColliding()
        {
            var interactable = GetComponent<Interactable>();
            while (interactable.TotalCollidingObjects != 0)
            {
                yield return new WaitForFixedUpdate();
            }
            _rb.isKinematic = false;
        }

        protected override float GetCurrentInverseLerpValue()
        {
            return Mathf.InverseLerp(_minPosition.z, _maxPosition.z, transform.localPosition.z);
        }

        protected override void SetSliderPosition(float value)
        {
            var localPos = transform.localPosition;
            localPos.z = Mathf.Lerp(_minPosition.z, _maxPosition.z,
                Mathf.InverseLerp(MinMaxValue.x, MinMaxValue.y, value));

            transform.localPosition = localPos;
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying || MinMaxMovement == null)
                return;

            var bottomLimit = transform.localPosition;
            bottomLimit.y = 0;
            bottomLimit.z += MinMaxMovement.x;

            var upperLimit = transform.localPosition;
            upperLimit.y = 0;
            upperLimit.z += MinMaxMovement.y;


            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.TransformPoint(bottomLimit), transform.TransformPoint(upperLimit));
        }
    }
}
