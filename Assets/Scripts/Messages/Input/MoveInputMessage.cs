using UnityEngine;

namespace Assets.Scripts.Messages.Input
{
    public class MoveInputMessage : IMessage
    {
        public Vector2 Direction { get; private set; }

        public MoveInputMessage(Vector2 direction)
        {
            Direction = direction;
        }
    }
}
