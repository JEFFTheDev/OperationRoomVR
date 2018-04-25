// Copyright (c) 2018 ManusVR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class PhysicsObject
    {
        public Collider[] Colliders;
        public Rigidbody Rigidbody;
        public PhysicsLayer PhysicsLayer;
        public GameObject GameObject;

        public PhysicsObject(Rigidbody rigidbody, Collider[] colliders, PhysicsLayer layer)
        {
            Rigidbody = rigidbody;
            Colliders = colliders;
            PhysicsLayer = layer;
            GameObject = rigidbody.gameObject;
        }
    }

    public class PhysicsManager : MonoBehaviour
    {
        public static PhysicsManager Instance;

        public PhysicsObject[] GetPhysicsObjects { get { return _physicsObjects.Values.ToArray(); } }
        private readonly Dictionary<Transform, PhysicsObject> _physicsObjects = new Dictionary<Transform, PhysicsObject>();
        private readonly Dictionary<PhysicsLayer, List<Collider>> _layerColliders = new Dictionary<PhysicsLayer, List<Collider>>();
        private Dictionary<Transform, Transform> _childs = new Dictionary<Transform, Transform>();

        /// <summary>
        ///     Register a physics object to the physics manager.
        /// </summary>
        /// <param name="colliders">All of the colliders on the given object</param>
        /// <param name="rigidbody"></param>
        public void Register(Collider[] colliders, Rigidbody rigidbody, PhysicsLayer type)
        {
            if (rigidbody == null)
                Debug.LogError("Rigidbody can not be null");

            if (!_physicsObjects.ContainsKey(rigidbody.transform))
            {
                PhysicsObject pObject = new PhysicsObject(rigidbody, colliders, type);
                _physicsObjects.Add(rigidbody.transform, pObject);

                List<Collider> colliderList;
                if (_layerColliders.TryGetValue(type, out colliderList))
                {
                    colliderList.AddRange(colliders);
                }
            }
            else
            {
                HashSet<Collider> newCollection = new HashSet<Collider>(_physicsObjects[rigidbody.transform].Colliders);
                foreach (var collider1 in colliders)
                {
                    newCollection.Add(collider1);
                }
                _physicsObjects[rigidbody.transform].Colliders = newCollection.ToArray();
            }

            foreach (var collider in colliders)
                RegisterChild(collider.gameObject, rigidbody.gameObject);

            foreach (var coll in colliders.Where(c => !c.isTrigger))
                IgnoreColliderCollisionOnLayers(coll, PhysicsLayer.Layers.Where(layer => !type.AllowedCollisions.Contains(layer)));
        }

        public void Remove(Collider[] colliders, Rigidbody rigidbody)
        {
            _physicsObjects.Remove(rigidbody.transform);
            foreach (var collider in colliders)
                _childs.Remove(collider.transform);
        }

        /// <summary>
        /// Register a child gameobject as physics object
        /// </summary>
        /// <param name="child"></param>
        /// <param name="customRoot"></param>
        private void RegisterChild(GameObject child, GameObject customRoot)
        {
            if (!_childs.ContainsKey(child.transform))
                _childs.Add(child.transform, customRoot.transform);
        }

        /// <summary>
        ///     Check if the given GameObject is registered with the physics manager
        /// </summary>
        /// <param name="root">Th</param>
        /// <returns></returns>
        public bool ContainsPhysicsObject(GameObject gameObject)
        {
            return _physicsObjects.ContainsKey(gameObject.transform.root);
        }

        /// <summary>
        ///     Process the collision between objects and check if it should be ignored
        /// </summary>
        /// <param name="objectCollider"></param>
        /// <param name="collision"></param>
        public bool ProcessCollision(Collider objectCollider, Collision collision)
        {
            PhysicsLayer otherLayer;
            if (!_physicsObjects.ContainsKey(collision.transform))
                otherLayer = PhysicsLayer.GetLayer(Layer.UnityDefault);
            else
                otherLayer = _physicsObjects[collision.transform].PhysicsLayer;

            PhysicsObject physicsObject = null;
            if (GetPhysicsObject(objectCollider.gameObject, out physicsObject))
            {
                if (physicsObject.PhysicsLayer == null)
                {
                    physicsObject.PhysicsLayer = PhysicsLayer.GetLayer(Layer.UnityDefault);
                    Debug.LogWarning("PhysicsLayer not assigned. UnityDefault layer automatically inserted instead.");
                }
                if (!physicsObject.PhysicsLayer.CanCollideWith(otherLayer))
                {
                    Physics.IgnoreCollision(objectCollider, collision.collider, true);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Get the beloning physics object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="physicsObject"></param>
        /// <returns></returns>
        public bool GetPhysicsObject(GameObject gameObject, out PhysicsObject physicsObject)
        {
            if (_physicsObjects.ContainsKey(gameObject.transform))
            {
                physicsObject = _physicsObjects[gameObject.transform];
                return true;
            }

            if (_childs.ContainsKey(gameObject.transform))
            {
                physicsObject = _physicsObjects[_childs[gameObject.transform]];
                return true;
            }

            physicsObject = null;
            return false;
        }

        public PhysicsObject[] GetPhysicsObjectsWithLayer(PhysicsLayer physicsLayer)
        {
            return _physicsObjects.Values.Where(physicsObject => physicsObject.PhysicsLayer == physicsLayer).ToArray();
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogWarning("Only one PhysicsManager will be used at the same time");
                return;
            }
                

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                //if (PhysicsPreferences.ShouldPromptFixedTimestep && Time.fixedDeltaTime > PhysicsPreferences.SuggestedTimestep)
                //{
                //    if (EditorUtility.DisplayDialog("Incorrect Timestep Settings",
                //        "We've found that setting the Fixed Timestep to at least " + PhysicsPreferences.SuggestedTimestep +
                //        " results in the most accurate physics interactions. \n\n" +
                //        "Consider adjusting the Fixed Timestep settings in: Edit/Project Settings/Time.",
                //        "Don't Remind Me Again!",
                //        "I Will,\n Copy Value To My Clipboard!"))
                //    {
                //        PhysicsPreferences.ShouldPromptFixedTimestep = false;
                //    }
                //    else
                //    {
                //        EditorGUIUtility.systemCopyBuffer = PhysicsPreferences.SuggestedTimestep.ToString();
                //    }
                //    EditorApplication.isPlaying = false;
                //}
                //else if (PhysicsPreferences.ShouldPromptGravitySettings && !FloatComparer.AreEqual(Physics.gravity.y, PhysicsPreferences.SuggestedGravityForce, 0.0001f))
                //{
                //    if (EditorUtility.DisplayDialog("Gravity Settings",
                //        "We've found that setting the gravity to " + PhysicsPreferences.SuggestedGravityForce +
                //        " results in the most accurate physics interactions. \n\n" +
                //        "Consider adjusting your gravity settings in: Edit/Project Settings/Physics.",
                //        "Don't Remind Me Again!",
                //        "I Will,\n Copy Value To My Clipboard!"))
                //    {
                //        PhysicsPreferences.ShouldPromptGravitySettings = false;
                //    }
                //    else
                //    {
                //        EditorGUIUtility.systemCopyBuffer = PhysicsPreferences.SuggestedGravityForce.ToString();
                //    }
                //    EditorApplication.isPlaying = false;
                //}
                //else if (PhysicsPreferences.ShouldPromptDefaultSolverIterations && Physics.defaultSolverIterations != PhysicsPreferences.SuggestedDefaultSolverIterations)
                //{
                //    if (EditorUtility.DisplayDialog("Default Solver Iterations",
                //        "We've found that setting the amount of Default Solver Iterations to " + PhysicsPreferences.SuggestedDefaultSolverIterations +
                //        " results in the most accurate physics interactions. \n" +
                //        "Consider adjusting your Default Solver Iterations Settings in: Edit/Project Settings/Physics.",
                //        "Don't Remind Me Again!",
                //        "I Will,\n Copy Value To My Clipboard!"))
                //    {
                //        PhysicsPreferences.ShouldPromptDefaultSolverIterations = false;
                //    }
                //    else
                //    {
                //        EditorGUIUtility.systemCopyBuffer = PhysicsPreferences.SuggestedDefaultSolverIterations.ToString();
                //    }
                //    EditorApplication.isPlaying = false;
                //}
                //else if (PhysicsPreferences.ShouldPromptDefaultSolverVelocityIterations && Physics.defaultSolverVelocityIterations != PhysicsPreferences.SuggestedDefaultSolverVelocityIterations)
                //{
                //    if (EditorUtility.DisplayDialog("Default Solver Velocity Iterations",
                //        "We've found that setting the amount of Default Solver Velocity Iterations to " + PhysicsPreferences.SuggestedDefaultSolverVelocityIterations +
                //        " results in the most accurate physics interactions. \n" +
                //        "Consider adjusting your Default Solver Velocity Iterations Settings in: Edit/Project Settings/Physics.",
                //        "Don't Remind Me Again!",
                //        "I Will,\n Copy Value To My Clipboard!"))
                //    {
                //        PhysicsPreferences.ShouldPromptDefaultSolverVelocityIterations = false;
                //    }
                //    else
                //    {
                //        EditorGUIUtility.systemCopyBuffer = PhysicsPreferences.SuggestedDefaultSolverVelocityIterations.ToString();
                //    }
                //    EditorApplication.isPlaying = false;
                //}
            }
#endif     

        }

        void Start()
        {
            foreach (var physicsLayer in PhysicsLayer.Layers)
                _layerColliders.Add(physicsLayer, new List<Collider>());

            StartCoroutine(RegisterNonPhysicsObjects());
        }

        IEnumerator RegisterNonPhysicsObjects()
        {
            var sceneColliders = GameObject.FindObjectsOfType<Collider>();
            yield return null;
            List<Collider> physicsobjectColliders = new List<Collider>();
            foreach (var pObject in _physicsObjects.Values)
            {
                physicsobjectColliders.AddRange(pObject.Colliders);
            }
            sceneColliders = sceneColliders.Except(physicsobjectColliders).Where(coll => coll != null && !coll.isTrigger).ToArray();
            var defaultLayer = PhysicsLayer.GetLayer(Layer.UnityDefault);
            IgnoreCollidersCollisionOnLayers(sceneColliders, defaultLayer.DisallowedCollisions);
        }

        void IgnoreCollidersCollisionOnLayers(IEnumerable<Collider> colliders, IEnumerable<PhysicsLayer> layersToIgnore)
        {
            foreach (var physicsLayer in layersToIgnore)
            {
                //Get all colliders in layer
                List<Collider> colliderObjects;
                if (!_layerColliders.TryGetValue(physicsLayer, out colliderObjects) || colliderObjects.Count == 0)
                    continue;

                //Ignore collision on each collider
                foreach (var layerCollider in colliderObjects)
                    foreach (var coll in colliders)
                        Physics.IgnoreCollision(layerCollider, coll, true);
            }
        }

        void IgnoreColliderCollisionOnLayers(Collider coll, IEnumerable<PhysicsLayer> layersToIgnore)
        {
            foreach (var physicsLayer in layersToIgnore)
            {
                //Get all colliders in layer
                List<Collider> colliderObjects;
                if (!_layerColliders.TryGetValue(physicsLayer, out colliderObjects) || colliderObjects.Count == 0)
                    continue;

                //Ignore collision on each collider
                foreach (var layerCollider in colliderObjects)
                    Physics.IgnoreCollision(layerCollider, coll, true);
            }
        } 
    }
}