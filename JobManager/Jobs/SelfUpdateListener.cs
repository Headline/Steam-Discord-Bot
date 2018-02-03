using Octokit;

namespace ChancyBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        public override void OnRun()
        {
            var latest = Helpers.GetLatestVersion(Program.Instance.ghReleases);
            if (!Program.VERSION.Equals(latest))
            {
                Helpers.Update();
            }
        }
    }
}