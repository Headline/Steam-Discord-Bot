using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class SetGameCommand : ModuleBase
    {
        [Command("setgame"), Summary("Sets the bot's game. [Owner only]")]
        public async Task Say(string input)
        {
            if (Context.User.Id != (ulong)Program.config.DiscordAdminId)
            {
                await Context.Channel.SendMessageAsync("Contact "
                                        + Program.config.DiscordAdminContact
                                        + " if you believe I need a new topic.");
                return;
            }

            await Program.Instance.client.SetGameAsync(input);
            await Context.Channel.SendMessageAsync("My topic is now: " + input);
        }
    }
}
