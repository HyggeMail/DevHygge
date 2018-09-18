using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Managers;
using HyggeMail.BLL.Models;
using HyggeMail.DAL;


namespace HyggeMail.BLL.Managers
{
    public class PushNotificationManager : BaseManager, IPushNotificationManager
    {
        /// <summary>
        /// Add log for push notification
        /// </summary>
        /// <param name="message">Messsage of notification</param>
        /// <param name="deviceId">Device id to whom notification sent</param>
        /// <param name="deviceType">Device type to whom notification sent</param>
        /// <param name="type">Type of notification</param>
        /// <param name="status">Status of notification</param>
        void IPushNotificationManager.AddNotificationLog(string message, string deviceId, DeviceType deviceType, NotificationType type, NotificationStatus status)
        {
            Context = new HyggeMailEntities();
            Context.PushNotifications.Add(new
            PushNotification
            {
                CreatedOn = DateTime.Now,
                DeviceId = deviceId,
                DeviceTypeId = Convert.ToInt16(deviceType),
                Message = message,
                StatusId = Convert.ToInt16(status),
                TypeId = Convert.ToInt16(type)
            });
            Context.SaveChanges();
        }

        /// <summary>
        /// Add log for push notification in bulk
        /// </summary>
        /// <param name="model"></param>
        void IPushNotificationManager.AddNotificationLog(List<PushNotificationViewModel> model)
        {
            Context = new HyggeMailEntities();
            model.ForEach(p =>
            {
                var noti = new PushNotification
                {
                    CreatedOn = DateTime.Now,
                    DeviceId = p.DeviceId,
                    DeviceTypeId = Convert.ToInt16(p.DeviceType),
                    Message = p.Message,
                    ErrorMessage = p.ErrorMessage,
                    StatusId = Convert.ToInt16(p.Status),
                    TypeId = Convert.ToInt16(p.Type)
                };
                Context.PushNotifications.Add(noti);
            });
            Context.SaveChanges();
        }


    }
}
