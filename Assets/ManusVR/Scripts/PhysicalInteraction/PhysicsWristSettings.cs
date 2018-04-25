// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    [CreateAssetMenu]
    public class PhysicsWristSettings : ScriptableObject
    {
        [Tooltip("The max amount of angular velocity when the hand is colliding")] [Range(0, 20)]
        public float MaxAngularVelocityColliding = 1;
        [Tooltip("The max amount of angular velocity when the hand is NOT colliding")] [Range(0, 20)]
        public float MaxAngularVelocityNotColliding = 5;
        [Tooltip("The max amount of rot delta for the target rigidbody")] [Range(0, 20)]
        public float MaxRotDeltaCollding = 1f; 
        [Tooltip("The max amount of rot delta for the target rigidbody")] [Range(0, 20)]
        public float MaxRotDeltaNotColliding = 6f;                      
    }
}
