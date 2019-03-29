using System;

namespace Lib
{
    public class Methods
    {
        public static int[][] FormIntMatrix(int numberCity)
        {
            int[][] dist = new int[numberCity][];
            for (int i = 0; i < numberCity; i++)
            {
                dist[i] = new int[numberCity];
            }
            return dist;
        }

        public static double[][] FormDoubleMatrix(int numberCity)
        {
            double[][] dist = new double[numberCity][];
            for (int i = 0; i < numberCity; i++)
            {
                dist[i] = new double[numberCity];
            }
            return dist;
        }

        public static void GenerateRandomDistances(double[][] dist)
        {

            for (int i = 0; i < dist.GetLength(0); i++)
            {

                for (int j = 0; j <= i; j++)
                {
                    if (i == j) { dist[i][j] = 0; }
                    else
                    {
                        int randDistance = RandomNumbers.NewDistance();
                        dist[i][j] = randDistance;
                        dist[j][i] = randDistance;
                    }
                }
            }
        }

        public static void GenerateRandomPheromones(double[][] pheromones)
        {

            for (int i = 0; i < pheromones.GetLength(0); i++)
            {

                for (int j = 0; j <= i; j++)
                {
                    if (i == j) { pheromones[i][j] = 0; }
                    else
                    {
                        double beginPheromones = 0.01;
                        pheromones[i][j] = beginPheromones;
                        pheromones[j][i] = beginPheromones;
                    }
                }
            }
        }

        public static void ShowAdjensencyMatrixInt(int[][] dist)
        {
            Console.Write($"---");
            for (int i = 0; i < dist.GetLength(0); i++)
            {
                Console.Write($"[{i + 1}]-");
            }
            Console.WriteLine();
            for (int i = 0; i < dist.GetLength(0); i++)
            {
                Console.Write($"[{i + 1}]");

                for (int j = 0; j < dist.GetLength(0); j++)
                {

                    Console.Write($" {dist[i][j]}  ");
                }
                Console.WriteLine();
            }

        }

        public static void ShowAdjensencyMatrixDouble(double[][] dist)
        {
            Console.WriteLine();

            Console.Write($"-------");
            for (int i = 0; i < dist.GetLength(0); i++)
            {
                Console.Write($"[{i + 1}] -");
            }
            Console.WriteLine();
            for (int i = 0; i < dist.GetLength(0); i++)
            {
                Console.Write($"[{i + 1}] ");

                for (int j = 0; j < dist.GetLength(0); j++)
                {
                    if (dist[i][j] < 100000)
                        Console.Write($"{dist[i][j],5:f0} ");
                    else Console.Write($"{"inf",5} ");
                }
                Console.WriteLine();
            }
        }

        public static void ShowPheremonesMatrix( double[][] pheremones)
        {
            for( int i=0; i < pheremones.GetLength(0); i++)
            {
                for( int j = 0; j < pheremones.GetLength(0); j++)
                {
                    Console.Write($"{pheremones[i][j]:f4} ");
                }
                Console.WriteLine();
                Console.WriteLine("-------------------");
            }
        }

        public static void CopyJaggedArray(double[][] sourseArr, double[][] directArr)
        {
            directArr = new double[sourseArr.Length][];
            for (int i = 0; i < sourseArr.Length; i++)
            {
                directArr[i] = new double[sourseArr.Length];
            }
            sourseArr.CopyTo(directArr, 0);
        }

    }
}
