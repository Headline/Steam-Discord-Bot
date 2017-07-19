using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;


namespace ChancyBot.Commands
{
    public class PonyCommand : ModuleBase
    {
        [Command("pony"), Summary("Grabs link based off pony name.")]
        public async Task Say([Remainder, Summary("Pony name")] string input)
        {
            try
            {
                string pony = new WebClient().DownloadString("https://areweponyyet.com/?chatty=1&pony=" + input);

                await Context.Channel.SendMessageAsync( "URL Fetched: " + "https://areweponyyet.com/" + pony);
            }
            catch (Exception ex)
            {
                await ChancyBot.Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Pony", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
