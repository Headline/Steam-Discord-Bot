using System;
using System.Net;

using ChancyBot;
using SteamKit2;
using Discord;

namespace ChancyBot.Jobs
{
	public class UpdateJob : Job
	{
		private uint version;
		private bool hasLastVersion;
		private uint appid;

		public UpdateJob(uint appid)
		{
			this.appid = appid;

			this.hasLastVersion = false;
		}

		public override void OnRun()
		{
			uint lastVersion = 0;

			if (hasLastVersion)
			{
				lastVersion = this.version;
			}

			using (dynamic steamApps = WebAPI.GetInterface("ISteamApps"))
			{
				steamApps.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

				KeyValue results = null;

				try
				{
					results = steamApps.UpToDateCheck(appid: appid, version: lastVersion);
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

				if (!results["up_to_date"].AsBoolean())
				{
					if (this.hasLastVersion)
					{
						// if we previously cached the version, display that it changed
						Program.Instance.Log(new LogMessage(LogSeverity.Info, "UpdateCheck", string.Format("{0} (version: {1}) is no longer up to date. New version: {2}", Helpers.GetAppName(appid), lastVersion, requiredVersion)));
						Helpers.SendMessageAllToGenerals(string.Format("{0} (version: {1}) is no longer up to date. New version: {2} \nLearn more: {3}", Helpers.GetAppName(appid), lastVersion, requiredVersion, ("https://steamdb.info/patchnotes/?appid=" + appid)));
					}

					// update our cache of required version
					this.version = requiredVersion;
					this.hasLastVersion = true;
				}
			}
		}
	}
}
