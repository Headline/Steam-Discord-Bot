using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class UpdateCommand : ModuleBase
    {
        [Command("update"), Summary("Updates and reloads the bot. [Owner only]")]
        public async Task Say()
        {
            if (Program.config.GitHubAuthToken.Length == 0)
            {
                await Context.Channel.SendMessageAsync("Bot updating is disabled!");
                return;
            }
            if (Context.User.Id != (ulong)Program.config.DiscordAdminId)
            {
                await Context.Channel.SendMessageAsync("Contact " 
                                        + Program.config.DiscordAdminContact
                                        + " if you believe I should be updated.");
                return;
            }

            var emb = new EmbedBuilder();
            emb.Title = "Update";
            emb.WithDescription("Okay. Updating...");
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);

            Update();
        }
        public async static void Update()
        {
            if (Program.config.GitHubAuthToken.Length == 0)
            {
                return;
            }

            try
            {
                /*
                 * We give the python script our pid, that way
                 * it can loop and wait for the application to completely
                 * exit before replacing our files
                 */
                int pid = Process.GetCurrentProcess().Id;

                var client = Program.Instance.ghClient;
                var releases = await client.Repository.Release.GetAll(Program.config.GitHubUpdateUsername, Program.config.GitHubUpdateRepository);

                string url = "https://github.com/"
                    + Program.config.GitHubUpdateUsername
                    + "/"
                    + Program.config.GitHubUpdateRepository
                    + "/releases/download/<name>/"
                    + Program.config.GitHubUpdateReleaseFilename;
                url = url.Replace("<name>", releases[0].Name);

                string command = string.Format("/k cd {0} & python updater.py {1} {2}",
                                                                Directory.GetCurrentDirectory(),
                                                                pid,
                                                                url);

                Process.Start("CMD.exe", command);
                Environment.Exit(0);
            }
            catch { } // ignore errors. if it failed: oh well.
        }
    }
}
