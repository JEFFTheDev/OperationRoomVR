// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.ManusVR.Scripts.ManusInterface
{
    public abstract class Button : InterfaceItem
    {
        public Action OnPress;

        [SerializeField, FormerlySerializedAs("OnPressEvent")]
        private UnityEvent _onPressEvent;
        
        protected RectTransform RectTransform;
        protected Image Image;
        
        protected override void Awake()
        {
            base.Awake();
            Image = GetComponent<Image>();
            RectTransform = GetComponent<RectTransform>();
            OnPress += () =>
            {
                if (_onPressEvent != null) _onPressEvent.Invoke();
            };
        }

        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Method called when button is completely pressed
        /// </summary>
        public virtual void ButtonPressed()
        {
            ButtonPressed(LastTouchedBy);
        }

        /// <summary>
        /// Method called when button is completely pressed
        /// </summary>
        public virtual void ButtonPressed(device_type_t handType)
        {
            if(OnPress != null)
                OnPress.Invoke();
            StartCoroutine(ApplyFeedback());
        }

        /// <summary>
        /// Coroutine that applies feedback after button is pressed.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ApplyFeedback()
        {
            yield break;
        }
    }
}
