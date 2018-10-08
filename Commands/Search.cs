using System;
using System.Threading.Tasks;

using Discord.Commands;

using static Google.Apis.Customsearch.v1.CseResource;
using static Google.Apis.Customsearch.v1.CseResource.ListRequest;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;

namespace SteamDiscordBot.Commands
{
    public class Search : ModuleBase
    {
        private SafeEnum SafeSearch;
        private int SearchNumber;
        private readonly string[] flags = new string[] { "-safesearch_active", "-safesearch_off", "-num"};
        
        [Command("search"), Summary("Peforms google search and returns result(s)")]
        public async Task Say(params string[] args)
        {
            try
            {
                bool result = await CheckGoogleConfig();
                if (!result) {
                    return; // Google Config is not valid/supplied 
                }

                CustomsearchService service = new CustomsearchService(new BaseClientService.Initializer { ApiKey = Program.config.GoogleApiKey });
                SafeSearch = GetSafeSearchVal(); //Tries to grab from json settings or uses hardcoded value
                SearchNumber = 1; //Default value number of results to return
                string output = "";

                var search = HandleSearchRequest(service, args);
                output = FormatResultOutput(search, output);
                if (output.Trim().Length == 0) {
                    await Context.Channel.SendMessageAsync("No Search Results Found.");
                    return;
                }

                await Context.Channel.SendMessageAsync(output);
            }

            catch (Exception e)
            {
                await Program.Instance.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "SearchCMD", e.Message));
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        private Google.Apis.Customsearch.v1.Data.Search HandleSearchRequest(CustomsearchService service, string[] args)
        {
            string query_input = PrepareInput(args);
            ListRequest listRequest = service.Cse.List(query_input);
            listRequest.Cx = Program.config.GoogleSearchEngineID;
            listRequest.Safe = this.SafeSearch;
            Google.Apis.Customsearch.v1.Data.Search search = listRequest.Execute();
            return search;
        }

        private string PrepareInput(string[] args)
        {
            string input = "";
            string temp = "";
            for (int i = 0; i < args.Length; i++)
            {
                temp = args[i].ToLower();
                if (temp[0] == '-')
                {
                    HandleSearchFlag(temp);
                    continue;
                }
                input += args[i] + " ";
            }
            return input;
        }

        private void HandleSearchFlag(string input)
        {
            int max = GetMaxSearchNumber();
            switch (input)
            {
                case "-safesearch_active":
                    this.SafeSearch = SafeEnum.Active;
                    break;
                case "-safesearch_off":
                    this.SafeSearch = SafeEnum.Off;
                    break;
                default:
                    if (input.Contains(this.flags[2]))
                    {
                        if (int.TryParse(input.Split('_')[1], out int number) && (number <= max) && (number >= 1))
                        {
                            this.SearchNumber = number;
                            return;
                        }
                    }
                    break;
            }
        }

        private string FormatResultOutput(Google.Apis.Customsearch.v1.Data.Search search, string output)
        {
            for (int i = 0; i < this.SearchNumber; i++)
            {
                output += search.Items?[i].Link.ToString() + "\n";
            }
            return output;
        }
      
        private async Task<bool> CheckGoogleConfig()
        {
            if (!Program.HasMember(Program.config, "GoogleApiKey") || !Program.HasMember(Program.config, "GoogleSearchEngineID"))
            {
                await Context.Channel.SendMessageAsync("This command must have a valid Google API key! Please contact the owner.");
                return false;
            }
            return true;
        }

        private SafeEnum GetSafeSearchVal()
        {
            if (Program.HasMember(Program.config, "GoogleSafeSearchActive"))
            {
                return Program.config.GoogleSafeSearchActive ? SafeEnum.Active : SafeEnum.Off;
            }

            return SafeEnum.Off; //Default Value    
        }

        private int GetMaxSearchNumber()
        {
            if (Program.HasMember(Program.config, "GoogleMaxSearchNumber"))
            {
                return Program.config.GoogleMaxSearchNumber;
            }

            return 10; //Default Value   
        }
    }
}
