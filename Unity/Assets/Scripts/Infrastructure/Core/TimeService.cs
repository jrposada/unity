using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// Interface to get time information from Unity.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// The time at the beginning of this frame (Read Only). This is the time in seconds
        /// since the start of the game.
        /// </summary>
        float Time { get; }

        /// <summary>
        /// The interval in seconds at which physics and other fixed frame rate updates (like
        /// MonoBehaviour's MonoBehaviour.FixedUpdate) are performed (Read Only).
        /// </summary>
        float FixedDeltaTime { get; }

        /// <summary>
        /// The completion time in seconds since the last frame (Read Only).
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// The total number of frames that have passed (Read Only).
        /// </summary>
        int FrameCount { get; }
    }

    public class TimeService : ITimeService
    {
        /// <inheritdoc cref="ITimeService.Time"/>
        public float Time => UnityEngine.Time.time;

        /// <inheritdoc cref="ITimeService.FixedDeltaTime"/>
        public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        /// <inheritdoc cref="ITimeService.DeltaTime"/>
        public float DeltaTime => UnityEngine.Time.deltaTime;

        /// <inheritdoc cref="ITimeService.FrameCount"/>
        public int FrameCount => UnityEngine.Time.frameCount;
    }


    public class TimeServiceBehaviour : MonoBehaviour, ITimeService
    {
        private readonly ITimeService timeService = new TimeService();

        /// <inheritdoc cref="ITimeService.Time"/>
        public float Time => timeService.Time;

        /// <inheritdoc cref="ITimeService.FixedDeltaTime"/>
        public float FixedDeltaTime => timeService.FixedDeltaTime;

        /// <inheritdoc cref="ITimeService.DeltaTime"/>
        public float DeltaTime => timeService.DeltaTime;

        /// <inheritdoc cref="ITimeService.FrameCount"/>
        public int FrameCount => timeService.FrameCount;

    }
}