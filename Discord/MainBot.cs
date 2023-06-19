using DSharpPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pajoma_nvtbot.Discord
{
    public static class MainBot
    {

        public static DiscordClient? Client { get; private set; }
        //public InteractivityExtension Interactivity { get; private set; }
        //public CommandsNextExtension Commands { get; private set; }

        public static void Init(string auth_token)
        {

            var dConfig = new DiscordConfiguration()
            {
                Token = auth_token,

                //Token = INI,
                TokenType = TokenType.Bot,
                AlwaysCacheMembers = false,
                MessageCacheSize = 4065,
                Intents = DiscordIntents.DirectMessageReactions
                | DiscordIntents.DirectMessages
                | DiscordIntents.GuildMessageReactions
                | DiscordIntents.GuildBans
                | DiscordIntents.GuildEmojis
                | DiscordIntents.GuildInvites
                | DiscordIntents.GuildMembers
                | DiscordIntents.GuildMessages
                | DiscordIntents.Guilds
                | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildWebhooks,
                AutoReconnect = true
            };

            RunAsync(dConfig).GetAwaiter();
        }

        public static async Task RunAsync(DiscordConfiguration dConfig)
        {
            //Try to create a new discord client, this is in the scope of the DC+' code.. 
            try
            {

                Client = new DiscordClient(dConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.Source + "\n" + ex.StackTrace);
                //Utils.Log.Exception(ex);
                Stop();
                return;
            }


            //Liten to all the Discord Events

            Client.GuildDownloadCompleted += Discord.Events.GuildDownloadCompleted;
            //Client.Heartbeated += DCAMXShared.Discord.Events.Discord.Heartbeated;
            
            Client.MessageCreated += Discord.Events.MessageAdded;
            
            //Client.ChannelCreated += DCAMXShared.Discord.Events.ChannelEvents.ChannelCreated;
            



            /*var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "?" },
                EnableMentionPrefix = true,
             
                EnableDms = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            */
            /*Client.UseInteractivity(new InteractivityConfiguration
            {

            });
            */

            //Finally, connect the bot. Also, in the scope of DC+' code.
            try
            {
                if (Client != null) await Client.ConnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.Source + "\n" + ex.StackTrace + ex.InnerException);
                //Utils.Log.Exception(ex);
                Stop();
            }



        }





        public static async Task DisconnectAsync()
        {
            if (Client != null) await Client.DisconnectAsync();
        }


        public static void Start()
        {

        }

        public async static void Stop ()
        {
            await DisconnectAsync();
        }
    }
}
