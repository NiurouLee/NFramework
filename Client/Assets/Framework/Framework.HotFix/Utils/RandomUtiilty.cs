using System;

namespace NFramework.Utils
{
    public static class RandomUtil
    {
        public static Random Random = new Random();

        public static int RandomNumber(int lower, int upper)
        {
            return Random.Next(lower, upper);
        }

        public static float RandomRate()
        {
            return Random.Next(1, 101);
        }
    }
}