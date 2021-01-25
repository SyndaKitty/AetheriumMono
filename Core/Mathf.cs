using System;
using Microsoft.Xna.Framework;

namespace AetheriumMono.Core
{
    public static class Mathf
    {
        static Random random = new Random();

        public const float PI = 3.1415926535f;
        public const float TAU = PI * 2;

        public static float Cos(float theta)
        {
            return (float) Math.Cos(theta);
        }

        public static float Sin(float theta)
        {
            return (float) Math.Sin(theta);
        }

        public static float Random(float min, float max)
        {
            float range = max - min;
            return (float) random.NextDouble() * range + min;
        }

        public static Vector2 RandomUnit()
        {
            float theta = Random(0, TAU);
            return new Vector2(Cos(theta), Sin(theta));
        }

        // Bias valid from 0-1
        public static float Bias(float x, float bias)
        {
            float k = Pow(1 - bias, 3);
            return (x * k) / (x * k - x + 1);
        }

        public static float Pow(float x, float y)
        {
            return (float) Math.Pow(x, y);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }

        public static float Abs(float value)
        {
            if (value < 0) return -value;
            return value;
        }

        public static int Sign(float value)
        {
            if (value < 0) return -1;
            return Math.Abs(value) < float.Epsilon ? 0 : 1;
        }
    }
}
