using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    public struct cluster
    {
        public int number;
        public int leaveCount;
        public int stayCount;
        public int metka;
        public double errors;
    }

    [Serializable]
    public class StoredResult
    {
        public double silhouette;
        public double Bmetrics;
        public double Qmetrics;
        public List<cluster> clusters;
        public string algorithm;
        public int countClusters;
        public string metrcis;
        public string link;
        public string init;
        public string alg;
        public bool normalization;
        public int numberTest;
        public double[] stayedPlayers;
        public double[] leavePlayers;
        public double damping;
        public int maxIter;
        public int convergIter;
        public double eps;
        public int minSample;

        public StoredResult(int n)
        {
            stayedPlayers = new double[3];
            leavePlayers = new double[3];
            countClusters = n;
            clusters = new List<cluster>();
        }

        public StoredResult()
        {

        }

        public void AddCluster(int numb, int leaveCount, int stayCount, int metka, double errors)
        {
            cluster cl = new cluster();
            cl.number = numb;
            cl.leaveCount = leaveCount;
            cl.stayCount = stayCount;
            cl.metka = metka;
            cl.errors = errors;
            clusters.Add(cl);
        }
    }
}
