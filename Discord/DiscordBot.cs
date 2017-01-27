using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using System.Threading;

namespace Civ5DraftBot.Discord
{
    class DiscordBot
    {

        private DiscordClient discord;
        private DraftPick draft;
        private string token = "";
        long draft_expire = -1;

        public DiscordBot()
        {

            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x => {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = false;
            });

            var commands = discord.GetService<CommandService>();

            commands.CreateCommand("start_draft")
                .Description("Starts the draft process")
                .Do(async (e) => {
                    var voice = e.User.VoiceChannel;

                    string r;
                    if (voice != null && voice.Type == ChannelType.Voice)
                    {
                        draft = new DraftPick();

                        draft_expire = Environment.TickCount;
                        draft_expire += 10 * 60 * 1000;

                        Thread t_expire = new Thread(new ThreadStart(expire));
                        t_expire.Start();

                        r = e.User.Mention + " The following users will be part of the draft:\n";
                        foreach (var users in voice.Users)
                        {
                            r += users.Name + "\n";
                            draft.Players.Add(users.Id);
                        }
                        
                        r += "\n\nThe draft will expire in 10 minutes";
                    } else
                    {
                        r = e.User.Mention + " please join a voice channel on this server.";
                    }
                    await e.Channel.SendMessage(r);
                });

            commands.CreateCommand("set_civs_per_player")
                .Parameter("num", ParameterType.Required)
                .Do(async (e) => {
                    string r = e.User.Mention;

                    if (draft == null)
                    {
                        r += " the draft has not been started yet. Use !start_draft first.";
                    }
                    else
                    {
                        var x = e.GetArg("num");
                        try
                        {
                            int num = int.Parse(x);
                            if (num <= 0)
                                throw new Exception("The value cannot be lower than 1");
                            if (num > 15)
                                throw new Exception("The value cannot be higher than 10");

                            draft.NumCivsPerPerson = num;
                            r += " The number of civs per person was set to " + num;
                        }
                        catch (Exception ex)
                        {
                            r += " The following error was encountered: " + ex.Message;
                        }
                    }
                    await e.Channel.SendMessage(r);
                });

            commands.CreateCommand("ban_civ")
                .Parameter("civ", ParameterType.Required)
                .Do(async (e) => {
                    string r = e.User.Mention;

                    if (draft == null)
                    {
                        r += " the draft has not been started yet. Use !start_draft first.";
                    } else
                    {
                        var x = e.GetArg("civ");
                        Civilization civ = getCiv(x);
                        if (!draft.Bans.Contains(civ))
                        {
                            draft.Bans.Add(civ);
                            r += " added " + civ.ToString() + " to the ban list.";
                        } else
                        {
                            r += civ.ToString() + " is already on the ban list.";
                        }
                    }

                    await e.Channel.SendMessage(r);
                });

            commands.CreateCommand("unban_civ")
                .Parameter("civ", ParameterType.Required)
                .Do(async (e) => {
                    string r = e.User.Mention;

                    if (draft == null)
                    {
                        r += " the draft has not been started yet. Use !start_draft first.";
                    } else
                    {
                        var x = e.GetArg("civ");
                        Civilization civ = getCiv(x);
                        if (draft.Bans.Contains(civ))
                        {
                            draft.Bans.Remove(civ);
                            r += " removed " + civ.ToString() + " from the ban list.";
                        } else
                        {
                            r += civ.ToString() + " is not on the ban list.";
                        }
                    }

                    await e.Channel.SendMessage(r);
                });

            commands.CreateCommand("banned_civs")
                .Do(async (e) => {
                    string r = e.User.Mention + "\n";

                    if (draft == null)
                    {
                        r += " the draft has not been started yet. Use !start_draft first.";
                    } else
                    {
                        foreach(var civ in draft.Bans)
                        {
                            r += civ.ToString() + "\n";
                        }

                        if (draft.Bans.Count == 0)
                        {
                            r += " the ban list is empty.";
                        }
                    }

                    await e.Channel.SendMessage(r);
                });

