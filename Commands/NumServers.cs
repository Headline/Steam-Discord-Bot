using System.Linq;
using System.Threading.Tasks;

using Discord.Commands;

using SteamKit2;
using SteamKit2.Unified.Internal;

namespace ChancyBot.Commands
{
    public class NumServers : ModuleBase
    {
        [Command("numservers"), Summary("Fetches current server count from steam.")]
        public async Task Say([Remainder, Summary("Valve master server query filter")] string filter)
        {
            var request = new CGameServers_GetServerList_Request
            {
                filter = filter,
                limit = 20000, // max allowed
            };

            var callback2 = await Program.Instance.connection.GameServers.SendMessage(api => api.GetServerList(request));
            var response = callback2.GetDeserializedResponse<CGameServers_GetServerList_Response>();
            var servers = response.servers;

            if (!servers.Any())
            {
                await Context.Channel.SendMessageAsync("No servers.");
            }

            if (servers.Count > 0 && servers.Count <= Program.Instance.connection.DISPLAY_AMOUNT)
            {
                foreach (CGameServers_GetServerList_Response.Server server in servers.Take(5))
                {
                    await Context.Channel.SendMessageAsync(string.Format("{0} - {1}/{2} - Map: {3} - AppID: {4} - Version: {5} - Dir: {6} - Tags: {7} - Name: {8} \n\n",
                    new SteamID(server.steamid).Render(true), server.players, server.max_players, server.map, server.appid, server.version, server.gamedir, server.gametype, server.name));
                }
            }
            else if (servers.Count > Program.Instance.connection.DISPLAY_AMOUNT)
            {
                var serv = servers.Take(5).Select(x => string.Format("{0} ({1})", x.addr, x.players));

                await Context.Channel.SendMessageAsync(string.Format("{0}{1}", string.Join(", ", serv), servers.Count > 5 ? string.Format(", and {0}{1} more", servers.Count == 5000 ? ">" : "", servers.Count - 5) : ""));
            }
        }
    }
}
