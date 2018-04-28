using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class UpdateCommand : ModuleBase
    {
        [Command("update"), Summary("Updates and reloads the bot. [Owner only]")]
        public async Task Say()
        {
            if (Program.config.GitHubAuthToken.Length == 0)
            {
                await Context.Channel.SendMessageAsync("Bot updating is disabled!");
                return;
            }
            if (Context.User.Id != (ulong)Program.config.DiscordAdminId)
            {
                await Context.Channel.SendMessageAsync("Contact " 
                                        + Program.config.DiscordAdminContact
                                        + " if you believe I should be updated.");
                return;
            }

            var emb = new EmbedBuilder();
            emb.Title = "Update";
            emb.WithDescription("Okay. Updating...");
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);

            Helpers.Update();
        }
    }
}
