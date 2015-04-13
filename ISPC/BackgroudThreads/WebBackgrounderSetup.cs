using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBackgrounder;
using log4net;
using System.Web.Caching;
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(ISPC.BackgroundThreads.WebBackgrounderSetup), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(ISPC.BackgroundThreads.WebBackgrounderSetup), "Shutdown")]
namespace ISPC.BackgroundThreads
{
    public static class WebBackgrounderSetup
    {
        static readonly JobManager _jobManager = CreateJobWorkersManager();

        public static void Start()
        {
            _jobManager.Start();
        }

        public static void Shutdown()
        {
            _jobManager.Dispose();
        }

        private static JobManager CreateJobWorkersManager()
        {
            var jobs = new IJob[]
        {         
            //new InputSimulateData(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(10)),
        };

            var coordinator = new SingleServerJobCoordinator();
            var manager = new JobManager(jobs, coordinator);
            manager.Fail(ex => (LogManager.GetLogger("MyLogger")).Error("ISPC RealTimeData refresh Blow Up.", ex));
            return manager;
        }
    }
}