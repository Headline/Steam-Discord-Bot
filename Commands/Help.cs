using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class HelpCommand : ModuleBase
    {
        [Command("help"), Summary("Prints and formats all commands.")]
        public async Task Say()
        {
            string message = "";
            foreach(string line in Program.Instance.helpLines)
            {
                message += line + "\n";
            }

            await Context.Channel.SendMessageAsync(message);
        }
    }
}