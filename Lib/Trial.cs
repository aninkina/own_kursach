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
     
        int[] cities; // пройденные города

        int currentCity; // Текущий город

        int currentAmount = 0; // Текущее количество городов

        /// <summary>
        ///  Конструктор иниц-щий первый город 
        /// </summary>
        /// <param name="targetCity">targetCity == currentCity</param>
        /// <param name="totalCities">количество городов всего в маршруте</param>
        public Trial(int targetCity, int totalCities, int[] clusters)
        {
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

        // CHECK EXCEPTION : может возвращать НЕ пройденные города
        public int this[int indexer] { get => cities[indexer]; } // Обращение к пройденным городам по индексу экземпляра

        public double GetTrialLength(double[][] dist)
        {
            double totalDistance = 0;
            if (IsTrialCompleted())
            {
                for (int i = 0; i < cities.Length - 1; i++)
                {
                    totalDistance += dist[cities[i]][cities[i + 1]]; // Суммирует дистанцию между i и i+1 городом в списке 
                }
                totalDistance += dist[cities[0]][cities[cities.Length - 1]]; // Добавляет расстояние между первым и последним городом
            }
            // Прописать исключения
            return totalDistance;
        } // Длина всего маршрута

        public void AddCity(int targetCity)
        {
            if (IsAllowedCity(targetCity))
            {
                currentCity = targetCity;
                currentAmount++;
                cities[currentAmount - 1] = targetCity;

            }
        } // Добавление города в маршрут

        public bool IsAllowedCity(int targetCity)
        {
            for (int i = 0; i < cities.Length; i++)
            {
                if (targetCity == this[i]) { return false; } // Если данный город уже пройден, то он запрещен
            }
            return true;
        } // Проверка что город не добавлен в маршрут

        // Массив вероятностей в каждый город
        static double[] CreateTrialProbability(Trial obj, double[][] pheromones, double[][] dist,double alpha, double beta)
        {
            int cityNumber = dist.GetLength(0);

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
                    Console.WriteLine($" p[{i}] = {probabilities[i]}");
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
            Console.WriteLine( $"rand = {rand}");
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
            int cityNumber = dist.GetLength(0);
            Trial obj = new Trial(firstCity, cityNumber, clusters);
            // Пока маршрут не закончен, добавляю наилучшие города
            while (!obj.IsTrialCompleted())
            {

                int newCity = GetChosenCity(CreateTrialProbability(obj, pheromones, dist,alpha,beta)); // Новый наилучший город
                Console.WriteLine($"next city = {newCity}");
                obj.AddCity(newCity);
            }
            // Маршрут создан
            // Теперь по нему меняем вес феромонов на всех ребрах, построенного маршрута
            for (int i = 0; i < cityNumber - 1; i++)
            {
                double L = 10; //  старый слой на порядок больше нового
                double newPheromone = distanceImportance / (obj.GetTrialLength(dist)*L); // Новый феромоновый слой
                // Суммирование нового слоя и старого с учетом забывания феромонов
                double deltaPheromone = pheromones[obj[i]][obj[i + 1]] * forgetRate + newPheromone * (1 - forgetRate);

                pheromones[obj[i]][obj[i + 1]] = deltaPheromone;

            }
        }

        /// <summary>
        /// Жадный алгоритм поиска кратчайшего пути по матрице смежности феромоногово слоя
        /// </summary>
        /// <param name="pheromones"></param>
        /// <param name="targetCity"></param>
        /// <param name="totalCities"></param>
        /// <param name="dist"></param>
        public static void FindTheBestTrial(double[][] pheromones, int targetCity, int totalCities, double[][] dist, int[] cluters)
        {
            // Составляем кратчайшний маршрут с таргет города
            Trial obj = new Trial(targetCity, totalCities, cluters);
            // Пока путь не составлен, добавляй города с ребром наибольшего феромонового слоя ( жадный алгоритм)


            while (!obj.IsTrialCompleted())
            {
                double maxPher = -1;
                int indexMax = -1;
                for (int i = 0; i < totalCities; i++)
                {
                    if (obj.IsAllowedCity(i))
                    {
                        if (maxPher < pheromones[obj.currentCity][i])
                        {
                            maxPher = pheromones[obj.currentCity][i];
                            indexMax = i;
                        }
                    }
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
