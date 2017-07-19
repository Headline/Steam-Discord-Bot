using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;

namespace ChancyBot.Commands
{
    public class NumPlayersCommand : ModuleBase
    {
        [Command("numplayers"), Summary("Fetches current player count from steam.")]
        public async Task Say([Remainder, Summary("The appid to search for")] int appid)
        {
            try
            {
                // GET Game Name
                var json = new WebClient().DownloadString("http://store.steampowered.com/api/appdetails?appids=" + appid);
                JObject o = JObject.Parse(json);
                string name = (string)o["" + appid]["data"]["name"];

                // GET Player Count
                json = new WebClient().DownloadString("https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?&appid=" + appid);
                o = JObject.Parse(json);
                string count = (string)o["response"]["player_count"];

                await Context.Channel.SendMessageAsync(name + " player count: " + count);
            }
            catch (Exception ex)
            {
                await ChancyBot.Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "NumPlayers", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
