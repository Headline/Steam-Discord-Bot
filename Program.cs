using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;

using SteamDiscordBot.Jobs;
using SteamDiscordBot.Steam;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using Octokit;
using Discord.Net.Providers.WS4Net;

using JsonConfig;
using SteamDiscordBot.Commands;

namespace SteamDiscordBot
{
    class Program
    {
        public static readonly string VERSION = "$$version$";

        // STEAM
        public SteamConnection connection;
        public JobManager manager;

        // DISCORD
        public DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        // GITHUB
        public GitHubClient ghClient;

        // MARKOV
        public Markov markov;

        // BOT
        public static Program Instance;
        public static dynamic config;
        public string[] helpLines;
        public List<MsgInfo> messageHist;
        public static uint ownerId;

        public static void Main(string[] args)
        {
            try
            {
                var reader = new StreamReader("settings.json");
                config = Config.ApplyJson(reader.ReadToEnd(), new ConfigObject());
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load configuration file settings.json!\nReason:" + e.Message);
                Environment.Exit(0);
            }

            if (config.DiscordBotToken.Length == 0)
            {
                Console.WriteLine("You must supply a DiscordBotToken!");
                Environment.Exit(0);
            }

            uint.TryParse(config.DiscordAdminId, out Program.ownerId); // cache owner id for later

            Instance = new Program();
            Instance.MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            var startupStr = string.Format("Bot starting up. ({0} by Michael Flaherty)", Program.VERSION);
            await Log(new LogMessage(LogSeverity.Info, "Startup", startupStr));

            var socketConfig = new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance,
                LogLevel = LogSeverity.Verbose
            };

            client = new DiscordSocketClient(socketConfig);
            commands = new CommandService();
            messageHist = new List<MsgInfo>();
            markov = new Markov();

            client.Log += Log;
            client.GuildAvailable += OnGuildAvailable;
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, config.DiscordBotToken);
            await client.StartAsync();

            // Connect to steam and pump callbacks 
            connection = new SteamConnection(config.SteamUsername, config.SteamPassword);
            connection.Connect();

            ghClient = new GitHubClient(new ProductHeaderValue("Steam-Discord-Bot"));
            if (config.GitHubAuthToken.Length != 0)
                ghClient.Credentials = new Credentials(config.GitHubAuthToken);

            // Handle Jobs
            manager = new JobManager(config.JobInterval); // time in seconds to run each job
            new Thread(new ThreadStart(() =>
            {
                if (config.SelfUpdateListener && config.GitHubAuthToken.Length == 0)
                    manager.AddJob(new SelfUpdateListener());
                if (config.SteamCheckJob)
                    manager.AddJob(new SteamCheckJob(connection));
                if (config.AlliedModdersThreadJob)
                    manager.AddJob(new AlliedModdersThreadJob("https://forums.alliedmods.net/external.php?newpost=true&forumids=108", "sourcemod"));
                
                foreach (string app in config.AppIDList)
                {
                    uint.TryParse(app, out uint appid);
                    manager.AddJob(new UpdateJob(appid));
                }

                manager.StartJobs();
            })).Start();

            await Task.Delay(-1);
        }

        private async Task OnGuildAvailable(SocketGuild arg)
        {
            await Task.Run(() => markov.AddGuild(arg.Name));
        }

        private async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            helpLines = BuildHelpLines();
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            var context = new CommandContext(client, message);

            if (message.Author.IsBot) return;

            if (!(message.HasCharPrefix('!', ref argPos)
                || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                MsgInfo info = new MsgInfo()
                {
                    message = message.Content,
                    user = message.Author.Id
                };
                messageHist.Add(info);
                markov.WriteToGuild(context.Guild.Name, message.Content);
                return;
            }

            if (Helpers.IsCommandDisabled(message))
            {
                await context.Channel.SendMessageAsync("That command is disabled!");
                return;
            }

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public Task Log(LogMessage msg)
        {
            return Task.Run(() => Console.WriteLine(msg.ToString()));
        }

        public string[] BuildHelpLines()
        {
            List<string> arrayList = new List<string>();

            Assembly asm = Assembly.GetExecutingAssembly(); // Get assembly

            var results = from type in asm.GetTypes()
                          where typeof(ModuleBase).IsAssignableFrom(type)
                          select type; // Grab all types that inherit ModuleBase

            foreach (Type t in results) // For each type in results
            {
                /* Grab MethodInfo of the type where the method has the attribute SummaryAttribute */
                MethodInfo info = t.GetMethods().Where(x => x.GetCustomAttributes(typeof(SummaryAttribute), false).Length > 0).First();

                /* Grab summary attribute */
                SummaryAttribute summary = info.GetCustomAttribute(typeof(SummaryAttribute)) as SummaryAttribute;

                /* Grab command attribute */
                CommandAttribute command = info.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;

                /* Both objects are non null, valid, so lets grab the attribute text */
                if (summary != null && command != null)
                {
                    arrayList.Add("!" + command.Text + " - " + summary.Text);
                }
            }

            return arrayList.ToArray(); // return string[] array
        }
    }
}
