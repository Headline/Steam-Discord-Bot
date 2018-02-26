﻿using Discord;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace SteamDiscordBot.Jobs
{
	public abstract class Job
	{
		public abstract void OnRun();
	}

    public class JobManager
    {
        // instance variables
        private Timer timer;
        private List<Job> jobs;

        // getters & setters
        public void AddJob(Job job)
        {
            jobs.Add(job);
        }

        public void StartJobs()
        {
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        public void KillJobs()
        {
            timer.Stop();
            timer.Dispose();
        }

        // constructor
        public JobManager(int seconds)
        {
            this.jobs = new List<Job>();
            timer = new Timer(seconds * 1000);
            timer.Elapsed += RunJobs;
        }


        // thread method
        private void RunJobs(object source, ElapsedEventArgs e)
        {
            Program.Instance.Log(new LogMessage(LogSeverity.Error, "RunJobs", "Running jobs..."));
            foreach (Job job in jobs)
            {
                Task.Run(() => job.OnRun());
            }
        }
    }
}