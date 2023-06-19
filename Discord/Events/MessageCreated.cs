using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pajoma_nvtbot.Discord
{
    static public partial class Events
    {
        public static Task MessageAdded(DiscordClient c, MessageCreateEventArgs arg)
        {
            if (arg.Author == c.CurrentUser) return Task.CompletedTask;
            
            if (arg.Message.Content.StartsWith("/"))
            {
                string[] cmdSplit = arg.Message.Content.Remove(0, 1).Split(' ');

                switch (cmdSplit[0])
                {
                    case "register":
                        //arg.Message.Channel.SendMessageAsync("Hello");
                        if(cmdSplit.Length != 3) { arg.Message.Channel.SendMessageAsync("Nutze /register (Nummer) (Password) oder /stop (Nummer) | Urlaub: W.I.P");  break; }

                        int plchd;
                        if (int.TryParse(cmdSplit[1], out plchd) || !string.IsNullOrEmpty(cmdSplit[2]) || !string.IsNullOrWhiteSpace(cmdSplit[2]))
                        {
                            Program.m_UserList.Add(new Users.User(Convert.ToInt16(cmdSplit[1]), cmdSplit[2], arg.Channel.Id.ToString()));
                            arg.Message.Channel.SendMessageAsync("Infonachrichten Timer für tägliche Leidensstatistik aktiviert.");
                        }
                        else
                        {
                            arg.Message.Channel.SendMessageAsync("Ein Personalzeitkonto unter den angegeben Daten existiert nicht oder das Passwort war falsch.");
                        }
                        break;

                    case "stop":
                        //arg.Message.Channel.SendMessageAsync("Hello");
                        break;
                }
            }
            else arg.Message.Channel.SendMessageAsync("Nutze /register (Nummer) (Password) oder /stop (Nummer) | Urlaub: W.I.P");
            return Task.CompletedTask;
        }
    }
}
