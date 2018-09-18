using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    public class NotificationStackModel
    {
        public NotificationStackModel()
        {
            NotificationId = new Guid();
        }

        public NotificationStackModel(NotificationStack model)
        {
            NotificationId = model.NotificationId;
            UserId = model.UserId ?? 0;
            Status = (NotificationStatus)model.Status;
            Priority = (Priority)model.Priority;
            DeviceId = model.DeviceId;
            StringifyMessage = model.Message;
        }

        public Guid NotificationId { get; set; }
        public int UserId { get; set; }
        public NotificationStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string DeviceId { get; set; }
        public IGCMResponseModel Message { get; set; }
        public string StringifyMessage { get; set; }
    }
    public class UnreadNotificationModel
    {
        public int UserId { get; set; }
        public int unReadNotifications { get; set; }
    }
    public class ExpertNotificationModel
    {
        public int log { get; set; }
        public int request { get; set; }
        public int total { get; set; }
    }
}