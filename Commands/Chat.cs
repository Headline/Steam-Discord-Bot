using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;

namespace ChancyBot.Commands
{
    public class ChatCommand : ModuleBase
    {
        [Command("chat"), Summary("Uses a Markov model with to generate response text.")]
        public async Task Say()
        {
            string response = MarkovHelper.GetPhraseFromFile(Context.Guild.Name + ".txt");
            await Context.Channel.SendMessageAsync(response);
        }
    }

    public class ChatRemoveCommand : ModuleBase
    {
        [Command("chatremove"), Summary("Removes the term from knowledgebase.")]
        public async Task Say(string term)
        {
            int amount = MarkovHelper.RemoveTermFromFile(Context.Guild.Name + ".txt", term);
            await Context.Channel.SendMessageAsync(string.Format("Removed \"{0}\" from {1} lines", term, amount));
        }
    }

    public class ChatKnowledgeBaseCommand : ModuleBase
    {
        [Command("chatknowledge"), Summary("Sends a pastebin link containing it's knowledgebase.")]
        public async Task Say()
        {
            string knowledgebase = MarkovHelper.GetHastebinLink(Context.Guild.Name + ".txt");
            await Context.Channel.SendMessageAsync("Here's my knowlege base: " + knowledgebase);
        }
    }
    /*public class ChatAboutCommand : ModuleBase
    {
        [Command("chatabout"), Summary("Uses a Markov model with to generate response text including the param.")]
        public async Task Say(string name)
        {
            try
            {
                string response = MarkovHelper.GetPhraseFromFile(Context.Guild.Name + ".txt", name);
                await Context.Channel.SendMessageAsync(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }*/
}
