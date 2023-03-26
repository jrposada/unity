using UnityEngine;

namespace Character
{
    /// <summary>
    /// Animator controller variables.
    /// </summary>
    public struct AnimationData
    {
        /// <summary>
        /// Gets or sets the speed on Z axis. From 0 to 1.
        /// </summary>
        public float SpeedZ { get; set; }

        /// <summary>
        /// Gets or sets the speed on X axis. From 0 to 1.
        /// </summary>
        public float SpeedX { get; set; }
        
        /// <summary>
        /// Gets or sets the alertness. From 0 to 1.
        /// </summary>
        public float Alertness { get; set; }

        //bool IsGrounded { get; set; }
        //float Horizontal { get; set; }
        //float Vertical { get; set; }
        //bool IsSprint { get; set; }
        //bool IsWalk { get; set; }
        //bool IsJump { get; set; }
        //float MovementSpeed { get; set; }
        //bool IsDash { get; set; }
        //bool IsWallJump { get; set; }
    }

    public interface IAnimation
    {
        /// <summary>
        /// Animator full path hashs.
        /// </summary>
        int[] Hashes { get; }

        void Initialize(Animator animator);
        void Update(AnimationData data);
        void OnAnimatorIK(int layerIndex, Vector3 transformForward);
    }
}
