using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class Cluster
    {
        public int[] activeDays = new int[2];
        public double[] payment = new double[2];
        public double[] averageTimeBattle = new double[2];
        public double[] freqLosses = new double[2];
        public double[] averageTimeQuests = new double[2];
        public double[] averageCountQuests = new double[2];
        public double[] averageInactiveDays = new double[2];

        public Cluster(string[] arr)
        {
            activeDays[0] = (arr[0] == "-") ? -1 : Convert.ToInt32(arr[0]);
            activeDays[1] = (arr[1] == "-") ? -1 : Convert.ToInt32(arr[1]);
            payment[0] = (arr[2] == "-") ? -1 : Convert.ToDouble(arr[2]);
            payment[1] = (arr[3] == "-") ? -1 : Convert.ToDouble(arr[3]);
            averageTimeBattle[0] = (arr[4] == "-") ? -1 : Convert.ToDouble(arr[4]);
            averageTimeBattle[1] = (arr[5] == "-") ? -1 : Convert.ToDouble(arr[5]);
            freqLosses[0] = (arr[6] == "-") ? -1 : Convert.ToDouble(arr[6]);
            freqLosses[1] = (arr[7] == "-") ? -1 : Convert.ToDouble(arr[7]);
            averageTimeQuests[0] = (arr[8] == "-") ? -1 : Convert.ToDouble(arr[8]);
            averageTimeQuests[1] = (arr[9] == "-") ? -1 : Convert.ToDouble(arr[9]);
            averageCountQuests[0] = (arr[10] == "-") ? -1 : Convert.ToDouble(arr[10]);
            averageCountQuests[1] = (arr[11] == "-") ? -1 : Convert.ToDouble(arr[11]);
            averageInactiveDays[0] = (arr[12] == "-") ? -1 : Convert.ToDouble(arr[12]);
            averageInactiveDays[1] = (arr[13] == "-") ? -1 : Convert.ToDouble(arr[13]);
        }

    }
}
