// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using System.Collections.Generic;
using ManusVR;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ManusVR.Scripts
{
    public enum FingerIndex
    {
        thumb,
        index,
        middle,
        ring,
        pink
    }

    /// <summary>
    /// List of values/states to check for each hand
    /// Go from big to small numbers
    /// </summary>
    public enum CloseValue
    {
        Fist = 95,
        Small = 60,
        Tiny = 20,
        Open = 5
    }

    /// <summary>
    /// State of each hand
    /// </summary>
    public struct HandValue
    {
        public CloseValue CloseValue;
        public bool IsClosed;
        public bool IsOpen;
        public bool HandOpened;
        public bool HandClosed;
        public ToggleEvent OnValueChanged;
    }

    [System.Serializable]
    public class ToggleEvent : UnityEvent<CloseValue>
    {
    }

    public class HandData : MonoBehaviour
    {
        public IntPtr Session
        {
            get { return session; }
        }

        private IntPtr session;

        // Saving the leftHand retrieved from the hand
        private manus_hand_t _leftHand;
        internal manus_hand_t _rightHand;

        // Save if the last leftHand was retrieved correctly
        private manus_ret_t _leftRet = manus_ret_t.MANUS_DISCONNECTED;
        private manus_ret_t _rightRet = manus_ret_t.MANUS_DISCONNECTED;

        public KeyCode RotateLeftHandL = KeyCode.S;
        public KeyCode RotateLeftHandR = KeyCode.A;
        public KeyCode RotateRightHandL = KeyCode.W;
        public KeyCode RotateRightHandR = KeyCode.Q;

        private readonly Vector3 _postRotLeft = new Vector3(-90, -90, 0);
        private readonly Vector3 _postRotRight = new Vector3(-90, 90, 0);
        private readonly Vector3 _postRotThumbLeft = new Vector3(-21.7f, 109.24f, -164.3f);
        private readonly Vector3 _postRotThumbRight = new Vector3(212.1f, 82.2f, -28f);

        [SerializeField] private TrackingValues _trackingValues;

        public TrackingValues TrackingValues
        {
            get { return _trackingValues; }
        }

        /// <summary>
        /// Get the close value of the hand
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public CloseValue GetCloseValue(device_type_t deviceType)
        {
            return _handValues[(int) deviceType].CloseValue;
        }

        public UnityEvent<CloseValue> GetOnValueChanged(device_type_t deviceType)
        {
            return _handValues[(int) deviceType].OnValueChanged;
        }

        /// <summary>
        /// Check if the hand just opened
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public bool HandOpened(device_type_t deviceType)
        {
            return _handValues[(int) deviceType].HandOpened;
        }

        /// <summary>
        /// Check if the hand just closed
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public bool HandClosed(device_type_t deviceType)
        {
            return _handValues[(int) deviceType].HandClosed;
        }

        private HandValue[] _handValues = new HandValue[2];

        // Use this for initialization
        public virtual void Start()
        {
            Application.runInBackground = true;
            Manus.ManusInit(out session);
            Manus.ManusSetCoordinateSystem(session, coor_up_t.COOR_Y_UP, coor_handed_t.COOR_LEFT_HANDED);

            for (int i = 0; i < 2; i++)
            {
                _handValues[i].CloseValue = CloseValue.Open;
                _handValues[i].OnValueChanged = new ToggleEvent();
            }

            Manus.ManusGetHand(session, (device_type_t) 0, out _leftHand);
            Manus.ManusGetHand(session, (device_type_t) 1, out _rightHand);
        }



        // Update is called once per frame
        private void Update()
        {
            manus_hand_t leftHand;
            manus_hand_t rightHand;

            _leftRet = Manus.ManusGetHand(session, device_type_t.GLOVE_LEFT, out leftHand);
            _rightRet = Manus.ManusGetHand(session, device_type_t.GLOVE_RIGHT, out rightHand);
            // if the retrieval of the handdata is succesfull update the local value and wether the hand is closed
            if (_leftRet == manus_ret_t.MANUS_SUCCESS)
            {
                _leftHand = leftHand;
                UpdateCloseValue(TotalAverageValue(_leftHand), device_type_t.GLOVE_LEFT);
            }

            if (_rightRet == manus_ret_t.MANUS_SUCCESS)
            {
                _rightHand = rightHand;
                UpdateCloseValue(TotalAverageValue(_rightHand), device_type_t.GLOVE_RIGHT);
            }

            ManualWristRotation();
        }


        private void ManualWristRotation()
        {
            const float speed = 30;
            if (Input.GetKey(RotateRightHandL))
                _trackingValues.HandYawOffset[device_type_t.GLOVE_RIGHT] += Time.deltaTime * speed;
            if (Input.GetKey(RotateRightHandR))
                _trackingValues.HandYawOffset[device_type_t.GLOVE_RIGHT] -= Time.deltaTime * speed;
            if (Input.GetKey(RotateLeftHandL))
                _trackingValues.HandYawOffset[device_type_t.GLOVE_LEFT] += Time.deltaTime * speed;
            if (Input.GetKey(RotateLeftHandR))
                _trackingValues.HandYawOffset[device_type_t.GLOVE_LEFT] -= Time.deltaTime * speed;
        }

        /// <summary>
        /// Check if there is valid output for the given device
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public bool ValidOutput(device_type_t deviceType)
        {
            if (deviceType == device_type_t.GLOVE_LEFT)
                return _leftRet == manus_ret_t.MANUS_SUCCESS;
            else
                return _rightRet == manus_ret_t.MANUS_SUCCESS;
        }

        /// <summary>
        /// Get the thumb imu rotation of the given device
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public Quaternion GetThumbRotation(device_type_t deviceType)
        {
            Vector3 postRotThumb = deviceType == device_type_t.GLOVE_LEFT ? _postRotThumbLeft : _postRotThumbRight;
            var thumbRot = Quaternion.Euler(postRotThumb);

            switch (deviceType)
            {
                case device_type_t.GLOVE_LEFT:
                    return transform.rotation * _leftHand.raw.imu[1] * thumbRot;
                case device_type_t.GLOVE_RIGHT:
                    return transform.rotation * _rightHand.raw.imu[1] * thumbRot;
            }

            return transform.parent.rotation;
        }

        /// <summary>
        /// Get the wrist rotation of a given device
        /// </summary>
        /// <param name="deviceType">The device type</param>
        /// <returns></returns>
        public Quaternion GetWristRotation(device_type_t deviceType)
        {
            Vector3 postRot = deviceType == device_type_t.GLOVE_LEFT ? _postRotLeft : _postRotRight;
            var wristRotOffset = Quaternion.Euler(postRot);
            switch (deviceType)
            {

                case device_type_t.GLOVE_LEFT:
                    return transform.rotation * _leftHand.wrist * wristRotOffset;
                case device_type_t.GLOVE_RIGHT:
                    return transform.rotation * _rightHand.wrist * wristRotOffset;
                default:
                    return Quaternion.identity;
            }
        }

        /// <summary>
        /// Get the rotation of the given finger
        /// </summary>
        /// <param name="fingerIndex"></param>
        /// <param name="deviceType"></param>
        /// <param name="pose"></param>
        /// <returns></returns>
        public Quaternion GetFingerRotation(FingerIndex fingerIndex, device_type_t deviceType, int pose)
        {
            manus_hand_t hand;
            if (deviceType == device_type_t.GLOVE_LEFT)
                hand = _leftHand;
            else
                hand = _rightHand;

            Quaternion fingerRotation = hand.fingers[(int) fingerIndex].joints[pose].rotation;
            if (fingerRotation == null)
                return Quaternion.identity;
            return fingerRotation;
        }

        /// <summary>
        /// Get the average value of the first joints without the thumb
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public double FirstJointAverage(device_type_t deviceType)
        {
            manus_hand_t hand = deviceType == device_type_t.GLOVE_LEFT ? _leftHand : _rightHand;

            double total = hand.raw.finger_sensor[1];
            total += hand.raw.finger_sensor[3];
            total += hand.raw.finger_sensor[5];
            total += hand.raw.finger_sensor[7];
            return total / 4;
        }

        /// <summary>
        /// Get the average value of all fingers combined on the given hand
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public double TotalAverageValue(manus_hand_t hand)
        {
            int sensors = 0;
            double total = 0;
            // Loop through all of the finger values
            for (int bendPosition = 0; bendPosition < 10; bendPosition++)
            {
                // Only get the sensor values of the first bending point without the thumb (1,3,5,7)
                if (bendPosition < 8)
                {
                    sensors++;
                    total += hand.raw.finger_sensor[bendPosition];
                }
            }

            return total / sensors;
        }

        public double Average(device_type_t deviceType)
        {
            return TotalAverageValue(deviceType == device_type_t.GLOVE_RIGHT ? _rightHand : _leftHand);
        }

        internal void UpdateCloseValue(double averageSensorValue, device_type_t deviceType)
        {
            var values = Enum.GetValues(typeof(CloseValue));
            HandValue handValue;
            if (deviceType == device_type_t.GLOVE_LEFT)
                handValue = _handValues[0];
            else
                handValue = _handValues[1];

            CloseValue closest = CloseValue.Open;
            // Save the old value for comparisment
            CloseValue oldClose = handValue.CloseValue;

            // Get the current close value
            foreach (CloseValue item in values)
            {
                // Div by 100.0 is used because an enum can only contain ints
                if (averageSensorValue > (double) item / 100.0)
                    closest = item;
            }

            handValue.CloseValue = closest;

            // Invoke the on value changed event
            if (oldClose != handValue.CloseValue && handValue.OnValueChanged != null)
                handValue.OnValueChanged.Invoke(handValue.CloseValue);

            // Check if the hand just closed
            handValue.HandClosed = oldClose == CloseValue.Tiny && handValue.CloseValue == CloseValue.Small;
            // Check if the hand just opened
            handValue.HandOpened = (oldClose == CloseValue.Small && handValue.CloseValue == CloseValue.Open);

            if (deviceType == device_type_t.GLOVE_LEFT)
                _handValues[0] = handValue;
            else
                _handValues[1] = handValue;
        }

        public void SetInputData(List<manus_hand_t> handData, List<manus_ret_t> succeses, device_type_t deviceType)
        {
            enabled = false;
            if (handData.Count != 0)
                StartCoroutine(PlayRecording(handData, succeses, deviceType));
        }

        public void StopRecording()
        {
            enabled = true;
            StopAllCoroutines();
        }

        IEnumerator PlayRecording(List<manus_hand_t> handData, List<manus_ret_t> succeses, device_type_t deviceType)
        {
            while (true)
            {
                //Loop through data
                for (int i = 0; i < handData.Count; i++)
                {
                    yield return new WaitForFixedUpdate();
                    if (succeses.Count > 0)
                    {
                        if (deviceType == device_type_t.GLOVE_LEFT)
                            _leftRet = succeses[i];
                        else
                            _rightRet = succeses[i];
                        if (succeses[i] != manus_ret_t.MANUS_SUCCESS)
                            continue;
                    }
                    else
                    {
                        _leftRet = manus_ret_t.MANUS_SUCCESS;
                        _rightRet = manus_ret_t.MANUS_SUCCESS;
                    }                 

                    var manusHandT = handData[i];

                    switch (deviceType)
                    {
                        case device_type_t.GLOVE_LEFT:
                            _leftHand = manusHandT;
                            UpdateCloseValue(TotalAverageValue(_leftHand), deviceType);
                            break;
                        case device_type_t.GLOVE_RIGHT:
                            _rightHand = manusHandT;
                            UpdateCloseValue(TotalAverageValue(_rightHand), deviceType);
                            break;
                    }
                }

            }
        }
    }
}