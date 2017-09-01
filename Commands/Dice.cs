using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ChancyBot.Commands
{
	public class DiceCommand : ModuleBase
	{
		[Command("roll"), Summary("rolls a dice of arbitrary size. (example: \'!roll 33\' rolls a 33-sided die)")]
		public async Task Say(string argSize)
		{
			Random rand = new Random();
			uint num = 0;
			num = (uint) rand.Next(1, (int) Convert.ToUInt32(argSize, 10));
			await Context.Channel.SendMessageAsync("you rolled a "+num);
		}
	}
}
