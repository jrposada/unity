using Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    /**
 * Logic to move a CharacterController.
 */
    public interface IMovementSystem<T>
    {
        /// <summary>
        /// Current state.
        /// </summary>
        T State { get; }

        /// <summary>
        /// Gets current velocity magnitude.
        /// </summary>
        float Speed { get; }

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="controller">Character controller to control.</param>
        /// <param name="groundLayerMask">Ground layer mask.</param>
        /// <param name="maxDistanceToGround">Maximum distance from the ground to be considerer as grounded.</param>
        void Initialize(CharacterController controller, LayerMask groundLayerMask, float maxDistanceToGround = 0.3f);

        /// <summary>
        /// Adds a new movement.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="movement">Movement logic.</param>
        void Add(T state, IMovement movement);

        /// <summary>
        /// Removes a movement.
        /// </summary>
        /// <param name="state">Linked state.</param>
        void Remove(T state);
        
        /// <summary>
        /// Tries to change current state.
        /// </summary>
        /// <param name="state">New state.</param>
        /// <param name="force">Ignore current state transition.</param>
        /// <returns>Whether or not the state was successful.</returns>
        bool ChangeState(T state, bool force = false);

        Action OnStateChangeEvent { get; set; }
        bool IsGrounded { get; }
        Action OnIsGroundedChangeEvent { get; set; }
        bool IsHittingWall { get; }

        void OnUpdate(MovementData data);
        void SetSnapping(bool value);
    }

    public class MovementSystem<T> : IMovementSystem<T>
    {
        private class GroundInfo
        {
            public bool IsHit { get; set; }
            public Vector3 Normal { get; set; }
            public bool IsUnderSlopeLimit { get; set; }
            public bool IsGrounded { get; set; }

        }

        public T State { get; private set; }

        public Action OnStateChangeEvent { get; set; }

        public bool IsGrounded => groundInfo.IsGrounded;

        public Action OnIsGroundedChangeEvent { get; set; }

        public bool IsHittingWall => controller.collisionFlags == CollisionFlags.Sides;

        public float Speed => controller.velocity.magnitude;

        protected readonly Dictionary<T, IMovement> movements = new Dictionary<T, IMovement>();
        private GroundInfo groundInfo = new();
        protected CharacterController controller;

        private const float k_gravityFallMult = 2f;
        private const float k_gravityIncreasedMult = 1.5f;

        private LayerMask groundLayerMask;
        private float maxDistanceToGround = 1.3f;

        private bool m_isInvokeOnStateChangeEvent;
        private bool m_isInvokeOnIsGroundedChangeEvent;

        private Vector3 m_velocity;
        private Vector3 m_slideVelocity;
        private bool m_isSnapping = true;

        /// <inheritdoc cref="IMovementSystem.Initialize(CharacterController, LayerMask, float)"/>
        public virtual void Initialize(CharacterController controller, LayerMask groundLayerMask, float maxDistanceToGround)
        {
            this.controller = controller;
            this.groundLayerMask = groundLayerMask;
            this.maxDistanceToGround = maxDistanceToGround;
        }

        public virtual void Add(T state, IMovement movement)
        {
            movements.Add(state, movement);
        }

        public virtual void Remove(T state)
        {
            movements.Remove(state);
        }

        public virtual bool ChangeState(T state, bool force = false)
        {
            bool stateChanged = false;

            if (!State.Equals(state))
            {
                if (force || (movements[State]?.TryExit() ?? true))
                {
                    State = state;
                    movements[State]?.Enter();
                    m_isInvokeOnStateChangeEvent = true;
                    stateChanged = true;
                }
            }

            return stateChanged;
        }

        public virtual void OnUpdate(MovementData data)
        {
            UpdateGroundInfo();
            SimulateGravity(data.IsGrounded, data.Time);
            Move(data);
            NotifyChanges();
        }

        public virtual void SetSnapping(bool value)
        {
            m_isSnapping = value;
        }

        private void UpdateGroundInfo()
        {
            //Debug.DrawRay(controller.GetBottomHemisphere(controller.skinWidth - controller.radius), Vector3.down * ((2 * controller.skinWidth) + maxDistanceToGround), Color.green);
            groundInfo.IsHit = DetectGround(out RaycastHit hit);
            groundInfo.Normal = hit.normal;
            groundInfo.IsUnderSlopeLimit = controller.IsNormalUnderSlopeLimit(hit.normal);


            bool isGrounded = false;
            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up,
            // the slope angle is lower than the character controller's limit and the character is not moving up.
            if (
                groundInfo.IsHit &&
                Vector3.Dot(hit.normal, controller.transform.up) > 0f &&
                groundInfo.IsUnderSlopeLimit &&
                m_velocity.y <= 0)
            {
                isGrounded = true;

                if (m_isSnapping && hit.distance > controller.skinWidth)
                {
                    // Snap to the ground.
                    controller.Move(Vector3.down * (hit.distance - controller.skinWidth));
                }
            }

            if (isGrounded != groundInfo.IsGrounded)
            {
                groundInfo.IsGrounded = isGrounded;
                m_isInvokeOnIsGroundedChangeEvent = true;
            }
        }


        private void SimulateGravity(bool isIncreaseGravity, float time)
        {
            if (!IsGrounded)
            {
                if (controller.velocity.y > 0 && isIncreaseGravity)
                {
                    m_velocity.y += Physics.gravity.y * time * k_gravityIncreasedMult;
                }
                else if (controller.velocity.y <= 0)
                {
                    m_velocity.y += Physics.gravity.y * time * k_gravityFallMult;
                }
                else
                {
                    m_velocity.y += Physics.gravity.y * time;
                }
            }
        }

        private void Move(MovementData data)
        {
            m_velocity.x = m_velocity.z = 0;
            movements[State]?.CalculateVelocity(data, ref m_velocity);

            if (groundInfo.IsHit && !groundInfo.IsUnderSlopeLimit)
            {
                m_slideVelocity.y = 0;
                m_slideVelocity.x = m_velocity.x;
                m_slideVelocity.z = m_velocity.z;
                m_slideVelocity += Vector3.ProjectOnPlane(Vector3.up * m_velocity.y, groundInfo.Normal);

                controller.Move(m_slideVelocity * data.Time);
            }
            else
            {
                controller.Move(m_velocity * data.Time);
            }
        }

        private void NotifyChanges()
        {
            if (m_isInvokeOnStateChangeEvent)
            {
                m_isInvokeOnStateChangeEvent = false;
                OnStateChangeEvent?.Invoke();
            }

            if (m_isInvokeOnIsGroundedChangeEvent)
            {
                m_isInvokeOnIsGroundedChangeEvent = false;
                OnIsGroundedChangeEvent?.Invoke();
            }
        }

        /// <summary>
        /// Cast character controller capsule for a distance of maxDistanceToGround.
        /// </summary>
        /// <param name="hit">Hit information.</param>
        /// <returns>Whether there was a hit or not.</returns>
        private bool DetectGround(out RaycastHit hit)
        {
            return Physics.CapsuleCast(
                controller.GetBottomHemisphere(),
                controller.GetTopHemisphere(),
                controller.radius,
                Vector3.down,
                out hit,
                controller.skinWidth + maxDistanceToGround,
                groundLayerMask,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
