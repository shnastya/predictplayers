using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class Quests
    {
        public int countDays;
        public int countQuests;
        public double minutes;
        public int clusterTime;
        public int clusterCount;

        public Quests()
        {
            countDays = countQuests = 0;
        }

        public void Add(int day)
        {
            countDays += day;
            countQuests++;
        }

        public double GetAverageCountQuests()
        {
            if (countDays == 0) return 0;
            return (double)countQuests / countDays;
        }

        public double GetAverageTimeQuests()
        {
            if (countQuests == 0 || countQuests == 1) return 0;
            return minutes / (countQuests - 1);
        }
    }
}
