using UnityEngine;

namespace Assets.Scripts.LevelObjectives
{
    public class KillAllMonstersObjective : ILevelObjective
    {
        private readonly GameObject _monsterGroup;

        public KillAllMonstersObjective(GameObject monsterGroup)
        {
            _monsterGroup = monsterGroup;
        }

        public bool IsCompleted()
        {
            return _monsterGroup.transform.childCount == 0;
        }
    }
}
