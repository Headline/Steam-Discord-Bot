using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class ChatCommand : ModuleBase
    {
        [Command("chat"), Summary("Uses a Markov model with to generate response text.")]
        public async Task Say()
        {
            string response = Program.Instance.markov.ReadFromGuild(Context.Guild.Id);
            await Context.Channel.SendMessageAsync(response);
            Program.Instance.markov.BuildNext(Context.Guild.Id);
        }
    }

    public class ChatRemoveCommand : ModuleBase
    {
        [Command("chatremove"), Summary("Removes the term from knowledgebase. [Owner only]")]
        public async Task Say(string term)
        {
            if (Context.User.Id != (ulong)Program.config.DiscordAdminId)
            {
                await Context.Channel.SendMessageAsync("Contact "
                                        + Program.config.DiscordAdminContact
                                        + " if you believe terms should be removed.");
                return;
            }

            int amount = await Program.Instance.markov.RemoveFromGuild(Context.Guild.Id, term);
            await Context.Channel.SendMessageAsync(string.Format("Removed \"{0}\" from {1} lines", term, amount));
        }
    }

    public class ChatKnowledgeBaseCommand : ModuleBase
    {
        [Command("chatknowledge"), Summary("Sends a pastebin link containing its knowledgebase.")]
        public async Task Say()
        {
            string knowledgebase = Program.Instance.markov.GetHastebinLink(Context.Guild.Id);
            await Context.Channel.SendMessageAsync("Here's my knowlege base: " + knowledgebase);
        }
    }
}
