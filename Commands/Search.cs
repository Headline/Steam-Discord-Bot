
//System
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//Discord 
using Discord.Commands;

//Google 
using static Google.Apis.Customsearch.v1.CseResource;
using static Google.Apis.Customsearch.v1.CseResource.ListRequest;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;

namespace SteamDiscordBot.Commands
{
    public class Search : ModuleBase
    {
        public static readonly int DEFAULT_SEARCH_NUMBER = 1;
        public static readonly string[] FLAGS = new string[] { "-safesearch", "-num"};

        public static CustomsearchService service = new CustomsearchService(new BaseClientService.Initializer { ApiKey = API_KEY });

        //bitfield
        public enum GOOGLEFLAGS : uint
        {
            SAFESEARCH = (1<<0),
            NUM = (1<<1)
        }

        SafeEnum

        [Command("search"), Summary("Peforms google search and returns result(s)")]
        public async Task Say(params string[] args)
        {
            GOOGLEFLAGS flags = 0;
            // safe search

            flags |= GOOGLEFLAGS.SAFESEARCH;
            if ((flags & GOOGLEFLAGS.SAFESEARCH) == GOOGLEFLAGS.SAFESEARCH)
            {
            }

            FindFlags(args);

            args = RemoveArguementFlags(args, '-');
            if (args.Length == 0) { return; }
            string input = FormatInput(args);

            try
            {
                string output = "";
                ListRequest listRequest = service.Cse.List(input);
                listRequest.Cx = SEARCH_ENGINE_ID;
                listRequest.Safe = DEFAULT_SAFETY_LEVEL;

                if (Current_Search_Flags.Any()) { HandleSearchFlags(listRequest); }
                var search = listRequest.Execute();

                for (int i = 0; i < Current_Search_Result_Number; i++)
                {
                    output += search.Items?[i].Link.ToString() + "\n";
                }

                ResetForNextQuery();
                if (output == "\n")
                    throw new ArgumentException();
                await Context.Channel.SendMessageAsync(output);
            }

            catch (ArgumentException)
            {
                await Context.Channel.SendMessageAsync("No search results found");
            }
            catch (Exception e)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "SearchCMD", e.Message));
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        public static void FindFlags(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (ValidFlagFound(args[i]))
                {
                    Current_Search_Flags.Add(args[i]); //Adds flag to List to handle later
                }
            }
        }

        public static string FormatInput(string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                input += args[i] + " ";
            }
            input += args[args.Length - 1];
            return input;
        }

        public static SafeEnum GetSearchSafetyLevel(string input)
        {
            input = input.ToLower();
            if (input == "high")
            {
                return ListRequest.SafeEnum.High; // Search level safety: high
            }
            else if (input == "medium")
            {
                return ListRequest.SafeEnum.Medium; // Search level safety: medium
            }
            else if (input == "off")
            {
                return ListRequest.SafeEnum.Off; // Search level safety: off
            }
            else
            {
                return DEFAULT_SAFETY_LEVEL; //Invalid input given, use default value
            }
        }

        public static void HandleSearchFlags(ListRequest listrequest)
        {
            string flag = "";
            string value = "";

            foreach (string item in Current_Search_Flags)
            {
                flag = item.Split('_')[0];
                value = item.Split('_')[1];

                switch (flag)
                {
                    case "-safesearch":
                        listrequest.Safe = GetSearchSafetyLevel(value);
                        break;

                    case "-ss":
                        listrequest.Safe = GetSearchSafetyLevel(value);
                        break;

                    case "-num":
                        int number = -1;
                        if (int.TryParse(value, out number))
                        {
                            if(number <= SEARCH_MAX_COUNT && number >= 1)
                            {
                                Current_Search_Result_Number = number;
                            }
                        }
                        break;

                    default:
                        break;

                }
            }
        }

        public static bool ValidFlagFound(string arg)
        {
            int index = arg.IndexOf('-');
            if (index == -1) { return false; }
            index = arg.IndexOf('_');
            if ((index + 1) > arg.Length) { return false; }

            return true;
        }

        public static string[] RemoveArguementFlags(string[] args, char flag)
        {
            string temp = "";
            string output = "";

            for (int i = 0; i < args.Length; i++)
            {
                temp = args[i];
                if (temp[0] != flag)
                {
                    output += temp + " ";
                }
            }

            return output.Split(' '); //search halo
        }
        public static void ResetForNextQuery()
        {
            Current_Search_Flags.Clear();
            Current_Search_Result_Number = DEFAULT_SEARCH_NUMBER;
        }
    }

}
