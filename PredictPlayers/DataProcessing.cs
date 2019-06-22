using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictPlayers
{
    class DataProcessing
    {
        static string path = @"C:\Users\Анастасия\Desktop\4 курс\11 триместр\НИР\Курсовая\Telegram Desktop\users data\";
        static List<User> lst = new List<User>();

        static DayOfWeek WhatDay(string y, string m, string d)
        {
            string date = y + "-" + m + "-" + d;
            DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return dt.DayOfWeek;
        }

        static int GetIndex(string userId)
        {
            return lst.FindIndex((o) => o.id == userId);
        }

        public static void Start(string resultFile, MainForm f)
        {
            string str;
            List<HeroCount> heroes = new List<HeroCount>();

            f.listBoxMsg.Items.Add("Обработка данных началась...");

            using (StreamReader sr = new StreamReader(path + "events_hero.txt"))   //список возможных race
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(',');
                    if (heroes.FindIndex((o) => o.name == arr[5]) == -1)
                        heroes.Add(new HeroCount(arr[5]));
                }
            }

            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_user.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(',');
                    string year = arr[2].Substring(1, 4);
                    string month = arr[2].Substring(6, 2);
                    string day = arr[2].Substring(9, 2);
                    User user = new User(arr[1], WhatDay(year, month, day), heroes);
                    lst.Add(user);
                }
            }
            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_payment.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(',');
                    int ind = GetIndex(arr[2]);
                    if (arr[4] == "payment") lst[ind].payment += Convert.ToDouble(arr[3]);
                    else
                        lst[ind].spending += Math.Abs(Convert.ToDouble(arr[3]));
                }
            }
            f.progressBar.PerformStep();
            using (StreamReader sr = new StreamReader(path + "events_hero1.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    int indUser = GetIndex(arr[3]);
                    int indHero = heroes.FindIndex((o) => o.name == arr[5]);
                    if (indUser != -1 && indHero != -1)
                        lst[indUser].heroes[indHero].count++;

                    if (indUser != -1)
                        if (lst[indUser].heroesId.FindIndex((o) => o == arr[4]) == -1)
                        {
                            lst[indUser].heroesId.Add(arr[4]);
                            lst[indUser].heroMaxLevel.Add(0);
                        }
                }
            }
            f.progressBar.PerformStep();
            foreach (User user in lst)
            {
                user.CountDifferentHeroes();
                user.SameHeroes();
            }

            using (StreamReader sr = new StreamReader(path + "events_battle.txt"))
            {
                str = sr.ReadLine();
                string userId = "";
                int ind = -1;
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(',');
                    if (arr[3] != userId)
                    {
                        userId = arr[3];
                        ind = GetIndex(arr[3]);
                    }
                    if (ind >= 0)
                    {
                        if (arr[17] == "duel")
                            lst[ind].duel = true;
                        lst[ind].battles.Add(Convert.ToInt32(arr[6]), arr[5]);

                    }
                }
            }
            f.progressBar.PerformStep();
            using (StreamReader sr = new StreamReader(path + "events_level1.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    int userId = GetIndex(arr[3]);
                    if (userId != -1)
                    {
                        int heroId = lst[userId].heroesId.FindIndex((o) => o == arr[4]);
                        if (arr[5] != "49") lst[userId].heroMaxLevel[heroId] += Convert.ToInt32(arr[5]);
                        else if (arr[5] == "49") lst[userId].heroMaxLevel[heroId] = 49;
                    }

                }

            }
            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_resource.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(',');
                    int ind = GetIndex(arr[3]);
                    if (arr[5] == "market-sell") lst[ind].marketSell = true;
                    else if (arr[5] == "market-buy") lst[ind].marketBuy = true;
                }
            }
            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_quest_sort_date_id.txt"))
            {
                str = sr.ReadLine();
                string userId = "", date = "";
                int ind = -1;
                DateTime dtBegin, dtEnd, dtLastDay;
                DateTime.TryParse("01.01.2000 00:00:00", out dtBegin);
                DateTime.TryParse("08.11.2018", out dtLastDay);
                string lastDayUser = "";
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    string newDate = arr[2].Substring(0, 10);

                    if (userId != arr[3])
                    {
                        date = "";
                        DateTime.TryParse(arr[2], out dtBegin);
                        //if (ind != -1)
                        //{
                        //    DateTime dtLastDayUser;
                        //    DateTime.TryParse(lastDayUser, out dtLastDayUser);
                        //    var res = (dtLastDay - dtLastDayUser).TotalDays;
                        //    lst[ind].hourLeave = res;
                        //    if (res >= 3) lst[ind].leave = true;
                        //}

                        userId = arr[3];
                        ind = GetIndex(userId);
                    }
                    else
                    {
                        DateTime.TryParse(arr[2], out dtEnd);
                        var res = (dtEnd - dtBegin).TotalMinutes;
                        lst[ind].quests.minutes += res;
                        dtBegin = dtEnd;
                    }

                    if (date != newDate)
                    {
                        lst[ind].quests.Add(1);
                        date = newDate;
                        lastDayUser = date;
                    }
                    else lst[ind].quests.Add(0);
                }
            }
            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_resource_sort_date_id.txt"))
            {//считаются промежутки неактивности
                str = sr.ReadLine();
                string userId = "";
                int ind = -1;
                string oldDate = "", newDate = "";
                DateTime dtLastDay;
                DateTime.TryParse("08.11.2018", out dtLastDay);
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    newDate = arr[2].Substring(0, 10);

                    if (userId != arr[3])
                    {
                        if (ind != -1)
                        {
                            DateTime dtLastDayUser;
                            DateTime.TryParse(oldDate, out dtLastDayUser);
                            var res = (dtLastDay - dtLastDayUser).TotalDays;
                            lst[ind].hourLeave = res;
                            if (res >= 3) lst[ind].leave = true;
                            // if (res <= 2) lst[ind].endOfTheGap = true;
                        }
                        userId = arr[3];
                        ind = GetIndex(userId);
                        oldDate = "";
                    }

                    if (oldDate != newDate)
                    {
                        if (oldDate != "")
                        {
                            DateTime start, end;
                            DateTime.TryParse(oldDate, out start);
                            DateTime.TryParse(newDate, out end);
                            var res = (end - start).TotalDays;
                            if (res > 1)
                                lst[ind].activeDays.AddInactiveGap(res);
                        }

                        lst[ind].activeDays.Add();
                        oldDate = newDate;
                    }
                }

            }
            f.progressBar.PerformStep();

            using (StreamReader sr = new StreamReader(path + "events_level_sort_delta49.txt"))
            {
                str = sr.ReadLine();
                int ind = -1;
                DateTime dtLastDay, dtDayReg;
                DateTime.TryParse("08.11.2018", out dtLastDay);

                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    if (arr[5] != "49")
                        break;
                    else
                    {
                        ind = GetIndex(arr[3]);
                        DateTime.TryParse(arr[2].Substring(0, 10), out dtDayReg);
                        var res = (dtLastDay - dtDayReg).TotalDays;
                        if (res <= 2)
                        {
                            lst[ind].endOfTheGap = true;
                        }
                    }
                }
            }
            f.progressBar.PerformStep();

            //DiskrData();


            PrintResult(resultFile);
            //PrintDiskr(resultFile);
            f.progressBar.PerformStep();

            f.listBoxMsg.Items.Add("Обработка данных завершена.");
        }

        private static void DiskrData()
        {
            string str;
            List<Cluster> clusters = new List<Cluster>();
            using (StreamReader sr = new StreamReader("C:\\Users\\Анастасия\\source\\repos\\PredictPlayers\\PredictPlayers\\bin\\Debug\\Category.txt"))
            {
                str = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    var arr = str.Split(';');
                    Cluster cl = new Cluster(arr);
                    clusters.Add(cl);
                }
            }

            ConvertData convertData = new ConvertData(clusters);
            foreach (User us in lst)
            {
                convertData.Convert(us);
            }
        }

        public static void ReplaceCommaOnPoint(string fileName)
        {
            string str = string.Empty;
            string[] lines = File.ReadAllLines(fileName);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                int i = 0;
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        str = line.Replace(",", ".");
                        writer.Write(str);
                        if(i != lines.Count()-1)
                            writer.WriteLine();
                    }
                    i++;
                }
            }


            //using (StreamReader reader = File.OpenText(fileName))
            //{
            //    str = reader.ReadToEnd();
            //}
            //str = str.Replace(",", ".");

            //using (StreamWriter file = new StreamWriter(fileName))
            //{
            //    file.Write(str);
            //}
        }

        public static void PrintDiskr(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (User us in lst)
                {
                    if (us.heroesId.Count == 1) us.leave = true;
                    sw.WriteLine(us.payCluster + ";" + us.spendCluster + ";" + (int)us.dayOfWeek + ";" +
                        us.countDiffHeroes + ";" + Convert.ToInt32(us.sameHeroes) + ";" + Convert.ToInt32(us.duel) + ";" + Convert.ToInt32(us.marketSell) +
                        ";" + Convert.ToInt32(us.marketBuy) + ";" +
                        us.battles.clusterTime + ";" + us.battles.clusterLosses + ";" +
                        us.quests.clusterCount + ";" + us.quests.clusterTime + ";" + us.activeDays.clusterCount + ";" +
                        us.activeDays.clusterInactiveGap + ";" + Convert.ToInt32(us.leave));
                }
            }
        }

        public static void PrintResult(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(/*path + "RESULT_NOTID_NOTENDOFTHEGAP2.TXT"*/ fileName))
            {
                int sumLvl;
                sw.WriteLine("payment;spending;DayOfWeek;DifferentHeroes;SameHeroes;Duel;MarketSell;MarketBuy;" +
                "AverageTimeBatlles;FreqLosses;CountQuests/Day;AverageTimeQuests;CountActiveDays;AverageInactiveGap;Leave");
                foreach (User us in lst)
                {
                    sumLvl = 0;
                    foreach (int lvl in us.heroMaxLevel)
                        sumLvl += lvl;

                    if (us.heroesId.Count == 1 || sumLvl == 49 || us.endOfTheGap == true)
                    {
                        us.leave = true;
                    }
                    else
                    {
                        sw.WriteLine(us.payment + ";" + us.spending + ";" + (int)us.dayOfWeek + ";" +
                            us.countDiffHeroes + ";" + Convert.ToInt32(us.sameHeroes) + ";" + Convert.ToInt32(us.duel) + ";" + Convert.ToInt32(us.marketSell) +
                            ";" + Convert.ToInt32(us.marketBuy) + ";" +
                            us.battles.GetAverageTime() + ";" + us.battles.GetFreqLosses() + ";" +
                            us.quests.GetAverageCountQuests() + ";" + us.quests.GetAverageTimeQuests() + ";" + us.activeDays.Count + ";" +
                            us.activeDays.AverageInactiveGap() + ";" + Convert.ToInt32(us.leave));
                    }

                }
            }

            ReplaceCommaOnPoint(fileName);
        }
    }
}
