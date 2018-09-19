
//System
using System;
using System.Threading.Tasks;

//Discord 
using Discord.Commands;

//Google 
using static Google.Apis.Customsearch.v1.CseResource;
using static Google.Apis.Customsearch.v1.CseResource.ListRequest;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using Discord;

namespace SteamDiscordBot.Commands
{
    public class Search : ModuleBase
    {
        static SafeEnum SafeSearch;
        static int Search_Number;
        static readonly string[] FLAGS = new string[] { "-safesearch_active", "-safesearch_off", "-num"};
        
        [Command("search"), Summary("Peforms google search and returns result(s)")]
        public async Task Say(params string[] args)
        {
            try
            {
                CustomsearchService service = new CustomsearchService(new BaseClientService.Initializer { ApiKey = Program.config.GoogleApiKey });
                SafeSearch = Program.config.GoogleSafeSearchActive ? SafeEnum.Active : SafeEnum.Off; //Default value of safesearch grabbed from settings.json
                Search_Number = 1; //Default value number of results to return
                string output = "";

                var search = HandleSearchRequest(service, args);
                output = FormatResultOutput(output, search);
                if (output == "\n") { throw new ArgumentException(); } //Thrown when query input returned no result(s)

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

        static string FormatResultOutput(string output, Google.Apis.Customsearch.v1.Data.Search search)
        {
            for (int i = 0; i < Search_Number; i++)
            {
                output += search.Items?[i].Link.ToString() + "\n";
            }
            return output;
        }

        static Google.Apis.Customsearch.v1.Data.Search HandleSearchRequest(CustomsearchService service, string[] args)
        {
            string query_input = PrepareInput(args);
            ListRequest listRequest = service.Cse.List(query_input);
            listRequest.Cx = Program.config.GoogleSearchEngineID;
            listRequest.Safe = SafeSearch;
            Google.Apis.Customsearch.v1.Data.Search search = listRequest.Execute();
            return search;
        }

        static string PrepareInput(string[] args)
        {
            string input = "";
            string temp = "";
            for (int i = 0; i < args.Length; i++)
            {
                temp = args[i].ToLower();
                if(temp[0] == '-') 
                {
                    HandleSearchFlag(temp);
                    continue;
                }
                input += args[i] + " ";
            }
            return input;
        }

        static void HandleSearchFlag(string input)
        {
            switch (input)
            {
                case "-safesearch_active":
                    SafeSearch = SafeEnum.Active;
                    break;
                case "-safesearch_off":
                    SafeSearch = SafeEnum.Off;
                    break;
                default:
                    if (input.Contains(FLAGS[2]))
                    {
                        if (int.TryParse(input.Split('_')[1], out int number) && (number <= Program.config.GoogleMaxSearchNumber) && (number >= 1))
                        {
                            Search_Number = number;
                            return;
                        }
                    }
                    break; 
            }
        }
    }
}
