using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class MoveInDirectionMessage : IMessage
    {
        public Vector2 Direction { get; private set; }

        public MoveInDirectionMessage(Vector2 direction)
        {
            Direction = direction;
        }
    }
}
