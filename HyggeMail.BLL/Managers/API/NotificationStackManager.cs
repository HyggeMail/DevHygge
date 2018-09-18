using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using HyggeMail.BLL.Notification;

namespace HyggeMail.BLL.Managers
{
    public delegate void NotificationHandler(string token, string json, NotificationType type);

    public interface INotificationStackManager
    {
        ActionOutput<NotificationStackModel> AddOrUpdateStack(NotificationStackModel model);
        bool UpdateStatus(string[] notificationIds, NotificationStatus status);
        bool ProcessNotificationQueue(int userId, string token = null, NotificationHandler method = null);
    }

    public class NotificationStackManager : BaseManager, INotificationStackManager
    {
        public ActionOutput<NotificationStackModel> AddOrUpdateStack(NotificationStackModel model)
        {
            try
            {
                if (model.NotificationId == Guid.Empty)
                {
                    var stackNotification = new NotificationStack();
                    stackNotification.NotificationId = Guid.NewGuid();
                    _UpdateModel(ref stackNotification, model);
                    stackNotification.CreatedDate = DateTime.UtcNow;
                    stackNotification.Message = "";
                    Context.NotificationStacks.Add(stackNotification);
                    Context.SaveChanges();
                    model.Message.NotificationId = stackNotification.NotificationId;
                    model.Message.ReceiverId = model.UserId;
                    stackNotification.Message = model.Message.GetJson();
                    Context.SaveChanges();
                    return new ActionOutput<NotificationStackModel>()
                    {
                        Object = new NotificationStackModel(stackNotification),
                        Status = ActionStatus.Successfull
                    };
                }
                else
                {
                    var stackNotification = Context.NotificationStacks.Find(model.NotificationId);
                    if (stackNotification == null)
                        return new ActionOutput<NotificationStackModel>() { Status = ActionStatus.Error };
                    _UpdateModel(ref stackNotification, model);
                    stackNotification.Message = model.Message.GetJson();
                    Context.SaveChanges();
                    return new ActionOutput<NotificationStackModel>()
                    {
                        Object = new NotificationStackModel(stackNotification),
                        Status = ActionStatus.Successfull
                    };
                }
            }
            catch (DbEntityValidationException e)
            {
                return new ActionOutput<NotificationStackModel>() { Status = ActionStatus.Error };
            }
        }

        public bool UpdateStatus(string[] notificationIds, NotificationStatus status)
        {
            try
            {
                foreach (var i in notificationIds)
                {
                    Guid notificationId;
                    if (Guid.TryParse(i, out notificationId))
                    {
                        var entity = Context.NotificationStacks.FirstOrDefault(x => x.NotificationId == notificationId);
                        if (entity != null)
                        {
                            entity.Status = (int)status;
                        }
                    }
                }

                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ProcessNotificationQueue(int userId, string token = null, NotificationHandler method = null)
        {
            try
            {
                //Process High Priority Notification
                var highPriorityNotification = Context.NotificationStacks.Where(x => x.UserId == userId && x.Status != (int)NotificationStatus.Seen && x.Priority == (int)Priority.High).OrderBy(x => x.Priority).ToList();
                if (highPriorityNotification.Count != 0)
                {
                    using (var manager = new PushNotify().Configure())
                    {
                        foreach (var notificationStack in highPriorityNotification)
                        {
                            manager.NotifyAndroidUser(string.IsNullOrEmpty(token) ? notificationStack.User.DeviceToken : token, notificationStack.Message, NotificationType.MessageAlert);
                        }
                    }
                    return true;
                }

                //Process Low Priority Notification
                var lowPriorityNotification = Context.NotificationStacks.Where(x => x.UserId == userId && x.Status != (int)NotificationStatus.Seen && x.Priority != (int)Priority.High).OrderBy(x => x.Priority).ToList();
                if (lowPriorityNotification.Count != 0)
                {
                    using (var manager = new PushNotify().Configure())
                    {
                        foreach (var notificationStack in lowPriorityNotification)
                        {
                            manager.NotifyAndroidUser(string.IsNullOrEmpty(token) ? notificationStack.User.DeviceToken : token, notificationStack.Message, NotificationType.MessageAlert);
                        }
                    }
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void _UpdateModel(ref NotificationStack stackNotification, NotificationStackModel model)
        {
            stackNotification.UserId = model.UserId;
            stackNotification.Status = (int)model.Status;
            stackNotification.Priority = (int)model.Priority;
            stackNotification.DeviceId = model.DeviceId;
        }
    }
}