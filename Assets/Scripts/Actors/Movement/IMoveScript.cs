using UnityEngine;

namespace Assets.Scripts
{
    public interface IMoveScript
    {
        void MoveNormal(Vector2 direction, float speed);
        void MoveMaxSpeed(Vector2 direction);
    }
}