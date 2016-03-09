using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Stats
{
    public enum StatsEnum
    {
        Health,
        Energy,
        Bullets,
        Stability
    }

    [Serializable]
    public class StatInfo
    {
        public StatsEnum Stat;
        public int MinValue;
        public int MaxValue;
        public int Value;
        public bool NotifyChange;
    }

    public class RegenInfo
    {
        public StatsEnum Stat;
        public int AddPerSec;
    }

    public class Stats : MonoBehaviour
    {
        public List<StatInfo> StatInfos;

        private readonly IDictionary<StatsEnum, StatInfo> _currentValues = new Dictionary<StatsEnum, StatInfo>();
        private readonly IList<RegenInfo> _regens = new List<RegenInfo>();  

        public void Start()
        {
            StatInfos.ForEach(si => _currentValues[si.Stat] = si);
            StartCoroutine(RegenTick());
        }

        public bool HasEnaugh(StatsEnum stat, int required)
        {
            if (required == 0) return true;

            if (!_currentValues.ContainsKey(stat))
            {
                Debug.LogErrorFormat("Following stat is not defined for this object: {0}", stat.ToString());
                return false;
            }

            return _currentValues[stat].Value >= required;
        }

        public bool IsStatDefined(StatsEnum stat)
        {
            return _currentValues.ContainsKey(stat);
        }

        public void AddAmount(StatsEnum stat, int amount)
        {
            if (!_currentValues.ContainsKey(stat))
            {
                Debug.LogErrorFormat("Following stat is not defined for this object: {0}", stat.ToString());
                return;
            }

            var statInfo = _currentValues[stat];
            var newValue = statInfo.Value + amount;

            var capedValue = Math.Min(statInfo.MaxValue, newValue);
            capedValue = Math.Max(capedValue, statInfo.MinValue);

            if (capedValue != statInfo.Value)
            {
                statInfo.Value = capedValue;
                if (statInfo.NotifyChange)
                {
                    this.GetPubSub().PublishMessageInContext(new StatChangedMessage(statInfo.Stat, capedValue));
                }
            }
        }

        public int GetValue(StatsEnum stat)
        {
            return _currentValues[stat].Value;
        }

        public void SetValue(StatsEnum stat, int value)
        {
            var statInfo = _currentValues[stat];
            statInfo.Value = value;
            if (statInfo.NotifyChange)
            {
                this.GetPubSub().PublishMessageInContext(new StatChangedMessage(statInfo.Stat, value));
            }
        }

        public void AddRegen(StatsEnum stat, int addPerSec, float timeInSeconds = -1)
        {
            var regen = new RegenInfo {Stat = stat, AddPerSec = addPerSec};
            _regens.Add(regen);
            if (timeInSeconds > 0)
            {
                StartCoroutine(DisableRegen(regen, timeInSeconds));
            }
        }

        public void ClearAllRegens()
        {
            _regens.Clear();
        }

        private IEnumerator DisableRegen(RegenInfo regen, float time)
        {
            yield return new WaitForSeconds(time);
            if (_regens.Contains(regen))
            {
                _regens.Remove(regen);
            }
        }

        private IEnumerator RegenTick()
        {
            while (true)
            {
                _regens.ForEach(r => AddAmount(r.Stat, r.AddPerSec));
                yield return new WaitForSeconds(1);
            }
        }
    }
}
