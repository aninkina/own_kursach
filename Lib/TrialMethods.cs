using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class TrialMethods
    {

        // Возвращает  лист индексов кластеров
        public static List<int> IdentifyCluster(int targetCity, Cluster[] clusters)
        {
            // Лист хранит индексы кластеров
            List<int> someClusters = new List<int>();

            // Бежим по кластерам,ищем те, в которых лежит targetCity
            for (int i = 0; i < clusters.Length; i++)
                if (clusters[i].IsExisted(targetCity))
                    someClusters.Add(i);

            return someClusters.Count > 0 ? someClusters : throw new Exception("каждый город должен состоять в кластере!");
        }


        public static bool IsSomethingCommon(List<int> cities1, List<int> cities2)
        {
            for (int i = 0; i < cities1.Count; i++)
            {
                if (cities2.IndexOf(cities1[i]) > 0) { return true; }
            }
            return false;
        }
        // ERROR
        public static void FindBest(double[][] pheremones, double[][] dist, Cluster[] cl)
        {
            //
            // Choose first city
            double maxPheromone = 0;
            int indexMaxLine = -1;
            int indexMaxColumn = -1;
            for (int i = 0; i < cl[0].Length; i++)
            {
                Console.WriteLine($"el [{i}]={cl[0].Element(i)}");
                for (int j = 0; j < pheremones.GetLength(0); j++)
                {
                    if (!IsSomethingCommon(IdentifyCluster(cl[0].Element(i), cl), IdentifyCluster(j, cl)))
                        if (maxPheromone < pheremones[cl[0].Element(i)][j])
                        { maxPheromone = pheremones[cl[0].Element(i)][j]; indexMaxLine = cl[0].Element(i); indexMaxColumn = j; }
                }
            }
            // Нашли максимальный след из первого кластера в другой
            Trial obj = new Trial(indexMaxLine, cl);
            obj.AddCity(indexMaxColumn);
            while (!obj.IsTrialCompleted())
            {
                int indexMax = -1;
                double max = 0;
                for (int i = 0; i < pheremones.GetLength(0); i++)
                {
                    if (obj.IsAllowedCity(i))
                        if (max < pheremones[obj.currentCity][i]) { max = pheremones[obj.currentCity][i]; indexMax = i; }
                }
                obj.AddCity(indexMax);
            }
            Console.WriteLine("The Best trial");
            obj.ShowTrial();
            Console.Write($"The length:{obj.GetTrialLength(dist)} ");
            Console.WriteLine();

        }

    }
}
