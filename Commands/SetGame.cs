using System.Threading.Tasks;

using Discord.Commands;

namespace ChancyBot.Commands
{
    public class SetGameCommand : ModuleBase
    {
        [Command("setgame"), Summary("Sets the bot's game. [Headline only].")]
        public async Task Say(string input)
        {
            if (Context.User.Id != 194315619217178624)
            {
                await Context.Channel.SendMessageAsync("Contact Headline#9572 if you believe I need a new topic.");
                return;
            }

            await Program.Instance.client.SetGameAsync(input);
            await Context.Channel.SendMessageAsync("My topic is now: " + input);
        }
    }
}
