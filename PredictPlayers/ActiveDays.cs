using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class ActiveDays
    {
        public int Count;
        public int CountInactiveGaps { get; private set; }
        public double InactiveDays { get; private set; }
        public int clusterCount;
        public int clusterInactiveGap;

        public ActiveDays()
        {
            Count = CountInactiveGaps = 0;
            InactiveDays = 0;
        }

        public void Add()
        {
            Count++;
        }

        public void AddInactiveGap(double days)
        {
            CountInactiveGaps++;
            InactiveDays += days;
        }

        public double AverageInactiveGap()
        {
            if (CountInactiveGaps == 0) return 0;
            return InactiveDays / CountInactiveGaps;
        }


    }
}
