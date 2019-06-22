using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PredictPlayers
{
    public class Prognostication
    {
        public string[] features = new string[] {
            "Сумма внесенных средств",
            "Сумма потраченных средств",
            "День регистрации",
            "Созданы герои разных рас",
            "Созданы герои одинаковой расы",
            "Выходил на дуэль",
            "Продавал что-либо в магазине",
            "Покупал что-либо в магазине",
            "Среднее время битвы (мс)",
            "Частота проигрыша",
            "Среднее кол-во квестов в день",
            "Среднее время, затраченное на квест",
            "Количество активных дней",
            "Количество промежутков неактивностей"};

        public string[] answer = new string[]
        {
            "На основании результатов кластеризации этот игрок останется в игре с вероятностью ",
            "На основании результатов кластеризации этот игрок покинет игру с вероятностью "
        };

        string clustersPath = "Save\\Clusters\\model";
        List<double[]> model;
        int countFeatures = 14;
        int countClusters;
        double[] user;
        int numberTest;
        StoredResult storedResult;

        public Prognostication()
        {
            model = new List<double[]>();
        }

        public StoredResult NewModel(int numberTest)
        {
            this.numberTest = numberTest;
            string fileName = clustersPath + numberTest + ".txt";
            StreamReader sr = new StreamReader(fileName);
            countClusters = Convert.ToInt32(sr.ReadLine());
            for(int i = 0; i < countClusters; i++)
            {
                double[] f = new double[countFeatures];
                var arr = sr.ReadLine().Split(' ');
                for (int j = 0; j < countFeatures; j++)
                    f[j] = Convert.ToDouble(arr[j].Replace('.',','));
                model.Add(f);
            }

            storedResult = DataStore.Deserialize(numberTest);
            return storedResult;
        }

        public string Predict(double [] newUser)
        {
            user = newUser;
            double minDist = Double.MaxValue;
            double dist, summ;
            int numbCluster = -1;
            for(int i = 0; i < countClusters; i++)
            {
                summ = 0;
                for (int j = 0; j < countFeatures; j++)
                    summ += (model[i][j] - user[j]) * (model[i][j] - user[j]);
                summ = Math.Sqrt(summ);
                if (summ < minDist)
                {
                    minDist = summ;
                    numbCluster = i;
                }
            }

            int metka = storedResult.clusters[numbCluster].metka;
            int l0 = storedResult.clusters[numbCluster].stayCount;
            int l1 = storedResult.clusters[numbCluster].leaveCount;
            double P = (metka == 0) ? Convert.ToDouble(l0) / Convert.ToDouble(l1 + l0) : Convert.ToDouble(l1) / Convert.ToDouble(l1 + l0);
            P *= 100;
            P = Math.Round(P, 2);

            return (answer[storedResult.clusters[numbCluster].metka] + P.ToString() + "%");
        }
    }
}
