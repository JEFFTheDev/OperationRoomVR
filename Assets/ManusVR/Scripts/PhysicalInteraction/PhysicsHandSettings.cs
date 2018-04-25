// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    [CreateAssetMenu]
    public class PhysicsHandSettings : ScriptableObject
    {
        [Tooltip("The max velocity of the hand when colliding")] [Range(0, 5)]
        public float MaxVelocityColliding = 0.6f;
        [Tooltip("The max velocity when not colliding")] [Range(0, 10)]
        public float MaxVelocityNotColliding = 3;
    }
}
