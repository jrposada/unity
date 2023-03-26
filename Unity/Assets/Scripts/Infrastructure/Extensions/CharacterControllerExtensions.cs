using UnityEngine;

namespace Infrastructure
{
    public static class CharacterControllerExtensions
    {
        // 
        /// <summary>
        /// Gets the center point of the bottom hemisphere of the character controller capsule.
        /// </summary>
        /// <param name="characterController">This.</param>
        /// <param name="offset">Vertical offset.</param>
        /// <returns></returns>
        public static Vector3 GetBottomHemisphere(this CharacterController characterController, float offset = 0)
        {
            Vector3 bottom = characterController.transform.position + characterController.center;
            bottom.y -= characterController.height * 0.5f - characterController.radius - offset;
            return bottom;
        }

        /// <summary>
        /// Gets the center point of the top hemisphere of the character controller capsule.
        /// </summary>
        /// <param name="characterController">This.</param>
        /// <param name="offset">Vertical offset.</param>
        /// <returns></returns>
        public static Vector3 GetTopHemisphere(this CharacterController characterController, float offset = 0)
        {
            Vector3 bottom = characterController.transform.position + characterController.center;
            bottom.y -= characterController.height * 0.5f - characterController.radius - offset;
            return bottom;
        }

        // 

        /// <summary>
        /// Returns whether the angle with a plane is under the slope angle limit.
        /// </summary>
        /// <param name="normal">Plane's normal.</param>
        /// <returns></returns>
        public static bool IsNormalUnderSlopeLimit(this CharacterController controller, Vector3 normal)
        {
            // TODO: find a way to use slope but perform in stairs.
            return Vector3.Angle(controller.transform.up, normal) <= controller.slopeLimit;
        }
    }
}
