using Discord;
using System;
using System.Diagnostics;
using System.Linq;

using System.Net;
using System.Xml.Linq;
using Octokit;
using System.IO;
using System.Threading;

namespace ChancyBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        GitHubClient client;
        CommitInfo commit;
        string url;

        public SelfUpdateListener(GitHubClient client, string url)
        {
            this.client = client;
            this.url = url;
        }

        public string GetRepo()
        {
            string[] split = url.Split('/');

            return split[3] + "/" + split[4];
        }

        public override async void OnRun()
        {
            try
            {
                string xml = new WebClient().DownloadString(this.url);
                XDocument doc = XDocument.Parse(xml);
                XNamespace ns = "http://www.w3.org/2005/Atom/";

                var feed = doc.Descendants().ElementAt(0).Descendants().Elements();

                CommitInfo current = new CommitInfo(feed);

                if (commit == null) // first fetch
                {
                    commit = current;
                }
                else
                {
                    if (!commit.Equals(current))
                    {
                        int pid = Process.GetCurrentProcess().Id;

						var commits = await client.Repository.Commit.GetAll("Headline22", "Steam-Discord-Bot");
						
                        if (commits[0].Commit.Message.Contains("[skip update]")) // don't force updates, continue normal operation
                        {
                            commit = current;
                            return;
                        }
                        
                        var releases = await client.Repository.Release.GetAll("Headline22", "Steam-Discord-Bot");

                        string url = "https://github.com/Headline22/Steam-Discord-Bot/releases/download/<name>/steam-discord-bot.zip";
                        string name = NextVersion(releases[0].Name);
                        url = url.Replace("<name>", name);

                        string command = string.Format("/k cd {0} & python updater.py {1} {2}",
                                                                        Directory.GetCurrentDirectory(),
                                                                        pid,
                                                                        url);

                        Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", "Update detected. Killing jobs..."));
                        Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", "URL: "));
                        Program.Instance.manager.KillJobs();
                        Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", "Waiting for build completion..."));

                        Thread.Sleep(180 * 1000); // wait 3 minute for build to complete.
                        Process.Start("CMD.exe", command);
                        Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", "Executing updater & exiting..."));

                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", ex.Message));
            }

        }

        public static string NextVersion(string name)
        {
            string[] peices;
            peices = name.Split('.');

            int.TryParse(peices[name.Length - 1], out int buildVersion);
            buildVersion++;

            string output = "";
            for (int i = 0; i < peices.Length-1; i++) // loop to one before end
            {
                output += peices[i] + ".";
            }
            output += buildVersion;

            return output;
        }
    }
}