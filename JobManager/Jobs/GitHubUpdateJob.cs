using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChancyBot.Jobs
{
    public class GithubUpdateJob : Job
    {
        CommitInfo commit;
        string url;
        string target;

        public GithubUpdateJob(string url, string target)
        {
            this.target = target;
            this.url = url;
        }

        public string GetRepo()
        {
            string[] split = url.Split('/');

            return split[3] + "/" + split[4];
        }

        public override void OnRun()
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
                        Task.Run(() => Helpers.SendMessageAllToTarget(target, "New GitHub Update on: " + this.GetRepo() + "\n"
                            + current.ToString() + "\n"
                            + current.url));
                        commit = current;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Error, "GithubUpdateJob", ex.Message));
            }
        }
    }

    class CommitInfo
    {
        public string lastUpdated;
        public string title;
        public string author;
        public string url;

        public CommitInfo(IEnumerable<XElement> feed)
        {
            this.lastUpdated = feed.Where(x => x.Name.ToString().Contains("updated")).First().Value.Trim();
            this.title = feed.Where(x => x.Name.ToString().Contains("title")).First().Value.Trim();
            this.author = feed.Where(x => x.Name.ToString().Contains("author")).First().Elements().Where(x => x.Name.ToString().Contains("name")).First().Value.Trim();
            this.url = feed.Where(x => x.Name.ToString().Contains("link")).First().Attribute("href").Value.Trim();
        }

        public override string ToString()
        {
            return string.Format("{0} by {1}", this.title, this.author);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CommitInfo))
            {
                return false;
            }
            else
            {
                CommitInfo commit = (CommitInfo)obj;
                return (commit.lastUpdated.Equals(this.lastUpdated)
                     && commit.title.Equals(this.title)
                     && commit.author.Equals(this.author));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}