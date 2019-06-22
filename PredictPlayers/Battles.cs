using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class Battles
    {
        int sumTime;
        int count;
        int countWin;
        int countLoss;
        public int clusterTime;
        public int clusterLosses;

        public Battles()
        {
            sumTime = count = countWin = countLoss = 0;
        }

        public void Add(int time, string status)
        {
            sumTime += time;
            count++;
            if (status == "win")
                countWin++;
            else countLoss++;
        }

        public double GetAverageTime()
        {
            if (count == 0) return 0;
            return (double)sumTime / count;
        }

        public double GetFreqLosses()
        {
            if (countWin == 0) return 0;
            return (double)countLoss / countWin;
        }
    }
}
