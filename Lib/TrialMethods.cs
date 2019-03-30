using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class TrialMethods
    {
        public static List<int> IdentifyCluster(int index, Cluster[] clusters)
        {
            List<int> someClusters = new List<int>();

            for (int i = 0; i < clusters.Length; i++)
            {
                if( clusters[i].IsExisted(index)) { someClusters.Add(i); }
                }
            return someClusters.Count > 0? someClusters : 
            throw new Exception($"город не нвйден в кластерах .индекс ={ index}");
        }
        public static bool IsSomethingCommon( List<int> cities1, List<int> cities2)
        {
            for( int i = 0; i < cities1.Count; i++)
            {
                if( cities2.IndexOf(cities1[i]) > 0) { return true; }
            }
            return false;
        }
       
        public static void FindBest(double[][] pheremones, double[][] dist, Cluster[] cl)
        {
            //
            // Choose first city
            double maxPheromone = 0;
            int indexMaxLine = -1;
            int indexMaxColumn = -1;
            for (int i = 0; i < cl[i].Length; i++)
            {
                for (int j = 0; j < pheremones.GetLength(0); j++)
                {
                    if (!IsSomethingCommon(IdentifyCluster(i,cl), IdentifyCluster(j,cl)))
                        if (maxPheromone < pheremones[i][j]) { maxPheromone = pheremones[i][j]; indexMaxLine = i; indexMaxColumn = j; }
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
