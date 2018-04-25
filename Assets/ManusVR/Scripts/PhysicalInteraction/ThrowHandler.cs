// Copyright (c) 2018 ManusVR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public class ThrowHandler : MonoBehaviour
    {
        private struct TrajectoryNode
        {
            public int Index;
            public Vector3 Position;
            public float Timestep;
        }

        public device_type_t DeviceType;
        [SerializeField] private bool _showInitialTrajectory = false;
        [SerializeField] private Color _initialTrajectoryColor = Color.red;
        [SerializeField] private bool _showAssistedTrajectory = false;
        [SerializeField] private Color _assistedTrajectoryColor = Color.green;

        private LineRenderer _initialTrajectoryLineRenderer;
        private LineRenderer _assistedTrajectoryLineRenderer;
        private Material _lineRendererMaterial;

        private PhysicsHand _physicsRegularHand;
        private const int _frames = 7;
        private const int _VelocityMultiplier = 130;
        private Queue<Vector3> _historyPositions = new Queue<Vector3>();
        private Vector3 _throwDirection;

        //Coroutine status variables
        private Coroutine _initialTrajectoryPlotCoroutine;
        private Coroutine _assistedTrajectoryPlotCoroutine;

        void Start()
        {
            var controllers = GetComponents<PhysicsHand>();
            foreach (var controller in controllers)
                if (controller.DeviceType == DeviceType)
                    _physicsRegularHand = controller;

            _lineRendererMaterial = Resources.Load<Material>("LineMaterial");

            //TODO: Which gameobject should have these line renderers?
            if (_showInitialTrajectory)
                _initialTrajectoryLineRenderer = InitializeLineRenderer(new GameObject(), _initialTrajectoryColor);
            if (_showAssistedTrajectory)
                _assistedTrajectoryLineRenderer = InitializeLineRenderer(new GameObject(), _assistedTrajectoryColor);
        }

        /// <summary>
        /// Update the throwdirection
        /// </summary>
        void Update()
        {
            _historyPositions.Enqueue(_physicsRegularHand.Target.WristTransform.position);
            if (_historyPositions.Count > _frames)
            {
                _historyPositions.Dequeue();
                Vector3 oldPos = Vector3.zero;
                foreach (var position in _historyPositions)
                {
                    if (oldPos != Vector3.zero)
                    {
                        _throwDirection = position - oldPos;
                        break;
                    }
                    oldPos = position;
                    _throwDirection = _throwDirection / _frames;
                }
            }
        }

        /// <summary>
        /// Called by ObjectGrabber when a grabbed object gets released.
        /// </summary>
        /// <param name="rb">The released rigidbody</param>
        public void OnObjectRelease(Rigidbody rb)
        {
            if (_throwDirection.magnitude > 0.01f)
            {
                rb.velocity = _throwDirection * _VelocityMultiplier;
                Throwcedure(rb);
            }
        }

        /// <summary>
        /// Called when throw is started. Gets the trajectory, calls the render function, calls the aim assist function if necessary.
        /// </summary>
        /// <param name="rb"></param>
        //Todo: rename
        private void Throwcedure(Rigidbody rb)
        {
            var timeStep = 0.05f;
            TrajectoryNode[] trajectory = GetRigidbodyTrajectory(rb, timeStep);

            if (_showInitialTrajectory)
            {
                if (_initialTrajectoryPlotCoroutine != null)
                    StopCoroutine(_initialTrajectoryPlotCoroutine);
                _initialTrajectoryPlotCoroutine = StartCoroutine(PlotThrowTrajectory(trajectory,
                    _initialTrajectoryLineRenderer, () => _initialTrajectoryPlotCoroutine = null));
            }

            if (TryAimAssist(rb, trajectory))
            {
                trajectory = GetRigidbodyTrajectory(rb, timeStep);
                if (_showAssistedTrajectory)
                {
                    if (_assistedTrajectoryPlotCoroutine != null)
                        StopCoroutine(_assistedTrajectoryPlotCoroutine);
                    _assistedTrajectoryPlotCoroutine = StartCoroutine(PlotThrowTrajectory(trajectory,
                        _assistedTrajectoryLineRenderer, () => _assistedTrajectoryPlotCoroutine = null));
                }
            }
        }

        /// <summary>
        /// Checks if throw is within assist range of any possible throw target;
        /// If so it adjusts the velocity to hit the target.
        /// </summary>
        /// <param name="rb">Rigidbody being thrown</param>
        /// <param name="trajectory">The initial trajectory of the rigidbody</param>
        private bool TryAimAssist(Rigidbody rb, TrajectoryNode[] trajectory)
        {
            if (ThrowTarget.Instances == null)
                return false;

            foreach (var target in ThrowTarget.Instances)
            {
                TrajectoryNode node;
                if (ShouldAssistThrow(trajectory, target, out node))
                {
                    var newVelocity = CalculateNewVelocity(trajectory[0].Position,
                        target.TargetCollider.ClosestPoint(node.Position), node.Timestep);

                    rb.velocity = newVelocity;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculate the trajectory of a rigidbody
        /// </summary>
        /// <param name="rb">The rigidbody of which to get the trajectory</param>
        /// <param name="timeStep">The interval between two nodes in the calculated trajectory</param>
        /// <returns>An array of trajectorynodes which make up the trajectory</returns>
        private TrajectoryNode[] GetRigidbodyTrajectory(Rigidbody rb, float timeStep)
        {
            TrajectoryNode[] trajectory = new TrajectoryNode[100];
            for (int i = 0; ; i++)
            {
                trajectory[i] = new TrajectoryNode
                {
                    Index = i,
                    Position = PlotTrajectoryAtTime(rb.position, rb.velocity, i * timeStep),
                    Timestep = i * timeStep
                };

                if (i > 0 && Physics.Linecast(trajectory[i - 1].Position, trajectory[i].Position, 1 << LayerMask.NameToLayer("Default")))
                {
                    Array.Copy(trajectory, trajectory = new TrajectoryNode[i], i);
                    return trajectory;
                }
            }
        }

        /// <summary>
        /// Visually plots a trajectory.
        /// </summary>
        private IEnumerator PlotThrowTrajectory(TrajectoryNode[] trajectory, LineRenderer lineRenderer, Action cleanupCallback)
        {
            var waitForSeconds = new WaitForSeconds(trajectory.Last().Timestep);

            lineRenderer.enabled = true;
            lineRenderer.positionCount = trajectory.Length;

            foreach (var trajectoryNode in trajectory)
            {
                lineRenderer.SetPosition(trajectoryNode.Index, trajectoryNode.Position);
            }
            yield return waitForSeconds;
            lineRenderer.enabled = false;
            cleanupCallback.Invoke();
        }

        /// <summary>
        /// Initialize the values for the line renderer
        /// </summary>
        private LineRenderer InitializeLineRenderer(GameObject target, Color lineRendererColor)
        {
            LineRenderer line = target.AddComponent<LineRenderer>();
            line.enabled = true;
            line.positionCount = 10;
            line.startWidth = 0.01f;
            line.material = new Material(_lineRendererMaterial) {color = lineRendererColor};
            line.endWidth = 0.01f;
            return line;
        }

        /// <summary>
        /// Checks if aim assist should activate.
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="target"></param>
        /// <param name="closestNode">The closest descending node to the <paramref name="target"/></param>
        /// <returns></returns>
        private bool ShouldAssistThrow(TrajectoryNode[] trajectory, ThrowTarget target, out TrajectoryNode closestNode)
        {
            //Get all nodes in range of the target, where the node is descending compared to the previous node. Ascending nodes are ignored because they don't indicate a good trajectory.
       
            var possibleNodes = trajectory
                .Where(n => n.Index > 0 
                            && trajectory[n.Index - 1].Position.y > trajectory[n.Index].Position.y 
                            && Vector3.Distance(n.Position, target.TargetCollider.ClosestPoint(n.Position)) < target.MaxAssistDistance).ToArray();

            //No potential nodes found
            if (possibleNodes.Length == 0)
            {
                closestNode = trajectory[0];
                return false;
            }

            //Get closest potential node
            closestNode = possibleNodes
                .OrderBy(n => Vector3.Distance(n.Position, target.TargetCollider.ClosestPoint(n.Position)))
                .FirstOrDefault();
        
            return true;
        }

        /// <summary>
        /// Calculate the velocity required for a rigidbody to move from <paramref name="start"/> to <paramref name="targetPosition"/> in <paramref name="time"/> seconds.
        /// </summary>
        private Vector3 CalculateNewVelocity(Vector3 start, Vector3 targetPosition, float time)
        {
            return (targetPosition - Physics.gravity * time * time * 0.5f - start) / time;
        }

        /// <summary>
        /// Calculate the position a rigidbody will move to in <paramref name="time"/> seconds.
        /// </summary>
        /// <param name="start">Initial position</param>
        /// <param name="startVelocity">Initial velocity</param>
        /// <param name="time">Time since start of throw to calculate the new position for</param>
        /// <returns></returns>
        private Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
        }
    }
}