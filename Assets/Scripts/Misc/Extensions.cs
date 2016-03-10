using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) return;

            var list = enumerable as IList<T>;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    action(list[i]);
                }

                return;
            }

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static void StartAfterTime(this MonoBehaviour mb, Action a, float time)
        {
            mb.StartCoroutine(AfterTimeCourutine(a, time));
        }

        private static IEnumerator AfterTimeCourutine(Action a, float time)
        {
            yield return new WaitForSeconds(time);
            a();
        } 
    }
}
