﻿using System;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Math2
    {
        public static Vector2 AngleRadToVector(float angle)
        {
            return new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)).normalized;
        }

        public static Vector2 AngleDegToVector(float angle)
        {
            return AngleRadToVector(Mathf.Deg2Rad * angle);
        }
    }
}
