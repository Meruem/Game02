using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class ForceMovementMessage : IMessage
    {
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; }
        public float ForwardTime { get; set; }
        public bool AllowOtherMovement { get; private set; }

        public ForceMovementMessage(Vector2 direction, float speed, float forwardTime, bool allowOtherMovement)
        {
            Direction = direction;
            Speed = speed;
            ForwardTime = forwardTime;
            AllowOtherMovement = allowOtherMovement;
        }
    }
}
