using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class DuckrCommand : ModuleBase
    {
        [Command("duckr"), Summary("Appends input text to a picture of psychonic. Removes original message")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                input += args[i] + " ";
            }
            input += args[args.Length - 1];

            Helpers.CreateDuck(input);
            await Context.Message.DeleteAsync();
            await Context.Channel.SendFileAsync("temp.jpeg");
        }
    }

    public class DuckCommand : ModuleBase
    { 
        [Command("duck"), Summary("Appends input text to a picture of psychonic.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                input += args[i] + " ";
            }
            input += args[args.Length - 1];

            Helpers.CreateDuck(input);
            await Context.Channel.SendFileAsync("temp.jpeg");
        }
    }
}
