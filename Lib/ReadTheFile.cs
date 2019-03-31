using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Lib
{
  public  class ReadTheFile
    {
        public static void Read(out double[][] dist, out int n ,out int m, out Cluster[] cl)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\computer\Desktop\Курсовая\tests\3burma14.txt", Encoding.UTF8);

            
            if(!int.TryParse(lines[0].Split(' ')[1], out n)||(n <= 0)) { throw new ArgumentException($"Файл не корректен!\n Строка 1 неверна");  }
            if(!int.TryParse(lines[1].Split(' ')[1], out m)||(m <= 0)) { throw new ArgumentException("Файл не корректен!\n Строка 2 неверна");  }

            Cluster[] cluster = new Cluster[m];// Создаем кластер с m 

            for( int i =0; i < m; i++)
            {
                string[] tempLine = lines[4 + i].Split(' ');
                int[] tempSequenceOfCities = new int[tempLine.Length-1];
                for( int j =0; j < tempLine.Length-1; j++)
                {
                   if(!int.TryParse(tempLine[j], out tempSequenceOfCities[j])|| tempSequenceOfCities[j] < 0) { throw new ArgumentException(); }
                    tempSequenceOfCities[j] -= 1;// Читает непонятную стрку(пустую) после всех чисел
                }
                cluster[i] = new Cluster(tempSequenceOfCities); // Добавлена текущая строка городов в кластере
            }

            double[][] distance = new double[n][];
            for( int i =0; i < n; i++)
            {
                string[] tempLine = lines[4 + m + i].Split(' ');
                distance[i] = new double[tempLine.Length-1];
                for( int j =0; j < tempLine.Length-1; j++)
                {
                    if( !double.TryParse(tempLine[j], out distance[i][j])|| distance[i][j] <=0) { throw new ArgumentException(); }
                }
            }
            cl = cluster;
            dist = distance;
        }
    }
}
