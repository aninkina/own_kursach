using System;

namespace Lib
{
    public class RandomNumbers
    {
        public static Random GetRandom;

        static RandomNumbers()
        {
            GetRandom = new Random();
        }

        public static int NewDistance()
        {
            return GetRandom.Next(1, 10);
        }

        public static double RandProbability()
        {
            return GetRandom.NextDouble();
        }
    }
}
