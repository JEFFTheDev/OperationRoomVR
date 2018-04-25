// Copyright (c) 2018 ManusVR
using System;
using Assets.ManusVR.Scripts.Extra;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.ManusVR.Scripts.ManusInterface
{
    public class PhysicsLever : MonoBehaviour
    {
        public Action<float> OnValueChanged;
        [SerializeField]
        private CustomEvents.UnityEventFloat _valueChangedEvent;

        private Quaternion _minRotation, _midRotation, _maxRotation;
        private float _angleRange;

        [SerializeField, HideInInspector]
        private float _initialValue;
        public Vector2 MinMaxValue;

        private HingeJoint _hingeJoint;

        private float _currentValue = Single.NaN;
        
        public float CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (float.IsNaN(value) || FloatComparer.AreEqual(_currentValue, value, 0.0001f))
                    return;

                _currentValue = value;
                OnValueChanged.Invoke(_currentValue);
            }
        }

        void Awake()
        {
            _hingeJoint = gameObject.GetComponent<HingeJoint>();
            _midRotation = _hingeJoint.transform.localRotation;
            _minRotation = _midRotation * Quaternion.AngleAxis(_hingeJoint.limits.min, _hingeJoint.axis);
            _maxRotation = _midRotation * Quaternion.AngleAxis(_hingeJoint.limits.max, _hingeJoint.axis);
            _angleRange = Mathf.Max(_hingeJoint.limits.max, _hingeJoint.limits.min) -
                          Mathf.Min(_hingeJoint.limits.max, _hingeJoint.limits.min);
        }

        protected void Start()
        {
            OnValueChanged += _valueChangedEvent.Invoke;
            CurrentValue = _initialValue;
            RotateToValue(CurrentValue);
        }

        void OnValidate()
        {
            if (MinMaxValue.y < MinMaxValue.x)
                MinMaxValue.y = MinMaxValue.x;
            _initialValue = Mathf.Clamp(_initialValue, MinMaxValue.x, MinMaxValue.y);
        }

        void Update()
        {
            float angle = _hingeJoint.angle - _hingeJoint.limits.min;
            CurrentValue = Mathf.Lerp(MinMaxValue.x, MinMaxValue.y, angle / _angleRange);
        }

        public void RotateToValue(float value)
        {
            
            var valuePercent = Mathf.InverseLerp(MinMaxValue.x, MinMaxValue.y, value);
            var newAngle = _angleRange* valuePercent;
            transform.localRotation = _minRotation * Quaternion.AngleAxis(newAngle, _hingeJoint.axis);
        }
    }
}
