using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Trial
    {
        //     double trialLength;//  Текущая длина маршрута 

        //  public static double[][] pheremones, dist;
        int[] clusters;

        List<int> box = new List<int>(); // Чтобы бы не добавлять в маршрут лишние города

        int[] cities; // пройденные города

        int currentCity; // Текущий город

        int currentAmount = 0; // Текущее количество городов

        /// <summary>
        ///  Конструктор иниц-щий первый город 
        /// </summary>
        /// <param name="targetCity">targetCity == currentCity</param>
        /// <param name="totalCities">количество городов всего в маршруте</param>
        public Trial(int targetCity, int[] clusters)
        {
            int totalCities = clusters.Length;
            this.clusters = new int[clusters.Length];
            clusters.CopyTo(this.clusters, 0);
            cities = new int[totalCities];
            for (int i = 0; i < totalCities; i++)
            {
                cities[i] = -1; // Обозначение пустого города
            }
            AddCity(targetCity);

        }

        public bool IsTrialCompleted()
        {
            // Если маршрут составлен полностью возвращает TRUE
            return cities.Last() != -1 ? true : false;
        }

        public double GetTrialLength(double[][] dist)
        {
            double totalDistance = 0;
            if (IsTrialCompleted())
            {
                for (int i = 0; i < cities.Length - 1; i++)
                {
                    totalDistance += dist[cities[i]][cities[i + 1]]; // Суммирует дистанцию между i и i+1 городом в списке 
                }
                totalDistance += dist[cities[cities.Length - 1]][cities[0]]; // Добавляет расстояние между первым и последним городом
            }
            // Прописать исключения
            return totalDistance;
        } // Длина всего маршрута

        /*******************/
        public static int IdentifyCluster(int index, int[] clusters)
        {
            int border = 0;
            for (int i = 0; i < clusters.Length; i++)
            {
                border += clusters[i];
                if (index < border) { return i; }
            }
            throw new Exception($"город не нвйден в кластерах .индекс ={ index}");
        }

        /*******************/
        public void AddAllClusterToBox(int targetCity)
        {
            int indexCluster = IdentifyCluster(targetCity, this.clusters);// индекс
            int indexCity = 0;
            for (int i = 0; i < indexCluster; i++)
            {
                indexCity += clusters[i];
            }
            for (int i = 0; i < clusters[indexCluster]; i++)
            {
                box.Add(i + indexCity);
            }
        }

        public void AddCity(int targetCity)
        {
            if (IsAllowedCity(targetCity))
            {
                currentCity = targetCity;
                currentAmount++;
                cities[currentAmount - 1] = targetCity;
                AddAllClusterToBox(targetCity);// Добавляем все города-соседи города-таргета
            }
            else
            {
                throw new Exception("город был добавлен");
            }
        } // Добавление города в маршрут

        public bool IsAllowedCity(int targetCity)
        {
            /*
            for (int i = 0; i < cities.Length; i++)
            {
                if (targetCity == this[i]) { return false; } // Если данный город уже пройден, то он запрещен
            }
            return true;
            */
            return box.IndexOf(targetCity) < 0 ? true : false;
        } // Проверка что город не добавлен в маршрут


        // Массив вероятностей в каждый город
        static double[] CreateTrialProbability(Trial obj, double[][] pheromones, double[][] dist, double alpha, double beta)
        {
            int cityNumber = pheromones.GetLength(0);

            double probabilityOfAll = 0;

            for (int i = 0; i < cityNumber; i++)
            {
                if (obj.IsAllowedCity(i))
                {
                    double distanceInfluence = Math.Pow(1.0 / dist[obj.currentCity][i], alpha); // влияние расстояния
                    double pheromonesInfluence = Math.Pow(pheromones[obj.currentCity][i], beta);

                    probabilityOfAll += distanceInfluence * pheromonesInfluence;
                }
            }
            double[] probabilities = new double[cityNumber]; // Изначально у нас ПОЛНЫЙ граф

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (obj.IsAllowedCity(i))
                {
                    double distanceInfluence = Math.Pow(1.0 / dist[obj.currentCity][i], alpha); // влияние расстояния
                    double pheromonesInfluence = Math.Pow(pheromones[obj.currentCity][i], beta); // влияние расстояния
                    probabilities[i] = (distanceInfluence * pheromonesInfluence) / probabilityOfAll;
                }
                else
                {
                    probabilities[i] = 0; // Отрицательные вероятности не будут учитываться
                }
            } // Массив всех вероятностей

            return probabilities;
        }

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

        /// <summary>
        /// Изменяет феромоновый слой, по выбранному маршруту, явл-ся наилучшим (уплотнение слоя)
        /// </summary>
        /// <param name="pheromones"> феромоновый слой</param>
        /// <param name="dist"> расстояния от городов ( для поиска вероятностей)</param>
        /// <param name="alpha">( коэфицент жадности поиска)</param>
        /// <param name="beta">( коэфицент важности феромонового слоя)</param>
        /// <param name="firstCity">( первый город маршрута)</param>
        /// <param name="forgetConst">( коэфицент забывания старых феромонов (лежит в промежутке -- [0,1] )</param>
        /// <param name="distanceImportance">( нормирующий коэфицент нового слоя
        /// ( чтобы новый слой был сопоставим старому по значению)</param>
        public static void ChangePheromones(double[][] pheromones, double[][] dist, int firstCity,
            double forgetRate, double distanceImportance, double alpha, double beta, int[] clusters)
        {
            Console.WriteLine($"first city = {firstCity}");//
            Trial obj = new Trial(firstCity, clusters);
            // Пока маршрут не закончен, добавляю наилучшие города
            while (!obj.IsTrialCompleted())
            {
                int newCity = GetChosenCity(CreateTrialProbability(obj, pheromones, dist, alpha, beta)); // Новый наилучший город
                Console.WriteLine($"next city = {newCity}");
                obj.AddCity(newCity);
            }
            // Маршрут создан
            // Теперь по нему меняем вес феромонов на всех ребрах, построенного маршрута
            for (int i = 0; i < clusters.Length - 1; i++)
            {
                if (obj.GetTrialLength(dist) < 100)// Надо исключить деревья ( нет пути из конца в начало)
                {
                    double newPheromone = distanceImportance / (obj.GetTrialLength(dist)); // Новый феромоновый слой
                    Console.WriteLine($"new layer = {newPheromone} , trial length = {obj.GetTrialLength(dist)}");
                    // Суммирование нового слоя и старого с учетом забывания феромонов
                    double deltaPheromone = pheromones[obj.cities[i]][obj.cities[i]] * forgetRate + newPheromone * (1 - forgetRate);

                    pheromones[obj.cities[i]][obj.cities[i + 1]] = deltaPheromone;
                }
            }
        }

        public static void FindBest( double[][] pheremones, double[][] dist, int[] clusters)
        {
            //
            // Choose first city
            double maxPheromone = 0;
            int indexMaxLine = -1;
            int indexMaxColumn = -1;
            for( int i =0; i < clusters[i]; i++)
            {
                for( int j =0; j < pheremones.GetLength(0); j++)
                {
                    if(IdentifyCluster(j, clusters) != IdentifyCluster(i, clusters) )
                    if( maxPheromone < pheremones[i][j]) { maxPheromone = pheremones[i][j]; indexMaxLine = i; indexMaxColumn = j;}
                }
            }
            // Нашли максимальный след из первого кластера в другой
            Trial obj = new Trial(indexMaxLine, clusters);
            obj.AddCity(indexMaxColumn);

            while(!obj.IsTrialCompleted())
            {
                int indexMax = -1;
                double max = 0;
                for( int i = 0; i < pheremones.GetLength(0); i++)
                {
                    if( obj.IsAllowedCity(i))
                    if( max < pheremones[obj.currentCity][i]) { max = pheremones[obj.currentCity][i]; indexMax = i; }
                }
                obj.AddCity(indexMax);
            }
            Console.WriteLine("The Best trial");
            obj.ShowTrial();
            Console.Write($"The length:{obj.GetTrialLength(dist)} ");
            Console.WriteLine();

        }

        public void ShowTrial()
        {
            Console.Write($"The trial:");
            for (int i = 0; i < cities.Length - 1; i++)
            {
                Console.Write($"{cities[i] + 1} ->");
            }
            Console.WriteLine($"{cities[cities.Length - 1] + 1}");
        }

    }
}
