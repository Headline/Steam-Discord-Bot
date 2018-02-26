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

			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam connected! Logging in..."));

			Program.Instance.connection.steamUser.LogOn(new SteamUser.LogOnDetails
			{
				Username = Program.Instance.connection.user,
				Password = Program.Instance.connection.pass,
			});
		}

		public static void OnClanState(SteamFriends.ClanStateCallback callback)
		{
			string clanName = callback.ClanName;

			if (string.IsNullOrWhiteSpace(clanName))
				clanName = Program.Instance.connection.Friends.GetClanName(callback.ClanID);

			if (string.IsNullOrWhiteSpace(clanName) || clanName == "[unknown]") // god this sucks. why on earth did i make steamkit follow steamclient to the letter
				clanName = "Group";

			foreach (var announcement in callback.Announcements)
			{
				string announceUrl = string.Format("http://steamcommunity.com/gid/{0}/announcements/detail/{1}", callback.ClanID.ConvertToUInt64(), announcement.ID.Value);

				string message = string.Format("steam-news", "{0} announcement: {1} - {2}", clanName, announcement.Headline, announceUrl);
				Program.Instance.Log(new LogMessage(LogSeverity.Info, "Announcment", message));
				Helpers.SendMessageAllToGenerals(message);
			}

			foreach (var clanEvent in callback.Events)
			{
				if (!clanEvent.JustPosted)
					continue; // we're only interested in recent clan events

				string eventUrl = string.Format("http://steamcommunity.com/gid/{0}/events/{1}", callback.ClanID.ConvertToUInt64(), clanEvent.ID.Value);

				string message = string.Format("steam-news", "{0} event: {1} - {2}", clanName, clanEvent.Headline, eventUrl);
				Program.Instance.Log(new LogMessage(LogSeverity.Info, "Event", message));
				Helpers.SendMessageAllToGenerals(message);
			}
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

			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam successfully logged in!"));
		}

		public static void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam disconnected!"));

			Program.Instance.connection.isRunning = false;
		}

		public static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCB", "Steam logged off!"));

			Program.Instance.connection.isRunning = false;
		}
	}
}
