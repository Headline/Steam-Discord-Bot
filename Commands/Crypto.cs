using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;

namespace ChancyBot.Commands
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

                await Context.Channel.SendMessageAsync("Current Bitcoin Price (USD): " + price);
            }
            catch (Exception ex)
            {
                await ChancyBot.Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Ethereum", ex.Message));

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

                await Context.Channel.SendMessageAsync("Current Ethereum Price (USD): " + price);
            }
            catch (Exception ex)
            {
                await ChancyBot.Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Bitcoin", ex.Message));

                await Context.Channel.SendMessageAsync("Internal Error: " + ex.Message);
            }
        }
    }
}
