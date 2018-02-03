using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;

using ChancyBot.Jobs;
using ChancyBot.Steam;
using System.Linq;
using System.Collections.Generic;
using Octokit;
using ChancyBot.Commands;

namespace ChancyBot
{
    class Program
    {
        public static readonly string VERSION = "$$version$";
        // STEAM
        public SteamConnection connection;
        public string[] helpLines;
        public JobManager manager;

        // DISCORD
        public DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        // GITHUB
        public GitHubClient ghClient;
        public IReadOnlyList<Release> ghReleases;


        public static Program Instance;
        public List<MsgInfo> messageHist;

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
                Environment.Exit(0);
            }

            Config.Instance.Save();

            Console.WriteLine("Bot starting up. ({0} by Michael Flaherty)", Program.VERSION);
            Console.WriteLine("Using token: " + Config.Instance.DiscordBotToken);

            client = new DiscordSocketClient();
            commands = new CommandService();
            messageHist = new List<MsgInfo>();

            client.Log += Log;
            
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, Config.Instance.DiscordBotToken);
            await client.StartAsync();

            // Connect to steam and pump callbacks 
            connection = new SteamConnection(Config.Instance.SteamUsername, Config.Instance.SteamPassword);
            connection.Connect();
            Console.WriteLine("Pumping steam connection...");

            // Handle Jobs
            manager = new JobManager(30); // seconds to run each job
            new Thread(new ThreadStart(async () =>
            {
                ghClient = new GitHubClient(new ProductHeaderValue("Steam-Discord-Bot"));
                ghReleases = await Task.Run(() => ghClient.Repository.Release.GetAll("Headline", "Steam-Discord-Bot"));
                if (Config.Instance.GitHubAuthToken.Length != 0)
                    ghClient.Credentials = new Credentials(Config.Instance.GitHubAuthToken);

                // Calls updater.py when out of date
                manager.AddJob(new SelfUpdateListener(ghClient));

                // job to check steam connection
                manager.AddJob(new SteamCheckJob(connection)); 

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

            helpLines = BuildHelpLines();
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
                MarkovHelper.WriteLineToFile(context.Guild.Name + ".txt", message.Content);
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
    }
}
