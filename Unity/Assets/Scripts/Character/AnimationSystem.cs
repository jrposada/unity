using Moq;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface IAnimationSystem<T>
    {
        void Initialize(Animator animator, float ikDistance, LayerMask ikLayerMask, int leftFootIKWeightParameter, int rightFootIKWeightParameter);
        
        /// <summary>
        /// Initialize an animation and add it to the system.
        /// </summary>
        /// <param name="animation">Animation.</param>
        void Add(IAnimation animation);

        /// <summary>
        /// Remove animaiton from the system.
        /// </summary>        
        /// <param name="animation">Animation.</param>
        void Remove(IAnimation animation);
        void Update(AnimationData data);
        void OnAnimatorIK(int layerIndex, Vector3 forward);
    }

    public class AnimationSystem<T> : IAnimationSystem<T>
    {
        readonly protected struct IkInfo
        {
            /// <summary>
            /// Effective IK distance.
            /// </summary>
            public float Distance { get; }

            /// <summary>
            /// Layer mask of objects where to apply IK.
            /// </summary>
            public LayerMask LayerMask { get; }

            /// <summary>
            /// Left foot IK weight parameter id on the animator controller.
            /// </summary>
            public int LeftFootIKWeightParameter { get; }

            /// <summary>
            /// Right foot IK weight parameter id on the animator controller.
            /// </summary>
            public int RightFootIKWeightParameter { get; }

            public IkInfo(float distance, LayerMask layerMask, int leftFootIKWeightParameter, int rightFootIKWeightParameter)
            {
                Distance = distance;
                LayerMask = layerMask;
                LeftFootIKWeightParameter = leftFootIKWeightParameter;
                RightFootIKWeightParameter = rightFootIKWeightParameter;
            }

        }

        protected readonly Dictionary<int, IAnimation> animations = new();
        protected IkInfo ikInfo;
        protected Animator animator;

        /// <inheritdoc cref="IAnimationSystem{T}.Initialize(Animator, float, LayerMask, int, int)"/>>

        public virtual void Initialize(Animator animator, float ikDistance, LayerMask ikLayerMask, int leftFootIKWeightParameter, int rightFootIKWeightParameter)
        {
            this.animator = animator;
            ikInfo = new IkInfo(ikDistance, ikLayerMask, leftFootIKWeightParameter, rightFootIKWeightParameter);
        }

        /// <inheritdoc cref="IAnimationSystem{T}.Add(IAnimation)"/>>
        public virtual void Add(IAnimation animation)
        {
            foreach (int hash in animation.Hashes)
            {
                animation.Initialize(animator);
                animations.Add(hash, animation);
            }
        }

        /// <inheritdoc cref="IAnimationSystem{T}.Remove(IAnimation))"/>>
        public virtual void Remove(IAnimation animation)
        {
            foreach (int hash in animation.Hashes)
            {
                animations.Remove(hash);
            }
        }

        /// <inheritdoc cref="IAnimationSystem{T}.Update(AnimationData))"/>>
        public virtual void Update(AnimationData data)
        {
            int hash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (animations.ContainsKey(hash))
            {
                animations[hash]?.Update(data);
            }
        }

        /// <inheritdoc cref="IAnimationSystem{T}.OnAnimatorIK(int, Vector3))"/>>
        public virtual void OnAnimatorIK(int layerIndex, Vector3 transformForward)
        {
            FootIk(AvatarIKGoal.LeftFoot, ikInfo.LeftFootIKWeightParameter, animator.leftFeetBottomHeight, transformForward);
            FootIk(AvatarIKGoal.RightFoot, ikInfo.RightFootIKWeightParameter, animator.rightFeetBottomHeight, transformForward);

            int hash = animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;
            if (animations.ContainsKey(hash))
            {
                animations[hash]?.OnAnimatorIK(layerIndex, transformForward);
            }
        }

        private void FootIk(AvatarIKGoal goal, int paramenter, float bottomHeight, Vector3 transformForward)
        {
            animator.SetIKPositionWeight(goal, animator.GetFloat(paramenter));
            animator.SetIKRotationWeight(goal, animator.GetFloat(paramenter));

            if (Physics.Raycast(animator.GetIKPosition(goal) + Vector3.up, Vector3.down, out RaycastHit hit, ikInfo.Distance+ 1f, ikInfo.LayerMask))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += bottomHeight;
                Vector3 forward = Vector3.ProjectOnPlane(transformForward, hit.normal);
                animator.SetIKPosition(goal, footPosition);
                animator.SetIKRotation(goal, Quaternion.LookRotation(forward, hit.normal));
            }
        }
    }
}
