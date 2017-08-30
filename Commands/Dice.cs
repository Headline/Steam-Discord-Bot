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
	public class HelpCommand : ModuleBase
	{
		[Command("roll"), Summary("rolls a dice of arbitrary size.")]
		public async Task Say(string arg)
		{
			Random rand = new Random();
			int num = 0;
			if( arg == "d6" )
				num = rand.Next(1, 6);
			else if( arg == "d10" )
				num = rand.Next(1, 10);
			else if( arg == "d20" )
				num = rand.Next(1, 20);
			else if( arg == "d50" )
				num = rand.Next(1, 50);
			else if( arg == "d100" )
				num = rand.Next(1, 100);
			
			await Context.Channel.SendMessageAsync("you rolled a "+num);
		}
	}
}
