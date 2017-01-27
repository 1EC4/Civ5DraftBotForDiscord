using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civ5DraftBot
{
    class DraftPick
    {
        public List<ulong> Players { get; }
        public List<Civilization> Bans { get; }
        public List<Picks> PlayerOptions { get; }
        public int NumCivsPerPerson { get; set; }

        private Random gen;

        public DraftPick()
        {
            Players = new List<ulong>();
            Bans = new List<Civilization>();
            PlayerOptions = new List<Picks>();
            NumCivsPerPerson = 3;

            gen = new Random();
        }

        public void GeneratePicks()
        {
            PlayerOptions.Clear();
            for(int i = 0; i < Players.Count; i++)
            {
                Picks picks = new Picks(NumCivsPerPerson);
                for(int j = 0; j < NumCivsPerPerson; j++)
                {
                    Civilization civ;
                    do
                    {
                        civ = (Civilization)gen.Next(0, 43);
                    } while (Bans.Contains(civ) || duplicate(civ, picks));
                    picks.picks[j] = civ;
                }

                PlayerOptions.Add(picks);
            }
        }

        private bool duplicate(Civilization civ, Picks picks)
        {
            for (int i = 0; i < NumCivsPerPerson; i++)
            {
                if (picks.picks[i] == civ)
                {
                    return true;
                }
            }
            return false;
        }

    }

    class Picks
    {
        public Civilization[] picks;

        public Picks(int numCivs)
        {
            picks = new Civilization[numCivs];
        }
    }
}
