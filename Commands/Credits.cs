using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Octokit;


namespace SteamDiscordBot.Commands
{
    public class CreditsCommand : ModuleBase
    {
        [Command("credits"), Summary("Outputs the credits for the bot, ordered by contributions")]
        public async Task Say()
        {
            string owner;
            string repo;

            owner = Program.config.GitHubUpdateUsername;
            repo = Program.config.GitHubUpdateRepository;
            var contribs = await Program.Instance.ghClient.Repository.GetAllContributors(owner, repo);


            List<RepositoryContributor> creators = new List<RepositoryContributor>(contribs);
            creators.Sort(delegate (RepositoryContributor a, RepositoryContributor b)
            {
                if (a.Contributions == b.Contributions) return 0;
                return (a.Contributions < b.Contributions) ? 1 : -1; // greatest to least
            });

            string output = "";
            foreach (var usr in creators)
            {
                output += string.Format("**{0}** ({1} contribution(s))\n", usr.Login, usr.Contributions);
            }

            var emb = new EmbedBuilder();
            emb.Title = "Credits ordered by contribution count:";
            emb.WithDescription(output);
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}
