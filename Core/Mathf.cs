using System;

namespace AetheriumMono.Core
{
    public static class Mathf
    {
        static Random random = new Random();

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
            return (float)random.NextDouble() * range + min;
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
