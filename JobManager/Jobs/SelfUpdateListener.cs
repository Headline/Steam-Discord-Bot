using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChancyBot.Jobs
{
    public class SelfUpdateListener : Job
    {
        CommitInfo commit;
        string url;
        string target;

        public SelfUpdateListener(string url, string target)
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
                        int pid = Process.GetCurrentProcess().Id;
                        // TODO Call Python Script With Latest Zip File
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Error, "SelfUpdateListener", ex.Message));
            }
        }
    }
}