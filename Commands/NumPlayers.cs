using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace SteamDiscordBot.Commands
{
    public class NumPlayersCommand : ModuleBase
    {
        [Command("numplayers"), Summary("Fetches current player count from steam.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {

                input += args[i].Trim() + " ";
            }
            input += args[args.Length - 1];
            int appid = AppInfoCommand.InputToAppId(input);

            if (appid == -1)
            {
                var emb = new EmbedBuilder();
                emb.Title = "Error!";
                emb.WithDescription("No games found! :(");
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
                return;
            }

            try
            {
                // GET Game Name
                var json = new WebClient().DownloadString("http://store.steampowered.com/api/appdetails?appids=" + appid);
                JObject o = JObject.Parse(json);
                string name = (string)o["" + appid]["data"]["name"];

                // GET Player Count
                json = new WebClient().DownloadString("https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?&appid=" + appid);
                o = JObject.Parse(json);
                string count = (string)String.Format("{0:N0}", o["response"]["player_count"]);

                var emb = new EmbedBuilder();
                emb.Title = "Player Count";
                emb.WithDescription(name + " player count: " + count);
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
            catch (NullReferenceException)
            {
                await Context.Channel.SendMessageAsync("Invalid appid!");
            }
            catch (Exception ex)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "NumPlayers", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
