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

            Instance.MainAsync().GetAwaiter().GetResult();
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
                var manager = new JobManager(25); // seconds to run each job

                // Create update jobs for each desired appid
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
