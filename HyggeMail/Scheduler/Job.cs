using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Managers;
using HyggeMail.BLL.Notification;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyggeMail.Scheduler
{
    public class Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IEmailManager _emailManager = new EmailManager();
            _emailManager.SendNotificationEmail();
        }
    }
}