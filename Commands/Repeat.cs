using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace ChancyBot.Commands
{
    public class RepeatCommand : ModuleBase
    {
        [Command("repeat"), Summary("Repeats last message X amount of times.")]
        public async Task Say(int count)
        {
            //string[] args = argstring.Split(' ');

            List<MsgInfo> list = Program.Instance.messageHist;
            if (list.Count == 0)
            {
                await Context.Channel.SendMessageAsync("Error! No message to repeat yet...");
                return;
            }

            string message = GetLastMessageFromUser(list, Context.User.Id);
            if (message == null)
            {
                await Context.Channel.SendMessageAsync("Error! No message to repeat yet...");
                return;
            }

            string outMsg = "";
            for (int i = 0; i < count; i++)
            {
                outMsg += message + "\n";
            }

            if (outMsg.Length > 1999)
            {
                await Context.Channel.SendMessageAsync("Error! Message content too long. Try a lower repeat amount");
                return;
            }

            await Context.Channel.SendMessageAsync(outMsg);
        }

        private string GetLastMessageFromUser(List<MsgInfo> list, ulong userid)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].user == userid)
                {
                    return list[i].message;
                }
            }

            return null;
        }
    }
}