using System;
using System.Net;

using SteamKit2;
using Discord;
using System.Threading.Tasks;

namespace ChancyBot.Jobs
{
	public class UpdateJob : Job
	{
		private uint version;
		private uint appid;

		public UpdateJob(uint appid)
		{
            this.version = 0;
			this.appid = appid;
		}

		public async override void OnRun()
		{
			using (dynamic steamApps = WebAPI.GetInterface("ISteamApps"))
			{
				steamApps.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

				KeyValue results = null;

				try
				{
					results = steamApps.UpToDateCheck(appid: appid, version: this.version);
				}
				catch (WebException ex)
				{
					if (ex.Status != WebExceptionStatus.Timeout)
					{
						//Log.WriteWarn("UpToDateJob", "Unable to make UpToDateCheck request: {0}", ex.Message);
					}

					return;
				}

				if (!results["success"].AsBoolean())
					return; // no useful result from the api, or app isn't configured

				uint requiredVersion = (uint)results["required_version"].AsInteger(-1);

				if ((int)requiredVersion == -1)
					return; // some apps are incorrectly configured and don't report a required version

				if (this.version != requiredVersion && this.version != 0)
				{
					// if we previously cached the version, display that it changed
					await Program.Instance.Log(new LogMessage(LogSeverity.Info, "UpdateCheck", string.Format("{0} (version: {1}) is no longer up to date. New version: {2}", Helpers.GetAppName(appid), this.version, requiredVersion)));
					await Task.Run(() => Helpers.SendMessageAllToGenerals(string.Format("{0} (version: {1}) is no longer up to date. New version: {2} \nLearn more: {3}", Helpers.GetAppName(appid), this.version, requiredVersion, ("https://steamdb.info/patchnotes/?appid=" + appid))));
                    this.version = 0;
                }
                else
                {
                    this.version = requiredVersion;
                }
            }
        }
	}
}
