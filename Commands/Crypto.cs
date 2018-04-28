using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace SteamDiscordBot.Commands
{
    public class BitcoinCommand : ModuleBase
    {
        [Command("btc"), Summary("Fetches the latest Bitcoin market value.")]
        public async Task Say()
        {
            try
            {
                var json = new WebClient().DownloadString("https://min-api.cryptocompare.com/data/price?fsym=BTC&tsyms=USD");
                JObject obj = JObject.Parse(json);
                string price = (string)obj["USD"];

                var emb = new EmbedBuilder();
                emb.Title = "Bitcoin Price";
                emb.WithDescription(string.Format("Current BTC Price (USD): $**{0}**", price));
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
            catch (Exception ex)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Ethereum", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }

    public class EthereumCommand : ModuleBase
    {
        [Command("eth"), Summary("Fetches the latest Ethereum market value.")]
        public async Task Say()
        {
            try
            {
                var json = new WebClient().DownloadString("https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD");
                JObject obj = JObject.Parse(json);
                string price = (string)obj["USD"];

                var emb = new EmbedBuilder();
                emb.Title = "Ethereum Price";
                emb.WithDescription(string.Format("Current ETH Price (USD): $**{0}**", price));
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
            catch (Exception ex)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Bitcoin", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
