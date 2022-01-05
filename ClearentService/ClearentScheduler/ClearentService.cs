using NLog;
using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace ClearentScheduler
{
    public class ClearentService
    {
        private IScheduler scheduler;
        private static readonly string pollInterval = ConfigurationManager.AppSettings["polling-interval-minute"];
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Start() {
            logger.Info("Service is being stared.");
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ClearentPollingJob>().Build();
            int intervalInt;
            if (string.IsNullOrEmpty(pollInterval) || !int.TryParse(pollInterval, out intervalInt)) {
                intervalInt = 60;
            }

            ITrigger trigger = TriggerBuilder.Create()
                   .WithSimpleSchedule(a => a.WithIntervalInMinutes(intervalInt).RepeatForever())
                   .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public void Stop()
        {
            logger.Info("Service is being stopped.");
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Shutdown();
        }
    }
}
