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
		[Command("roll"), Summary("rolls a dice of arbitrary size.")]
		public async Task Say(string arg)
		{
			Random rand = new Random();
			int num = 0;
			if( arg.Equals("d6") )
				num = rand.Next(1, 6);
			else if( arg.Equals("d10") )
				num = rand.Next(1, 10);
			else if( arg.Equals("d20") )
				num = rand.Next(1, 20);
			else if( arg.Equals("d50") )
				num = rand.Next(1, 50);
			else if( arg.Equals("d100") )
				num = rand.Next(1, 100);
			
			await Context.Channel.SendMessageAsync("you rolled a "+num);
		}
	}
}
