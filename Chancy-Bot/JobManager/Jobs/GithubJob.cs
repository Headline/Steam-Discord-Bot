using System;
using System.Net;

using ChancyBot;
using SteamKit2;
using Discord;

using Newtonsoft.Json.Linq;

namespace ChancyBot.Jobs
{
	public class GithubJob : Job
	{
        string lastSHA;
        string url;
        string accountandrepo;

		public GithubJob(string accountandrepo)
		{
            this.accountandrepo = accountandrepo;
            this.url = "https://api.github.com/repos/" +  accountandrepo + "/commits";
            this.lastSHA = "";
        }

		public override void OnRun()
		{
			// GET Game Name
            try
            {
				var json = new WebClient().DownloadString(this.url);
				JObject o = JObject.Parse(json);

				if (!lastSHA.Equals("") && !lastSHA.Equals((string)o[0]["sha"])) // if it is a new commit
				{
					string author, authorId;
					string commiter, commiterId;
					string giturl, gitmessage;
					string message;

					author = (string)o[0]["author"]["login"];
					authorId = (string)o[0]["author"]["id"];

					commiter = (string)o[0]["committer"]["login"];
					commiterId = (string)o[0]["committer"]["id"];

					giturl = (string)o[0]["html_url"];
					gitmessage = (string)o[0]["message"];

					if (!authorId.Equals(commiterId)) // author and committer differ
					{
                        message = string.Format("{0} has committed to {1} with {2}: {3}\n{4}", author, accountandrepo, commiter, gitmessage, giturl);
					}
					else
					{
						message = string.Format("{0} has committed to {1}: {2}\n{3}", author, accountandrepo, gitmessage, giturl);
					}

					Helpers.SendMessageAllToGenerals(message);
				}
            }
            catch (Exception ex)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Warning, "GithubJob", "Unexpected error: " + ex.Message));
			}
		}
	}
}
