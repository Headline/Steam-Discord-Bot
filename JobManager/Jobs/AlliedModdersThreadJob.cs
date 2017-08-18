using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net;

using System.Xml.Linq;
using System;
using Discord;

namespace ChancyBot.Jobs
{
	public class AlliedModdersThreadJob : Job
	{
        List<ThreadInfo> history;
		string url;
		string target;

		public AlliedModdersThreadJob(string url, string target)
		{
            this.history = new List<ThreadInfo>();
			this.target = target;
			this.url = url;
		}

		public override void OnRun()
		{
            try
            {
                string xml = new WebClient().DownloadString(this.url);
                XDocument doc = XDocument.Parse(xml);
                XNamespace ns = "http://www.w3.org/2005/Atom/";

                var mainfeed = doc.Descendants().ElementAt(0).Descendants().Elements();

                var feed = mainfeed.Where(x => x.Name.ToString().Contains("item")).First().Elements();

                ThreadInfo current = new ThreadInfo(feed);

                if (history.Count == 0) // first fetch
                {
                    history.Add(current);
                }
                else
                {
                    if (!history.Exists(x => x.Equals(current)))
                    {
                        Task.Run(() => Helpers.SendMessageAllToTarget(target, "New AlliedModders plugin: " + current.title + "\n"
                            + current.link));
                        history.Add(current);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Error, "GithubUpdateJob", ex.Message));
            }
		}
	}

    class ThreadInfo
    {
        public string title;
        public string link;
        public string description;

        public ThreadInfo(IEnumerable<XElement> feed)
        {
            this.title = feed.Where(x => x.Name.ToString().Contains("title")).First().Value.Trim();
            this.link = feed.Where(x => x.Name.ToString().Contains("link")).First().Value.Trim();
            this.description = feed.Where(x => x.Name.ToString().Contains("description")).First().Value.Trim();
        }

        public override string ToString()
        {
            return string.Format("{0} by {1}", this.title, this.description);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ThreadInfo compObj = obj as ThreadInfo;
            if (compObj == null) return false;
            else return Equals(compObj);
        }

        public bool Equals(ThreadInfo commit2)
        {
            return commit2.link.Equals(this.link);
        }
    }
}
