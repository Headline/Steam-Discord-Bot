﻿using System;

using SteamKit2;
using SteamKit2.Unified.Internal;
using static SteamKit2.SteamUnifiedMessages;
using System.Threading;

namespace SteamDiscordBot.Steam
{
    public class SteamConnection
    {
		// STEAMKIT2
		const uint k_EMsgGCClientGoodbye = 4008;
		const uint k_EMsgGCTFSpecificItemBroadcast = 1096;

		public SteamGameCoordinator SteamGameCoordinator;

		public SteamClient steamClient;
		public CallbackManager manager;
		public SteamUser steamUser;
		public SteamMasterServer masterServer;
		public UnifiedService<IGameServers> GameServers;
		public SteamFriends Friends { get; private set; }

		public string user;
		public string pass;
		public bool isRunning;
		public readonly int DISPLAY_AMOUNT = 5;
        private Thread steamThread;

        public SteamConnection(string user, string pass)
        {
            this.steamClient = new SteamClient();

            this.manager = new CallbackManager(this.steamClient);
            this.Friends = this.steamClient.GetHandler<SteamFriends>();
            this.steamUser = this.steamClient.GetHandler<SteamUser>();
            this.masterServer = this.steamClient.GetHandler<SteamMasterServer>();
            this.GameServers = this.steamClient.GetHandler<SteamUnifiedMessages>().CreateService<IGameServers>();
            this.SteamGameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            this.manager.Subscribe<SteamClient.ConnectedCallback>(SteamCallbacks.OnConnected);
            this.manager.Subscribe<SteamClient.DisconnectedCallback>(SteamCallbacks.OnDisconnected);
            this.manager.Subscribe<SteamUser.LoggedOnCallback>(SteamCallbacks.OnLoggedOn);
            this.manager.Subscribe<SteamUser.LoggedOffCallback>(SteamCallbacks.OnLoggedOff);

            this.user = user;
            this.pass = pass;

            this.isRunning = false;

            this.steamClient.ConnectionTimeout = new TimeSpan(0, 0, 15); // 15 seconds
        }

        public void Connect()
        {
            if (this.isRunning)
            {
                /* We need to kill the thread, and wait for it to die. */
                this.isRunning = false;
                Timer timer = new Timer(Timer_ConnectDelay, null, 100, Timeout.Infinite);
                return;
            }
            /* Otherwise, we can just run the thread */
            RunThread();
        }

        private void Timer_ConnectDelay(object state)
        {
            RunThread();
        }

        private void RunThread()
        {
            this.isRunning = true;

            steamThread = new Thread(new ThreadStart(() =>
            {
                this.steamClient.Connect();

                while (this.isRunning)
                {
                    this.manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                }
            }));

            steamThread.Start();
        }
        public void Kill()
        {
            this.isRunning = false;
        }
    }
}
