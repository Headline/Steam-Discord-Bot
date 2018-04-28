using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class SetGameCommand : ModuleBase
    {
        [Command("setgame"), Summary("Sets the bot's game. [Owner only]")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {

                input += args[i].Trim() + " ";
            }
            input += args[args.Length - 1];

            if (Context.User.Id != (ulong)Program.config.DiscordAdminId)
            {
                await Context.Channel.SendMessageAsync("Contact "
                                        + Program.config.DiscordAdminContact
                                        + " if you believe I need a new topic.");
                return;
            }

            await Program.Instance.client.SetGameAsync(input);

            var emb = new EmbedBuilder();
            emb.Title = "Game Set!";
            emb.WithDescription("My game is now set to: " + input);
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}
