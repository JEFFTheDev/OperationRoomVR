// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class VelocityClamper : MonoBehaviour {
        /// <summary>
        /// Clamp the velocity on a given rigidbody
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="maxMagnitude"></param>
        public static void ClampVelocity(Rigidbody rigidbody, float maxMagnitude)
        {
            if (rigidbody.velocity.magnitude > maxMagnitude)
                rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxMagnitude);
        }

        /// <summary>
        /// Clamp the angular velocity on a given rigidbody
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="maxMagnitude"></param>
        public static void ClampAngularVelocity(Rigidbody rigidbody, float maxMagnitude)
        {
            if (rigidbody.angularVelocity.magnitude > maxMagnitude)
                rigidbody.angularVelocity = Vector3.ClampMagnitude(rigidbody.angularVelocity, maxMagnitude);
        }
    }
}
