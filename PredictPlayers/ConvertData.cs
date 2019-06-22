using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class ConvertData
    {
        List<Cluster> clusters;

        public ConvertData(List<Cluster> clusters)
        {
            this.clusters = clusters;
        }

        public void Convert(User user)
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                if (i == 0)
                {
                    if (user.activeDays.Count >= clusters[0].activeDays[0] && user.activeDays.Count <= clusters[0].activeDays[1])
                        user.activeDays.clusterCount = i;
                    if (clusters[i].payment[0] != -1 && clusters[i].payment[1] != -1 && user.payment >= clusters[0].payment[0] && user.payment <= clusters[0].payment[1])
                        user.payCluster = user.spendCluster = i;
                    if (user.battles.GetAverageTime() >= clusters[0].averageTimeBattle[0] && user.battles.GetAverageTime() <= clusters[0].averageTimeBattle[1])
                        user.battles.clusterTime = i;
                    if (user.battles.GetFreqLosses() >= clusters[0].freqLosses[0] && user.battles.GetAverageTime() <= clusters[0].freqLosses[1])
                        user.battles.clusterLosses = i;
                    if (user.quests.GetAverageTimeQuests() >= clusters[0].averageTimeQuests[0] && user.quests.GetAverageTimeQuests() <= clusters[0].averageTimeQuests[1])
                        user.quests.clusterTime = i;
                    if (user.quests.GetAverageCountQuests() >= clusters[0].averageCountQuests[0] && user.quests.GetAverageCountQuests() <= clusters[0].averageCountQuests[1])
                        user.quests.clusterCount = i;
                    if (user.activeDays.InactiveDays >= clusters[0].averageInactiveDays[0] && user.activeDays.InactiveDays <= clusters[0].averageInactiveDays[1])
                        user.activeDays.clusterInactiveGap = i;
                }
                else
                {
                    if (clusters[i].activeDays[0] != -1 && clusters[i].activeDays[1] != -1 && user.activeDays.Count > clusters[i].activeDays[0] && user.activeDays.Count <= clusters[i].activeDays[1])
                        user.activeDays.clusterCount = i;
                    else if (clusters[i].activeDays[0] != -1 && user.activeDays.Count > clusters[i].activeDays[0])
                        user.activeDays.clusterCount = i;
                    if (clusters[i].payment[0] != -1 && clusters[i].payment[1] != -1 && user.payment > clusters[i].payment[0] && user.payment <= clusters[i].payment[1])
                        user.payCluster = user.spendCluster = i;
                    else if (clusters[i].payment[0] != -1 && user.payment > clusters[i].payment[0])
                        user.payCluster = user.spendCluster = i;
                    if (clusters[i].averageTimeBattle[0] != -1 && clusters[i].averageTimeBattle[1] != -1 && user.battles.GetAverageTime() > clusters[i].averageTimeBattle[0] && user.battles.GetAverageTime() <= clusters[i].averageTimeBattle[1])
                        user.battles.clusterTime = i;
                    else if (clusters[i].averageTimeBattle[0] != -1 && user.battles.GetAverageTime() > clusters[i].averageTimeBattle[0])
                        user.battles.clusterTime = i;
                    if (clusters[i].freqLosses[0] != -1 && clusters[i].freqLosses[1] != -1 && user.battles.GetFreqLosses() > clusters[i].freqLosses[0] && user.battles.GetAverageTime() <= clusters[i].freqLosses[1])
                        user.battles.clusterLosses = i;
                    else if (clusters[i].freqLosses[0] != -1 && user.battles.GetFreqLosses() > clusters[i].freqLosses[0])
                        user.battles.clusterLosses = i;
                    if (clusters[i].averageTimeQuests[0] != -1 && clusters[i].averageTimeQuests[1] != -1 && user.quests.GetAverageTimeQuests() > clusters[i].averageTimeQuests[0] && user.quests.GetAverageTimeQuests() <= clusters[i].averageTimeQuests[1])
                        user.quests.clusterTime = i;
                    else if (clusters[i].averageTimeQuests[0] != -1 && user.quests.GetAverageTimeQuests() > clusters[i].averageTimeQuests[0])
                        user.quests.clusterTime = i;
                    if (clusters[i].averageCountQuests[0] != -1 && clusters[i].averageCountQuests[1] != -1 && user.quests.GetAverageCountQuests() > clusters[i].averageCountQuests[0] && user.quests.GetAverageCountQuests() <= clusters[i].averageCountQuests[1])
                        user.quests.clusterCount = i;
                    else if (clusters[i].averageCountQuests[0] != -1 && user.quests.GetAverageCountQuests() > clusters[i].averageCountQuests[0])
                        user.quests.clusterCount = i;
                    if (clusters[i].averageInactiveDays[0] != -1 && clusters[i].averageInactiveDays[1] != -1 && user.activeDays.InactiveDays > clusters[i].averageInactiveDays[0] && user.activeDays.InactiveDays <= clusters[i].averageInactiveDays[1])
                        user.activeDays.clusterInactiveGap = i;
                    else if (clusters[i].averageInactiveDays[0] != -1 && user.activeDays.InactiveDays > clusters[i].averageInactiveDays[0])
                        user.activeDays.clusterInactiveGap = i;
                }
            }

        }
    }
}
