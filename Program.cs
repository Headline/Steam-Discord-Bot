using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;

using ChancyBot.Jobs;
using ChancyBot.Steam;

namespace ChancyBot
{
    class Program
    {
        // STEAM
        public SteamConnection connection;

        // DISCORD
        public DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        public static Program Instance;

        public static void Main(string[] args)
        {
            Instance = new Program();

            try
            {
                Instance.MainAsync().GetAwaiter().GetResult();
            }
            catch
            {
                Console.WriteLine("Internal error. Ensure settings.xml is configured correctly");
                Console.ReadKey();
            }
        }

        private async Task MainAsync()
        {
			try
			{
				Config.Load();
			}
			catch (Exception)
			{
				Console.WriteLine("Failed to load config from file. Loading default config.");
				Config.Default();
                Console.WriteLine("Please configure Settings.xml! Exiting program...");
                Environment.Exit(0);
                Console.ReadKey();
            }

            Config.Instance.Save();

			Console.WriteLine("Using token: " + Config.Instance.DiscordBotToken);

			client = new DiscordSocketClient();
            commands = new CommandService();

            client.Log += Log;
            
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, Config.Instance.DiscordBotToken);
            await client.StartAsync();

            // Connect to steam and pump callbacks 
            connection = new SteamConnection(Config.Instance.SteamUsername, Config.Instance.SteamPassword);
			new Thread(new ThreadStart(() =>
            {
                connection.Connect();
            })).Start();

            // Handle Jobs
			new Thread(new ThreadStart(() =>
            {
                var manager = new JobManager(120); // seconds to run each job

                manager.AddJob(new SteamCheckJob(connection)); // job to check steam connection

                manager.AddJob(new GithubUpdateJob("https://github.com/alliedmodders/sourcemod/commits/master.atom", "sourcemod"));
                manager.AddJob(new GithubUpdateJob("https://github.com/alliedmodders/sourcepawn/commits/master.atom", "sourcemod"));
                manager.AddJob(new GithubUpdateJob("https://github.com/alliedmodders/ambuild/commits/master.atom", "sourcemod"));
                manager.AddJob(new GithubUpdateJob("https://github.com/alliedmodders/metamod-source/commits/master.atom", "sourcemod"));
                manager.AddJob(new GithubUpdateJob("https://github.com/alliedmodders/hl2sdk/commits/sdk2013.atom", "sourcemod"));

                manager.AddJob(new AlliedModdersThreadJob("https://forums.alliedmods.net/external.php?newpost=true&forumids=108", "sourcemod"));
                // add appids 
                foreach (uint app in Config.Instance.AppIDList)
                {
                    manager.AddJob(new UpdateJob(app));
                }

                manager.StartJobs();
            })).Start();


            await Task.Delay(-1);
        }


        private async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return null;
        }
    }
}
