using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace own_kursach
{

    class Program
    {
        const int numberCity = 5;

        const int maxTime = 50;



        static void Main(string[] args)
        {
            do
            {
                double[][] dist = Methods.FormDoubleMatrix(numberCity);

                double[][] pheromones = Methods.FormDoubleMatrix(numberCity);


                Methods.GenerateRandomDistances(dist);

                Methods.GenerateRandomPheromones(pheromones);

                Methods.ShowAdjensencyMatrixDouble(dist);



                for ( int i = 0; i < maxTime; i++)
                {
                    Trial.ChangePheromones(pheromones, dist,0);
                }
                
                Methods.ShowAdjensencyMatrixDouble(pheromones);
                Trial.FindTheBestTrial(pheromones, 0, numberCity, dist);

                Console.WriteLine("ESC");
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
