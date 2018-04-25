// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts
{
    public class RegularHand : Hand
    {

        /// <summary>
        ///     Updates a skeletal from glove data
        /// </summary>
        void Update()
        {
            Thumb.rotation = ThumbRotation();

            var handData = HandManager.HandData;
            // Update the hands. Most of this data is based directly on the sensors.
            if (!handData.ValidOutput(DeviceType))
                return;

            // Adjust the default orientation of the hand when the CalibrateKey is pressed.
            if (Input.GetKeyDown(CalibrateKey))
            {
                Debug.Log("Calibrated a hand.");
                HandData.TrackingValues.HandYawOffset[DeviceType] = AllignmentOffset();
            }
        }

        private void AllignWrist(HandData handData, float offset)
        {
            Debug.Log("Calibrated a hand.");
            WristTransform.rotation = handData.GetWristRotation(DeviceType);

            HandData.TrackingValues.HandYawOffset[DeviceType] = offset;
        }

        private float AllignmentOffset()
        {
            var offset = WristTransform.localEulerAngles.z - HandData.TrackingValues.HandYawOffset[DeviceType];
            return -offset;
        }
    }

}