            commands.CreateCommand("generate")
                .Do(async (e) => {
                    if (draft == null)
                    {
                        await e.Channel.SendMessage(e.User.Mention + " the draft has not been started yet. Use !start_draft first.");
                    } else
                    {
                        draft.GeneratePicks();
                        string r = "";

                        for(int i = 0; i < draft.Players.Count; i++)
                        {
                            r += e.Server.GetUser(draft.Players[i]).Mention + ": ";
                            for(int j = 0; j < draft.NumCivsPerPerson; j++)
                            {
                                r += draft.PlayerOptions[i].picks[j].ToString();

                                if (j != draft.NumCivsPerPerson - 1)
                                {
                                    r += " / ";
                                } else
                                {
                                    r += "\n";
                                }
                            }
                        }

                        await e.Channel.SendMessage(r);
                    }

                });

            discord.ExecuteAndWait(async() => {
                await discord.Connect(token, TokenType.Bot);
            });
        }

        private void expire()
        {
            while(Environment.TickCount < draft_expire)
            {
                Thread.Sleep(1);
            }
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private Civilization getCiv(string civ)
        {
            Civilization civil;

            switch (civ.ToLower())
            {
                case "america":
                    civil = Civilization.America;
                    break;
                case "arabia":
                    civil = Civilization.Arabia;
                    break;
                case "assyria":
                    civil = Civilization.Assyria;
                    break;
                case "austria":
                    civil = Civilization.Austria;
                    break;
                case "aztec":
                    civil = Civilization.Aztec;
                    break;
                case "babylon":
                    civil = Civilization.Babylon;
                    break;
                case "brazil":
                    civil = Civilization.Brazil;
                    break;
                case "byzantium":
                    civil = Civilization.Byzantium;
                    break;
                case "carthage":
                    civil = Civilization.Carthage;
                    break;
                case "celts":
                    civil = Civilization.Celts;
                    break;
                case "china":
                    civil = Civilization.China;
                    break;
                case "denmark":
                    civil = Civilization.Denmark;
                    break;
                case "egypt":
                    civil = Civilization.Egypt;
                    break;
                case "england":
                    civil = Civilization.England;
                    break;
                case "ethiopia":
                    civil = Civilization.Ethiopia;
                    break;
                case "france":
                    civil = Civilization.France;
                    break;
                case "germany":
                    civil = Civilization.Germany;
                    break;
                case "greece":
                    civil = Civilization.Greece;
                    break;
                case "huns":
                    civil = Civilization.Huns;
                    break;
                case "inca":
                    civil = Civilization.Inca;
                    break;
                case "india":
                    civil = Civilization.India;
                    break;
                case "indonesia":
                    civil = Civilization.Indonesia;
                    break;
                case "iroquois":
                    civil = Civilization.Iroquois;
                    break;
                case "japan":
                    civil = Civilization.Japan;
                    break;
                case "korea":
                    civil = Civilization.Korea;
                    break;
                case "maya":
                    civil = Civilization.Maya;
                    break;
                case "mongolia":
                    civil = Civilization.Mongolia;
                    break;
                case "morocco":
                    civil = Civilization.Morocco;
                    break;
                case "netherlands":
                    civil = Civilization.Netherlands;
                    break;
                case "ottomans":
                    civil = Civilization.Ottomans;
                    break;
                case "persia":
                    civil = Civilization.Persia;
                    break;
                case "poland":
                    civil = Civilization.Poland;
                    break;
                case "polynesia":
                    civil = Civilization.Polynesia;
                    break;
                case "portugal":
                    civil = Civilization.Portugal;
                    break;
                case "rome":
                    civil = Civilization.Rome;
                    break;
                case "russia":
                    civil = Civilization.Russia;
                    break;
                case "shoshone":
                    civil = Civilization.Shoshone;
                    break;
                case "siam":
                    civil = Civilization.Siam;
                    break;
                case "songhai":
                    civil = Civilization.Songhai;
                    break;
                case "spain":
                    civil = Civilization.Spain;
                    break;
                case "sweden":
                    civil = Civilization.Sweden;
                    break;
                case "venice":
                    civil = Civilization.Venice;
                    break;
                case "zulu":
                    civil = Civilization.Zulu;
                    break;
                default:
                    civil = Civilization.America;
                    break;
            }

            return civil;
        }
    }
}
