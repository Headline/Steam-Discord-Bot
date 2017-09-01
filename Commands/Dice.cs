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
        private Random rand;

        public DiceCommand()
        {
            rand = new Random();
        }

        [Command("roll"), Summary("Rolls a dice of arbitrary size.")]
		public async Task Say(string arg)
		{
            int sideAmmount = ExtractDigits(arg);

            int num = rand.Next(1, sideAmmount);

            await Context.Channel.SendMessageAsync("You rolled a " + num + "!");
        }

        public static int ExtractDigits(string input)
        {
            char[] characters = input.Where(c => Char.IsNumber(c)).ToArray();
            string digits = new string(characters);

            int.TryParse(digits, out int result);

            return result;
        }
	}
}
