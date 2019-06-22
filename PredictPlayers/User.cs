using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class User
    {
        public string id { get; private set; }
        public double payment;
        public int payCluster;
        public int spendCluster;
        public double spending;
        public DayOfWeek dayOfWeek;
        public List<HeroCount> heroes;
        public int countDiffHeroes { get; private set; }
        public bool sameHeroes { get; private set; }
        public bool duel;
        public List<string> heroesId;
        public List<int> heroMaxLevel;
        public bool marketSell;
        public bool marketBuy;
        public Battles battles;
        public Quests quests;
        public bool leave;
        public double hourLeave;
        public ActiveDays activeDays;
        public bool endOfTheGap;

        public User(string id, DayOfWeek dayOfWeek, List<HeroCount> heroes)
        {
            this.id = id;
            payment = 0;
            spending = 0;
            this.dayOfWeek = dayOfWeek;
            this.heroes = new List<HeroCount>();
            CopyHeroes(heroes);
            countDiffHeroes = 0;
            sameHeroes = false;
            duel = false;
            heroesId = new List<string>();
            heroMaxLevel = new List<int>();
            marketSell = marketBuy = false;
            battles = new Battles();
            quests = new Quests();
            leave = false;
            hourLeave = 0;
            activeDays = new ActiveDays();
            endOfTheGap = false;
        }

        void CopyHeroes(List<HeroCount> heroes)
        {
            foreach (HeroCount hero in heroes)
            {
                HeroCount hc = new HeroCount(hero.name);
                this.heroes.Add(hc);
            }
        }

        public void CountDifferentHeroes()
        {
            int res = 0;
            foreach (HeroCount hero in heroes)
                if (hero.count > 0) res += hero.count;
            countDiffHeroes = res;
        }

        public void SameHeroes()
        {
            foreach (HeroCount hero in heroes)
                if (hero.count > 1) sameHeroes = true;
            sameHeroes = false;
        }
    }
}
