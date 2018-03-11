using Octokit;

namespace SteamDiscordBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        public override async void OnRun()
        {
            try
            {
                var releases = await Program.Instance.ghClient.Repository.Release.GetAll(Program.config.GitHubUpdateUsername, Program.config.GitHubUpdateRepository);

                var latest = Helpers.GetLatestVersion(releases);
                if (!Program.VERSION.Equals(latest))
                {
                    Helpers.Update();
                }
            }
            catch { } // quietly fail
        }
    }
}