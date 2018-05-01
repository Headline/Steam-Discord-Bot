using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class LearnCommand : ModuleBase
    {
        [Command("learn"), Summary("Saves a new fact to the bot's memory.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length-1; i++)
            {
                if (i == 0 && args[i].Equals("that"))
                    continue;

                input += args[i] + " ";
            }
            input += args[args.Length - 1];

            var lowerInput = input.ToLower();
            if (lowerInput.Contains(Context.User.Username.ToLower()) || lowerInput.Contains(""+Context.User.Id))
            {
                var emb = new EmbedBuilder();
                emb.Title = "Error!";
                emb.WithDescription("You cannot teach me facts about yourself! :^)");
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
                return;
            }

            var list = Program.Instance.facts.GetList(Context.Guild.Id);
            list.Add(lowerInput);
            Program.Instance.facts.WriteToGuild(Context.Guild.Id, input.ToLower());

            var emb2 = new EmbedBuilder();
            emb2.Title = "Fact Learned!";
            emb2.WithDescription(string.Format("I've learned fact #{0}: {1}", list.Count, lowerInput));
            emb2.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb2);
        }
    }

    public class FactsCommand : ModuleBase
    {
        [Command("facts"), Summary("Outputs number of facts known.")]
        public async Task Say()
        {
            var list = Program.Instance.facts.GetList(Context.Guild.Id);

            var emb = new EmbedBuilder();
            emb.Title = "Facts Count";
            emb.WithDescription(string.Format("I know {0} facts.", list.Count));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }

    public class FactsListCommand : ModuleBase
    {
        [Command("facts list"), Summary("Outputs number of facts known.")]
        public async Task Say()
        {
            var list = Program.Instance.facts.GetList(Context.Guild.Id);
            string input = "";
            foreach (string fact in list)
            {
                input += fact + "\n";
            }
            string url = Helpers.UploadHastebin(input);

            var emb = new EmbedBuilder();
            emb.Title = "Facts List Fetched!";
            emb.WithDescription(string.Format("List of facts fetched: {0}", url));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }


    public class FactCommand : ModuleBase
    {
        [Command("fact"), Summary("Outputs a random fact")]
        public async Task Say()
        {
            Random rand = new Random();
            var list = Program.Instance.facts.GetList(Context.Guild.Id);
            int index = rand.Next(0, list.Count - 1);

            var emb = new EmbedBuilder();
            emb.Title = "Fact Fetched!";
            emb.WithDescription(string.Format("Displaying fact #{0}: {1}", (index + 1), list[index]));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }


    public class FactsAboutCommand : ModuleBase
    {
        [Command("facts about"), Summary("Outputs a fact about the input, if learned.")]
        public async Task Say(string input)
        {
            var list = Program.Instance.facts.GetList(Context.Guild.Id);
            string lower = input.ToLower();
            var sublist = list.Where(x => x.Contains(lower));

            if (sublist.Count() == 0)
            {
                var emb2 = new EmbedBuilder();
                emb2.Title = "Error!";
                emb2.WithDescription("I don't know anything about " + lower);
                emb2.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb2);
                return;
            }

            int index = new Random().Next(0, sublist.Count());
            string random = sublist.ElementAt(index);

            var emb = new EmbedBuilder();
            emb.Title = "Fact Found!";
            emb.WithDescription(string.Format("Displaying fact #{0}: {1}", list.IndexOf(random) + 1, random));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }

    public class Forget : ModuleBase
    {
        [Command("forget"), Summary("Forgets a fact learned.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (i == 0 && args[i].Equals("that"))
                    continue;

                input += args[i] + " ";
            }
            input += args[args.Length - 1];

            var lower = input.ToLower();
            int count = await Program.Instance.facts.RemoveFromGuild(Context.Guild.Id, lower);
            if (count == 0)
            {
                var emb = new EmbedBuilder();
                emb.Title = "Error!";
                emb.WithDescription(string.Format("I don't know anything about {0}", lower));
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
            else
            {
                var emb = new EmbedBuilder();
                emb.Title = "Fact Forgotten!";
                emb.WithDescription(string.Format("I have forgotten that {0}", lower));
                emb.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, emb);
            }
        }
    }
}