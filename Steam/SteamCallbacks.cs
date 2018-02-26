﻿using SteamKit2;
using Discord;

namespace SteamDiscordBot.Steam
{
	class SteamCallbacks
    {
		public static void OnConnected(SteamClient.ConnectedCallback callback)
		{
			if (callback.Result != EResult.OK)
			{
				Program.Instance.connection.isRunning = false;
				return;
			}

			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam connected!"));

			Program.Instance.connection.steamUser.LogOn(new SteamUser.LogOnDetails
			{
				Username = Program.Instance.connection.user,
				Password = Program.Instance.connection.pass,
			});
		}

		public static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{
			if (callback.Result != EResult.OK)
			{
				if (callback.Result == EResult.AccountLogonDenied)
				{
					Program.Instance.connection.isRunning = false;
					return;
				}


				Program.Instance.connection.isRunning = false;
				return;
			}
		}

		public static void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam disconnected!"));

			Program.Instance.connection.isRunning = false;
		}

		public static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Program.Instance.connection.isRunning = false;
		}
	}
}
