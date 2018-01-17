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
	public class CAHShuffleCmd : ModuleBase
	{
		[Command("cahshuffle"), Summary("[Cards Against Humanity] Shuffling Cards...")]
		public async Task Say(string argSize)
		{
			
		}
	}
	public class CAHDealCmd : ModuleBase
	{
		[Command("cahdeal"), Summary("[Cards Against Humanity] Dealing Cards...")]
		public async Task Say(string argSize)
		{
			
		}
	}
}

// http://cah.frustratednerd.com/
// https://www.cardsagainsthumanity.com/pdf/CAH_MainGame.pdf
