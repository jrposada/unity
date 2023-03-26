using UnityEngine;

namespace Character
{
    public class WasdMovment : IMovement
    {
        public float SideSpeed = 1;
        public float ForwardSpeed = 1;

        public void CalculateVelocity(MovementData data, ref Vector3 velocity)
        {
            velocity.x += SideSpeed * data.Horizontal;
            velocity.z += ForwardSpeed * data.Vertical;
        }

        public void Enter()
        {
            //throw new System.NotImplementedException();
        }

        public bool TryExit()
        {
            //throw new System.NotImplementedException();
            return true;
        }
    }
}
