using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace own_kursach
{

    class Program
    {

        const double forgetRate = 0.5;

        const int alpha = 1;

        const int beta = 1;

        const double distanceImportance = 2;

        const double inf = 100000000;

        const int numberCity = 6;

        const int maxTime = 10;

        public static void GenerateZero(double[][] arr)
        {
            for (int i = 0; i < numberCity; i++)
            {
                for (int j = 0; j < numberCity; j++)
                {
                    if (i == j) { arr[i][j] = 1; }
                    else { arr[i][j] = 1; }
                }
            }
        }

        static void Main(string[] args)
        {
            do
            {
                // Поменяли вызов на матрицу Matrix
                // Не отличаю  прочерка от бесконечности
                // Добавила L в ChangePheromones ничего не изменилось ( deltaT сравнима с newT)
                // Работает раз через раз 
                // Думаю как изменить на ореинтированный
                // Не понимаю приколов с M
                // Добавлен zero в Gtspp, чтобы не писать условия на ув. фер. слоя

                Console.Clear();
                   double[][] matrix = new double[6][];
                   matrix[0] = new double[] { inf, inf, inf, 7, inf, inf };
                   matrix[1] = new double[] { inf, inf, inf, inf, 1, inf };
                   matrix[2] = new double[] { inf, inf, inf, 4, inf, inf };
                   matrix[3] = new double[] { inf, inf, inf, inf, inf, 5 };
                   matrix[4] = new double[] { inf, inf, inf, inf, inf, 2 };
                   matrix[5] = new double[] { 6, 3, 8, inf, inf, inf };

                /*double[][] matrix = new double[6][];
                matrix[0] = new double[] { 1000000, 1, 2, 3, 9, 6 };
                matrix[1] = new double[] { 1, 1000000, 3, 2, 8, 7 };
                matrix[2] = new double[] { 2, 3, 1000000, 5, 9, 8 };
                matrix[3] = new double[] { 3, 2, 5, 1000000, 6, 9 };
                matrix[4] = new double[] { 9, 8, 9, 6, 1000000, 15 };
                matrix[5] = new double[] { 6, 7, 8, 9, 15, 1000000 };
                */

                // Надо заменить на этот клстер всё
                Cluster[] cl = new Cluster[3];
                cl[0] = new Cluster(new int[]{0,1,2});
                cl[1] = new Cluster(new int[] { 3,4});
                cl[2] = new Cluster(new int[] { 5 });

                int[] clusters = { 3,2,1};

                Methods.ShowAdjensencyMatrixDouble(matrix);
                double[][] pheromones = Methods.FormDoubleMatrix(numberCity);
                Methods.GenerateRandomPheromones(pheromones);

                for (int i = 0; i < maxTime; i++)
                {
                    Trial.ChangePheromones(pheromones, matrix, forgetRate, distanceImportance, alpha, beta, cl);
                }

                Methods.ShowPheremonesMatrix(pheromones);
               
                TrialMethods.FindBest(pheromones, matrix, cl);
                Console.WriteLine("ESC");
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
