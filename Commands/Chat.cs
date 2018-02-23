using System.Threading.Tasks;

using Discord.Commands;


namespace ChancyBot.Commands
{
    public class ChatCommand : ModuleBase
    {
        [Command("chat"), Summary("Uses a Markov model with to generate response text.")]
        public async Task Say()
        {
            string response = Program.Instance.markov.ReadFromGuild(Context.Guild.Name);
            await Context.Channel.SendMessageAsync(response);
            Program.Instance.markov.BuildNext(Context.Guild.Name);
        }
    }

    public class ChatRemoveCommand : ModuleBase
    {
        [Command("chatremove"), Summary("Removes the term from knowledgebase.")]
        public async Task Say(string term)
        {
            if (Context.User.Id != 194315619217178624)
            {
                await Context.Channel.SendMessageAsync("Contact Headline#9572 if you believe terms should be removed.");
                return;
            }

            int amount = Program.Instance.markov.RemoveFromGuild(Context.Guild.Name, term);
            await Context.Channel.SendMessageAsync(string.Format("Removed \"{0}\" from {1} lines", term, amount));
        }
    }

    public class ChatKnowledgeBaseCommand : ModuleBase
    {
        [Command("chatknowledge"), Summary("Sends a pastebin link containing its knowledgebase.")]
        public async Task Say()
        {
            string knowledgebase = Program.Instance.markov.GetHastebinLink(Context.Guild.Name);
            await Context.Channel.SendMessageAsync("Here's my knowlege base: " + knowledgebase);
        }
    }
}
