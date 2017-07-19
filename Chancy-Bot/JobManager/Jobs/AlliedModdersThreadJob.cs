using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;

using ChancyBot;
using SteamKit2;
using Discord;

using ChancyBot.Steam;
using System.Xml.Linq;

namespace ChancyBot.Jobs
{
	public class AlliedModdersThreadJob : Job
	{
        ThreadInfo commit;
		string url;
		string target;
		public AlliedModdersThreadJob(string url, string target)
		{
			this.target = target;
			this.url = url;
		}

		public override void OnRun()
		{
			string xml = new WebClient().DownloadString(this.url);
			XDocument doc = XDocument.Parse(xml);
			XNamespace ns = "http://www.w3.org/2005/Atom/";

            var mainfeed = doc.Descendants().ElementAt(0).Descendants().Elements();

            var feed = mainfeed.Where(x => x.Name.ToString().Contains("item")).First().Elements();

			ThreadInfo current = new ThreadInfo(feed);

			if (commit == null) // first fetch
			{
				commit = current;
			}
			else
			{
				if (!commit.Equals(current))
				{
                    Helpers.SendMessageAllToTarget(target, "New AlliedModders plugin: " + current.title + "\n"
						+ current.link);
					commit = current;
				}
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
            if (obj == null || !(obj is ThreadInfo))
            {
                return false;
            }
            else
            {
                ThreadInfo commit2 = (ThreadInfo)obj;
                return (commit2.title.Equals(this.title)
                    && commit2.link.Equals(this.link)
                    && commit2.description.Equals(this.description));
            }
        }
    }
}