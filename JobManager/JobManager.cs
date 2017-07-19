﻿using Discord;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;

namespace ChancyBot.Jobs
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

        // constructor
        public JobManager(int seconds)
        {
            this.jobs = new List<Job>();
            timer = new Timer(seconds * 1000);
            timer.Elapsed += RunJubs;
        }

        // thread method
        private void RunJubs(object source, ElapsedEventArgs e)
        {
            foreach (Job job in jobs)
            {
                job.OnRun();
            }
        }
    }
}