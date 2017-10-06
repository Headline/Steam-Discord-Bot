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
