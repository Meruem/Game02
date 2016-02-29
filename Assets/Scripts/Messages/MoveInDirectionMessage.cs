using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class MoveInDirectionMessage : IMessage
    {
        public Vector2 Direction { get; private set; }
        public bool UseMaxSpeed { get; private set; }
        public float Speed { get; private set; }

        public MoveInDirectionMessage(Vector2 direction, bool useMaxSpeed = true, float speed = 0)
        {
            Direction = direction;
            UseMaxSpeed = useMaxSpeed;
            Speed = speed;
        }
    }
}
