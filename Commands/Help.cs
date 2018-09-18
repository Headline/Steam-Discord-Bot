using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class HelpCommand : ModuleBase
    {
        [Command("help"), Summary("Prints and formats all commands.")]
        public async Task Say()
        {
            string message = "";
            string[] helplines = BuildHelpLines(Context.Guild.Id);
            foreach(string line in helplines)
            {
                message += line + "\n";
            }

            var emb = new EmbedBuilder();
            emb.Title = "Command List";
            emb.WithDescription(message);
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }

        public static string[] BuildHelpLines(ulong guild)
        {
            List<string> arrayList = new List<string>();

            Assembly asm = Assembly.GetExecutingAssembly(); // Get assembly

            var results = from type in asm.GetTypes()
                          where typeof(ModuleBase).IsAssignableFrom(type)
                          select type; // Grab all types that inherit ModuleBase

            string trigger = Program.Instance.triggerMap[guild];
            foreach (Type t in results) // For each type in results
            {
                /* Grab MethodInfo of the type where the method has the attribute SummaryAttribute */
                MethodInfo info = t.GetMethods().Where(x => x.GetCustomAttributes(typeof(SummaryAttribute), false).Length > 0).First();

                /* Grab summary attribute */
                SummaryAttribute summary = info.GetCustomAttribute(typeof(SummaryAttribute)) as SummaryAttribute;

                /* Grab command attribute */
                CommandAttribute command = info.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;

                /* Both objects are non null, valid, so lets grab the attribute text */
                if (summary != null && command != null)
                {
                    if (!Program.IsCommandDisabled(command.Text))
                        arrayList.Add(trigger + command.Text + " - " + summary.Text);
                }
            }

            return arrayList.ToArray(); // return string[] array
        }
    }
}