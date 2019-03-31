using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Trial
    {
        /// <summary>
        ///  Конструктор иниц-щий первый город 
        /// </summary>
        /// <param name="targetCity">targetCity == currentCity</param>
        /// <param name="totalCities">количество городов всего в маршруте</param>
        public Trial(int targetCity, Cluster[] cl)
        {
            // Подсчитываем кол-во уникальных городов
            List<int> totalList = new List<int>();
            for (int i = 0; i < cl.Length; i++)
            {
                for (int j = 0; j < cl[i].Length; j++)
                    if (totalList.IndexOf(cl[i].Element(j)) < 0)
                    {
                        totalList.Add(cl[i].Element(j));
                    }
            }
            _total = totalList.Count;
            // Создаем массив вида = { -1, -1, -1, -1 ,-1 .... -1}
            this.cl = cl ?? throw new ArgumentNullException(nameof(cl));
            cities = new int[_total];
            for (int i = 0; i < _total; i++)
                cities[i] = -1; // Обозначение пустого города
            // Добавляем первый город
            AddCity(targetCity);
        }

        // Массив кластеров (кластер хранит список городов в нем)
        Cluster[] cl;

        List<int> _banList;

        // Box, пройденных городов (добавляется весь кластер) 
        List<int> box = new List<int>();

        // Маршрут (по порядку)
        int[] cities;

        // Текущий город
        public int currentCity;

        // Текущее количество городов
        int currentAmount = 0;

        // Всего городов
        int _total = 0;

        // Маршрут проложен, когда все города "запрещены"
        public bool IsTrialCompleted() => box.Count == _total ? true : false;

        public bool IsAllowedCity(int targetCity) => box.IndexOf(targetCity) < 0 ? true : false;

        /*******************/
        public double GetTrialLength(double[][] dist)
        {
            double totalDistance = 0;
            // Маршрут имеет вид = { 1, 1, 1, 2}
            if (IsTrialCompleted())
            {
                // currentAmount-1 == кол-во ребер дерева 
                for (int i = 0; i < currentAmount - 1; i++)
                    // + ребро[i,i+1] к весу маршрута
                    totalDistance += dist[cities[i]][cities[i + 1]];
                // + ребро[конец, начало] к весу маршрута
                totalDistance += dist[cities[currentAmount - 1]][cities[0]];
            }
            // Прописать исключения
            return totalDistance;
        }

        public void AddAllClusterToBox(int targetCity)
        {
            // Кластеры  << нового города
            List<int> indexCluster = TrialMethods.IdentifyCluster(targetCity, this.cl);
            // Рандомный кластер, содержащий targetCity надо забанить
            if (indexCluster != null)
            {
                Console.Write($"cluster's index: ");
                indexCluster.ForEach(i => Console.Write($"{i} "));
                Console.WriteLine();

                int rand = indexCluster[RandomNumbers.GetRandom.Next(0, indexCluster.Count)];
                Cluster randCluster = cl[rand];
                Console.WriteLine($"Random cluster № {rand}  has city № {targetCity}");
                // Бежим по кластеру содержащеему targetCity
                // Баним города из этого кластера (если еще не забанены)
                Console.Write("IN BOX:");
                for (int i = 0; i < randCluster.Length; i++)
                {
                    if (box.IndexOf(randCluster.Element(i)) < 0)
                    {
                        Console.Write($"{randCluster.Element(i)} ");
                        box.Add(randCluster.Element(i));
                    }
                }
                Console.WriteLine();
            }
        }

        public void AddToBanList(int clusterIndex) => _banList.Add(clusterIndex);

        public bool IsItInBanList( int targetCity)
        {

        }

        // Если города нет в box'e запрещенных городов, добавляем, запрещаем всех городов-соседей
        public void AddCity(int targetCity)
        {
            if (IsAllowedCity(targetCity))
            {
                AddAllClusterToBox(targetCity);// Добавляем все города-соседи города-таргета
                currentCity = targetCity;
                currentAmount++;
                cities[currentAmount - 1] = targetCity;
            }
            else
            {
                throw new Exception("город был добавлен");
            }
        }

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
        public static void ChangePheromones(double[][] pheromones, double[][] dist,
            double forgetRate, double distanceImportance, double alpha, double beta, Cluster[] cl)
        {
            // Выбираем рандомный первый город
            int firstCity = RandomNumbers.GetRandom.Next(0, dist.GetLength(0));

            Console.WriteLine($"first city = {firstCity}");//
            Trial obj = new Trial(firstCity, cl);
            // Пока маршрут не закончен, добавляю наилучшие города
            while (!obj.IsTrialCompleted())
            {
                // Новый наилучший город
                int newCity = RandomNumbers.GetChosenCity(CreateTrialProbability(obj, pheromones, dist, alpha, beta));
                Console.WriteLine($"next city = {newCity}");

                obj.AddCity(newCity);
            }
            Console.WriteLine();
            // Маршрут создан
            // Теперь по нему меняем вес феромонов на всех ребрах, построенного маршрута
            Console.WriteLine($"total amount = {obj.currentAmount}");
            if (obj.currentAmount == obj._total)
            {
                // Создание слоя
                double l = 1000;
                double newPheromone = distanceImportance * l / (obj.GetTrialLength(dist));
                Console.WriteLine($"new layer = {newPheromone:f3} , trial length = {obj.GetTrialLength(dist)}");

                for (int i = 0; i < obj.currentAmount - 1; i++)
                {
                    // Суммирование нового слоя и старого с учетом забывания феромонов
                    pheromones[obj.cities[i]][obj.cities[i + 1]] = pheromones[obj.cities[i]][obj.cities[i]] * forgetRate + newPheromone * (1 - forgetRate);
                }
            }
        }

        public void ShowTrial()
        {
            Console.Write($"The trial:");
            for (int i = 0; i < cities.Length - 1; i++)
            {
                Console.Write($"{cities[i]} ->");
            }
            Console.WriteLine($"{cities[cities.Length - 1]}");
        }
    }
}
