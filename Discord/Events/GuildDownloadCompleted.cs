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
        internal static Task GuildDownloadCompleted(DiscordClient c, GuildDownloadCompletedEventArgs a)
        {
            return Task.CompletedTask;
        }
    }
}
