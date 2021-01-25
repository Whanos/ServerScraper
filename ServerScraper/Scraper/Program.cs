using System;
using System.IO;
using System.Threading;
using Discord;
using Discord.Gateway;
using Newtonsoft.Json;

namespace ServerScraper
{
    internal static class Program
    {
        private static DiscordSocketConfig config = new DiscordSocketConfig();
        public static DiscordSocketClient client = new DiscordSocketClient(config);
        
        public static string token;
        public static ulong Serverid;
        public static ulong Channelid;

        private static void Main(string[] args)
        {
            var botconfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            token = botconfig.CToken;
            Serverid = botconfig.CServerId;
            Channelid = botconfig.CChannelId;
            config.RetryOnRateLimit = false;
            config.Cache = true;
            client = new DiscordSocketClient(config);
            client.OnLoggedIn += client_OnLoggedIn;
            client.Login(token);
            Thread.Sleep(2000);
            
            //Run.
            GetIDs();
            Thread.Sleep(-1);
        }

        private static void client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Console.WriteLine("Logged in as " + client.User);
        }

        private static void GetIDs()
        {
            using (var write = new StreamWriter("UserIDs.txt", false))
            {
                write.WriteLine("---[USERIDs]---");
                try
                {
                    var guild = client.GetCachedGuild(Serverid);
                    var mcount = guild.MemberCount;
                    if (mcount >= 1000) Console.WriteLine("[WARN] Server has 1000+ members, expect quirky behaviour!");
                    using (StreamWriter writer = new StreamWriter("UserList.txt", true))
                    {
                        foreach (GuildMember member in client.GetGuildChannelMembers(guild, Channelid))
                        {
                            writer.WriteLine(member.User.Id);
                        }
                    }
                }
                catch (DiscordHttpException e)
                {
                    Console.WriteLine("This guild is not cached! Sorry, this is an API limitation.\n" + e);
                }
            }
            Console.WriteLine("Done!");
            Console.WriteLine("Ending...");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
        private class Config
        {
            public string CToken { get; set; }
            public ulong CServerId { get; set; }
            public ulong CChannelId { get; set; }
        }
    }
}