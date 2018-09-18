using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Managers;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Managers;

namespace HyggeMail.Framework.Notifications
{
    public class PushNotifier
    {
        private static PushBroker _pushBroker;
        private static string APNSCerticateFile = "~/Resources/HyggeMail_DisP12.p12";
        private static string APNSCerticateFilePassword = "HyggeMail";

        public static IPushNotificationManager pushNotificationService
        {
            get;
            set;
        }

        static PushNotifier()
        {
            pushNotificationService = new PushNotificationManager();
        }

        public static PushBroker PushBroker
        {
            get
            {
                if (_pushBroker == null)
                {
                    _pushBroker = new PushBroker();
                    //var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/lotteryAppLive.p12"));

                    _pushBroker.OnNotificationSent += NotificationSent;
                    _pushBroker.OnChannelException += ChannelException;
                    _pushBroker.OnServiceException += ServiceException;
                    _pushBroker.OnNotificationFailed += NotificationFailed;
                    _pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                    _pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                    _pushBroker.OnChannelCreated += ChannelCreated;
                    _pushBroker.OnChannelDestroyed += ChannelDestroyed;
                    var settings = new PushServiceSettings();
                    settings.MaxNotificationRequeues = 100;
                    settings.MinAvgTimeToScaleChannels = 100;
                    settings.NotificationSendTimeout = 15000;
                    settings.IdleTimeout = TimeSpan.FromMinutes(5);
                    settings.AutoScaleChannels = true;
                    //_pushBroker.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, "lottery"));
                    var appleCert = File.ReadAllBytes(HostingEnvironment.MapPath(APNSCerticateFile));

                    //  var ele = new GcmPushChannelSettings(Config.BrowserKey);
                    _pushBroker.RegisterGcmService(new GcmPushChannelSettings(Config.BrowserKey), settings);
                    _pushBroker.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, APNSCerticateFilePassword, true));

                    //Wire up the events for all the services that the broker registers
                }
                return _pushBroker;
            }
        }

        public static void Stop()
        {
            PushBroker.StopAllServices();
        }

        public static void NotifyAndroidUser(string token, string json, NotificationType type)
        {
            //Fluent construction of an Android GCM Notification
            //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
            var noti = new GcmNotification();
            //noti.DryRun = true;
           // PushBroker.QueueNotification(noti.ForDeviceRegistrationId(token).WithJson("{\"alert\":\"" + "Testing Testing" + "\",\"badge\":6, \"type\":\"" + type.ToString() + "\"}"));
           PushBroker.QueueNotification(noti.ForDeviceRegistrationId(token).WithJson(json));
        }

        public static void NotifyAndroidUserUsers(List<string> deviceIds, string message, NotificationType type)
        {
            //Fluent construction of an Android GCM Notification
            //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
            var noti = new GcmNotification();
            noti.RegistrationIds = deviceIds;
            PushBroker.QueueNotification(noti.WithJson("{\"alert\":\"" + message + "\",\"badge\":6, \"type\":\"" + type.ToString() + "\"}"));
        }

        public static void NotifyIOSUser(string token, string message, NotificationType type)
        {
            /// /Fluent construction of an iOS notification
            ////IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
            ////  for registered for remote notifications is called, and the device token is passed back to you
            //------------------------- // APPLE NOTIFICATIONS //-------------------------
            //Configure and start Apple APNS // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to 
            //generate one for connecting to Sandbox, and one for connecting to Production.  You must 
            // use the right one, to match the provisioning profile you build your 
            //   app with! 
            var appleCert = File.ReadAllBytes(HostingEnvironment.MapPath(APNSCerticateFile));
            //IMPORTANT: If you are using a Development provisioning Profile, you must use 
            // the Sandbox push notification server  
            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 
            // 'false') 
            //  If you are using an AdHoc or AppStore provisioning profile, you must use the  
            //Production push notification server 
            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to  
            //'true') 

            var payLoad = new AppleNotificationPayload();

            payLoad.AddCustom("NotificationType", NotificationType.MessageAlert);
            payLoad.AddCustom("Badge", 7);
            payLoad.AddCustom("DeviceID", token);
            PushBroker.QueueNotification(new AppleNotification(token, payLoad)//the recipient device id
                                                                               .WithAlert(message)//the message
                                                                               .WithBadge(7)
                                                                               .WithCategory(type.ToString())
                                                                               .WithSound("sound.caf")
                                                                               );
        }
        //public static void SendNotificatonToWinners(NotificationModel model)
        //{
        //    foreach (var winner in model.Winners)
        //    {
        //        DeviceType device = (DeviceType)winner.User.DeviceTypeId;
        //        string msg = string.Format("Congratulations, you have won {0}", winner.PrizeMoney);
        //        if (device == DeviceType.Android)
        //        {
        //            NotifyAndroidUser(winner.User.DeviceId, msg, NotificationType.WonPrize);
        //        }
        //        else if (device == DeviceType.IOS)
        //        {
        //            NotifyIOSUser(winner.User.DeviceId, msg, NotificationType.WonPrize);
        //        }
        //    }

        //}

        //public static void SendNotificaton(List<UserViewModel> users, string message, NotificationType type = NotificationType.AdminMessage)
        //{
        //    var ios = users.Where(p => p.DeviceTypeId == Convert.ToInt32(DeviceType.IOS) && p.DeviceId != null).Select(p => p.DeviceId).Distinct().ToList();
        //    ios.ForEach(p => { NotifyIOSUser(p, message, type); });
        //    var android = users.Where(p => p.DeviceTypeId == Convert.ToInt32(DeviceType.Android) && p.DeviceId != null).Select(p => p.DeviceId).Distinct().ToList();
        //    NotifyAndroidUserUsers(android, message, type);
        //}

        private static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        private static void LogNotification(INotification notification, NotificationStatus status)
        {
            if (notification.GetType() == typeof(AppleNotification))
            {
                var noti = (AppleNotification)notification;
                var type = (NotificationType)Enum.Parse(typeof(NotificationType), noti.Payload.Category);
                pushNotificationService.AddNotificationLog(noti.Payload.Alert.Body,
                    noti.DeviceToken, DeviceType.IOS, type, status);
            }
            else if (notification.GetType() == typeof(GcmNotification))
            {
                var noti = (GcmNotification)notification;
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var data = (Dictionary<string, object>)json_serializer.DeserializeObject(noti.JsonData);
                var type = NotificationType.MessageAlert; //(NotificationType)Enum.Parse(typeof(NotificationType), data["type"].ToString());
                var pList = new List<PushNotificationViewModel>();
                noti.RegistrationIds.ForEach(tok =>
                {
                    pList.Add(new PushNotificationViewModel { DeviceId = tok, DeviceType = DeviceType.Android, Message = noti.JsonData, Status = status, Type = type });
                });
                pushNotificationService.AddNotificationLog(pList);
            }
        }

        private static void NotificationSent(object sender, INotification notification)
        {
            LogNotification(notification, NotificationStatus.Sent);
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        private static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            LogNotification(notification, NotificationStatus.Failed);
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
            //throw notificationFailureException;
        }

        private static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
            //throw exception;
        }

        private static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
            //throw exception;
        }

        private static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        private static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        private static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }
    }
}