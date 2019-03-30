using System;

namespace Lib
{
    public class RandomNumbers
    {
        // Рандомит следующий город
        public static int GetChosenCity(double[] probabilities)
        {
            double rand = RandomNumbers.RandProbability();
            double temp = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                temp += probabilities[i];
                if (rand <= temp) { return i; }

            }
            return -1; // Никогда не должен сработать
        }


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
