using UnityEngine;

namespace Character
{
    public struct MovementData
    {
        /// <summary>
        /// Gets the horizontal input.
        /// </summary>
        public float Horizontal { get; set; }

        /// <summary>
        /// Gets the vertical input.
        /// </summary>
        public float Vertical { get; set; }

        /// <summary>
        /// Gets whether it is grounded or not.
        /// </summary>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// Elapsed time.
        /// </summary>
        public float Time { get; set; }
    }

    public interface IMovement
    {
        /// <summary>
        /// Calculate movement based on time elasped.
        /// </summary>
        /// <param name="data">Input information.</param>
        /// <param name="velocity">Calculated velocity.</param>
        void CalculateVelocity(MovementData data, ref Vector3 velocity);

        void Enter();
        bool TryExit();
    }
}
