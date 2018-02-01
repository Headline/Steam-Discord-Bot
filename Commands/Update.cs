using System.Threading.Tasks;

using Discord.Commands;

namespace ChancyBot.Commands
{
    public class UpdateCommand : ModuleBase
    {
        [Command("update"), Summary("Updates and reloads the bot.")]
        public async Task Say()
        {
           if (Context.User.Id != 194315619217178624)
            {
                await Context.Channel.SendMessageAsync("Contact Headline#9572 if you believe I should be updated.");
                return;
            }
            
            await Context.Channel.SendMessageAsync("Okay. Updating...");
            Helpers.Update();
        }
    }
}
