using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Quartz.Spi;
using Ninject.Syntax;

namespace HyggeMail.Scheduler
{
    public class JobScheduler
    {
        private static IScheduler _scheduler;
        public static IScheduler scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    _scheduler = scheduler;
                }
                return _scheduler;
            }
        }

        public static void Email()
        {
            IJobDetail job = JobBuilder.Create<Job>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("NotificationMail", "HyggeMail")
            .WithCronSchedule("0 59 23 1/16 * ? *")
            .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}