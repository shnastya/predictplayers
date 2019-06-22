using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace PredictPlayers
{
    class DataStore
    {
        static string filePath = "Save\\Test\\test";
        static string format = ".xml";

        public static string[] features = new string[] {
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

        public static void Serialize(int numberTest, StoredResult obj)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(StoredResult));
            string fileName = string.Format("{0}{1}{2}", filePath, numberTest, format);
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);
            }
        }

        public static StoredResult Deserialize(int numberTest)
        {
            StoredResult result;
            XmlSerializer formatter = new XmlSerializer(typeof(StoredResult));
            string fileName = string.Format("{0}{1}{2}", filePath, numberTest, format);
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                result = (StoredResult)formatter.Deserialize(fs);
            }

            return result;
        }

        public struct Cluster
        {
            public List<double[]> users;
            public int nmb;
        }

        public static List<Cluster> ModelClusters(string fileName, int countClusters)
        {
            List<Cluster> result = new List<Cluster>();

            int nmb = 0;
            using(StreamReader sr = new StreamReader(fileName))
            {
                var line = sr.ReadLine().Split(' ');
                nmb = Convert.ToInt32(line[14]);

                for (int j = 0; j < countClusters; j++)
                {
                    Cluster cluster = new Cluster();
                    cluster.users = new List<double[]>();
                    cluster.nmb = j;

                    while (nmb == j)
                    {
                        double[] values = new double[14];
                        for (int i = 0; i < 14; i++)
                            values[i] = Convert.ToDouble(line[i].Replace('.',','));
                        cluster.users.Add(values);
                        if (!sr.EndOfStream)
                        {
                            line = sr.ReadLine().Split(' ');
                            nmb = Convert.ToInt32(line[14]);
                        }
                        else nmb = -1;
                    }

                    result.Add(cluster);
                }
            }
            return result;
        }

        public static double[,] MatrixAverageCluster_n(int countClusters, int nmb)
        {
            double[,] matrix = new double[countClusters, 14];
            string fileName = "Save\\Clusters\\model" + nmb + "_n.txt";
            using (StreamReader sr = new StreamReader(fileName))
            {
                var arr = sr.ReadLine().Split(' ');
                for (int i = 0; i < countClusters; i++)
                {
                    arr = sr.ReadLine().Split(' ');
                    for (int j = 0; j < 14; j++)
                        matrix[i, j] = Convert.ToDouble(arr[j].Replace('.', ','));
                }
            }

            return matrix;
        }

        public static double[,] MatrixAverageCluster(int countClusters, int nmb)
        {
            double[,] matrix = new double[countClusters, 14];
            string fileName = "Save\\Clusters\\model" + nmb + ".txt";
            using(StreamReader sr = new StreamReader(fileName))
            {
                var arr = sr.ReadLine().Split(' ');
                for(int i = 0; i < countClusters; i++)
                {
                    arr = sr.ReadLine().Split(' ');
                    for (int j = 0; j < 14; j++)
                        matrix[i, j] = Convert.ToDouble(arr[j].Replace('.',','));
                }
            }

            return matrix;
        }

        public static double Distance(double[] user1, double[] user2)
        {
            double dist = 0;

            for (int i = 0; i < 14; i++)
                dist += (user1[i] - user2[i]) * (user1[i] - user2[i]);
            dist = Math.Sqrt(dist);

            return dist;
        }

        public static double[,] MatrixDist(int count, Cluster cluster)
        {
            double[,] dist = new double[count, count];
            for(int i = 0; i < count; i++)
                for(int j = 0; j <= i; j++)
                {
                    if (j == i)
                        dist[i, j] = 0;
                    else
                        dist[i, j] = dist[j, i] = Math.Abs(1 - Math.Round(Distance(cluster.users[i], cluster.users[j]),2));

                }

            return dist;
        }

        public static string[,] MatrixTooltip(int count, Cluster cluster)
        {
            double[] limit = new double[14] {0.01, 0.01, 0, 0, 0, 0, 0, 0, 0.001, 0.0005, 0.0005, 0.00001, 0.03, 0.02};
            string[,] tooltip = new string[count, count];
            string temp;
            for (int i = 0; i < count; i++)
                for (int j = 0; j <= i; j++)
                {
                    if (j == i)
                        tooltip[i, j] = "";
                    else
                    {
                        temp = "";
                        for(int k = 0; k < 14; k++)
                        {
                            if (Math.Abs(cluster.users[i][k] - cluster.users[j][k]) <= limit[k])
                            {
                                if (k >= 3 && k <= 7)
                                {
                                    if (k == 3 && cluster.users[i][k] == 1)
                                        temp += "Есть герои разных рас" + ", ";
                                    else if (k == 3 && cluster.users[i][k] == 0)
                                        temp += "Не созданы герои разных рас" + ", ";
                                    else if (k == 4 && cluster.users[i][k] == 1)
                                        temp += "Есть герои одинаковых рас" + ", ";
                                    else if (k == 4 && cluster.users[i][k] == 0)
                                        temp += "Нет героев одинаковых рас" + ", ";
                                    else if (k == 6 && cluster.users[i][k] == 1)
                                        temp += "Продавал что-либо в магазине" + ", ";
                                    else if (k == 6 && cluster.users[i][k] == 0)
                                        temp += "Не продавал ничего в магазине" + ", ";
                                    else if (k == 7 && cluster.users[i][k] == 1)
                                        temp += "Покупал что-либо в магазине" + ", ";
                                    else if (k == 7 && cluster.users[i][k] == 0)
                                        temp += "Не покупал ничего в магазине" + ", ";
                                    else if (k == 5 && cluster.users[i][k] == 0)
                                        temp += "Не выходил на дуэль" + ", ";
                                    else if (k == 5 && cluster.users[i][k] == 1)
                                        temp += "Выходил на дуэль" + ", ";

                                }
                                else
                                    temp += features[k] + ", ";
                            } 
                        }

                        temp = temp.Remove(temp.Length - 2);
                        tooltip[i, j] = tooltip[j, i] = temp;
                        temp = "";

                    }
                }

            return tooltip;
        }

        public static void SerializeJSON2(int nmbTest, int countClusters)
        {
            StoredResult stored = Deserialize(nmbTest);
            string path = "Json2\\Graph";
            int id = 1;
            double[,] matrix = MatrixAverageCluster(countClusters, nmbTest);
            double[,] matrix_n = MatrixAverageCluster_n(countClusters, nmbTest);
            string[] names = new string[countClusters];
            for (int i = 0; i < countClusters; i++)
                names[i] = "Кластер " + (i + 1).ToString();
            Graph2 g = new Graph2();
            g.nodes = new Node2[countClusters + 14];
            g.edges = new Edge1[countClusters*14*2];
            for(int i = 0; i < countClusters; i++)
            {
                g.nodes[i] = new Node2();
                g.nodes[i].id = id;
                g.nodes[i].label = names[i];
                if(stored.clusters[i].metka == 1)
                  g.nodes[i].class_ = 3;
                else
                  g.nodes[i].class_ = 4;
                id++;
            }
            for(int i = 0; i < 14; i++)
            {
                g.nodes[i+countClusters] = new Node2();
                g.nodes[i+countClusters].id = id;
                g.nodes[i+countClusters].label = features[i];
                g.nodes[i + countClusters].class_ = 1;
                id++;
            }
            int ind = 0;
            for (int i = 0; i < countClusters; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    g.edges[ind] = new Edge1();
                    g.edges[ind].source = i + 1;
                    g.edges[ind].target = countClusters + j + 1;
                    g.edges[ind].weight = matrix_n[i, j];
                    g.edges[ind].tooltip = matrix[i, j].ToString();
                    ind++;
                }
            }
            for (int i = 0; i < 14; i++)
                for (int j = 0; j < countClusters; j++)
                {
                    g.edges[ind] = new Edge1();
                    g.edges[ind].source = countClusters + i + 1;
                    g.edges[ind].target = j + 1;
                    g.edges[ind].weight = matrix_n[j, i];
                    g.edges[ind].tooltip = matrix[j, i].ToString();
                    ind++;
                }

            string ser = JsonConvert.SerializeObject(g);
            using (StreamWriter sw = new StreamWriter("Json2\\Graph.js"))
            {
                sw.Write(ser);
            }

        }

            public static void SerializeJSON(string fileName, int numberTest)
        {
            int countClusters = 0;
            List<Cluster> listClusters = new List<Cluster>();

            StoredResult testResult = Deserialize(numberTest);
            countClusters = testResult.countClusters;
            listClusters = ModelClusters(fileName, countClusters);
            int[] clusters = new int[countClusters];
            for(int i = 0; i < testResult.clusters.Count; i++)
            {
                clusters[i] = testResult.clusters[i].leaveCount + testResult.clusters[i].stayCount;
            }

            for (int i = 0; i < countClusters; i++)
            {
                Graph1 g = new Graph1();
                g.nodes = new Node1[clusters[i]];
                int id = 1;
                for(int j = 0; j < clusters[i]; j++)
                {
                    g.nodes[j] = new Node1();
                    g.nodes[j].id = id;
                    g.nodes[j].label = "Игрок " + id;
                    id++;
                }

                double[,] matrix = MatrixDist(clusters[i], listClusters[i]);
                if(i+1 == 13)
                {

                }
                string[,] tooltip = MatrixTooltip(clusters[i], listClusters[i]);
                g.edges = new Edge1[clusters[i]*(clusters[i]-1)];
                int ind = 0;
                for(int j = 0; j < clusters[i]; j++)
                {
                    for (int k = 0; k < clusters[i]; k++) {
                        if (j != k)
                        {
                            g.edges[ind] = new Edge1();
                            g.edges[ind].source = j + 1;
                            g.edges[ind].target = k + 1;
                            g.edges[ind].weight = matrix[j, k];
                            g.edges[ind].tooltip = tooltip[j, k];
                            ind++;
                        }
                    }
                }

                string ser = JsonConvert.SerializeObject(g);
                using (StreamWriter sw = new StreamWriter("Json\\Graph"+ (i+1).ToString() + ".js"))
                {
                    sw.Write(ser);
                }
            }

            /*Создание файла для второй диаграммы*/
            SerializeJSON2(numberTest, countClusters);

            
        }
    }
}
