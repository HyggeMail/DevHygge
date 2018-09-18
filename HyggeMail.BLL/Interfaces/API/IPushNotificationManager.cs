using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Models;

namespace HyggeMail.BLL.Interfaces
{
   public interface IPushNotificationManager
    {
        void AddNotificationLog(string message, string deviceId, DeviceType deviceType, NotificationType type, NotificationStatus status);
        void AddNotificationLog(List<PushNotificationViewModel> model);
    }
}
