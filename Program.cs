using Civ5DraftBot.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civ5DraftBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO check for duplicates
            /*DraftPick draft = new DraftPick();
            draft.Bans.Add(Civilization.Venice);
            draft.NumCivsPerPerson = 5;
            draft.Players.Add(00);
            draft.Players.Add(01);
            draft.Players.Add(02);
            draft.Players.Add(03);
            draft.Players.Add(04);
            draft.Players.Add(05);
            draft.Players.Add(06);
            draft.Players.Add(07);

            draft.GeneratePicks();

            int a = 0;
            foreach(var x in draft.PlayerOptions)
            {
                string s = "";
                foreach(var y in x.picks)
                {
                    s += y.ToString() + " / ";
                }
                Console.WriteLine("Player {0}: {1}", a, s);
                a++;
            }*/

            DiscordBot bot = new DiscordBot();

        }
    }
}
