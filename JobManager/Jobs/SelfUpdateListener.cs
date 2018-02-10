using Octokit;

namespace ChancyBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        public override async void OnRun()
        {
            var releases = await Program.Instance.ghClient.Repository.Release.GetAll("Headline", "Steam-Discord-Bot");

            try
            {
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