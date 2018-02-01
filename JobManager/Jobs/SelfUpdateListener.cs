using Octokit;

namespace ChancyBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        GitHubClient client;

        public SelfUpdateListener(GitHubClient client)
        {
            this.client = client;
        }

        public override async void OnRun()
        {
            var commits = await client.Repository.Commit.GetAll("Headline", "Steam-Discord-Bot");
            var releases = await client.Repository.Release.GetAll("Headline", "Steam-Discord-Bot");

            var latest = Helpers.GetLatestVersion(releases);

            if (!Program.VERSION.Equals(latest))
            {
                Helpers.Update();
            }
        }
    }
}