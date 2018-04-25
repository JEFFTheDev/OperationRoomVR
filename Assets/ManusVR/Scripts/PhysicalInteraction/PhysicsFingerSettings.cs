// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    [CreateAssetMenu]
    public class PhysicsFingerSettings : ScriptableObject
    {
        // Finger interaction settings
        [Tooltip("The amount of force on the HingeJointMoters")] [Range(0, 20)]
        public float MotorForce = 1f;                                   
        [Tooltip("The velocity multiplier of the target velocity when the fingers are colliding")] [Range(0, 100)]
        public float VelocityMultiplierColliding = 5;    
        [Tooltip("The velocity multiplier of the target velocity when the fingers are not colliding")] [Range(0, 100)]
        public float VelocityMultiplierNotColliding = 50;
        [Tooltip("The maximum amount of target velocity on the HingeJoint motors")] [Range(0, 2000)]
        public float MaxTargetVelocity = 1000;
        [Range(0, 1000)]
        public float MaxTargetVelocityColliding = 150;
        // finger movement
        //[Range(0, 10)]
        //public float MaxDeltaX = 5f;
        //[Range(0, 10)]
        //public float MaxDeltaY = 5f;
        //[Range(0, 10)]
        //public float MaxDeltaZ = 5f;

        // Thumb interaction settings
        [Tooltip("The max amount of angular velocity when the thumb is colliding")] [Range(0, 20)]
        public float MaxThumbVelocityColliding = 3;                  
        [Tooltip("The max amount of angular velocity when the thumb is NOT colliding")] [Range(0, 20)]
        public float MaxThumbVelocityNotColliding = 15;              
        [Tooltip("The rotation delta when the thumb is colliding")] [Range(0, 10)]
        public float ThumbRotDeltaColliding = 0.1f;                
        [Tooltip("The rotation delta when the thumb is NOT colliding ")] [Range( 0, 20)]
        public float ThumbRotDeltaNotColliding = 10f;

    }
}
