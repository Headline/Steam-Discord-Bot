using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class PonyCommand : ModuleBase
    {
        [Command("pony"), Summary("Grabs link based off pony name.")]
        public async Task Say([Remainder, Summary("Pony name")] string input)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => {
                    return true;
                };

                string pony = new WebClient().DownloadString("https://areweponyyet.com/?chatty=1&pony=" + input);

                var emb = new EmbedBuilder();
                emb.Title = "Pony Link Fetched!";
                emb.WithDescription("URL Fetched: " + "https://areweponyyet.com/" + pony);
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
            catch (Exception ex)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Pony", ex.Message));
                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
