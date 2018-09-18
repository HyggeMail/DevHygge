using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Managers;

namespace HyggeMail.BLL.Notification
{

    public class PushNotify : IDisposable
    {
        private PushBroker _pushBroker;
        private static string APNSCerticateFile = "~/Resources/HyggeMail.p12";
        private static string APNSCerticateFilePassword = "HyggeMail";

        private readonly IPushNotificationManager _pushNotificationService;

        public PushNotify()
        {
            _pushNotificationService = new PushNotificationManager();
        }

        public PushNotify Configure()
        {
            if (_pushBroker == null)
            {
                _pushBroker = new PushBroker();
                _pushBroker.OnNotificationSent += NotificationSent;
                _pushBroker.OnChannelException += ChannelException;
                _pushBroker.OnServiceException += ServiceException;
                _pushBroker.OnNotificationFailed += NotificationFailed;
                _pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                _pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                _pushBroker.OnChannelCreated += ChannelCreated;
                _pushBroker.OnChannelDestroyed += ChannelDestroyed;
                var settings = new PushServiceSettings();
                settings.MaxNotificationRequeues = 1000;
                settings.MinAvgTimeToScaleChannels = 1000;
                settings.NotificationSendTimeout = 15000;
                settings.IdleTimeout = TimeSpan.FromMinutes(10);
                settings.AutoScaleChannels = true;
                _pushBroker.RegisterGcmService(new GcmPushChannelSettings(Config.BrowserKey), settings);
            }

            return this;
        }

        public void NotifyAndroidUser(string token, string json, NotificationType type)
        {
            if (_pushBroker == null)
                Configure();
          
            //Fluent construction of an Android GCM Notification
            //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
            var noti = new GcmNotification();
            //noti.DryRun = true;
            //PushBroker.QueueNotification(noti.ForDeviceRegistrationId(token).WithJson("{\"alert\":\"" + message + "\",\"badge\":6, \"type\":\"" + type.ToString() + "\"}"));
            _pushBroker.QueueNotification(noti.ForDeviceRegistrationId(token).WithJson(json));
        }


        public void NotifyIOSUser(string token, string json, NotificationType type)
        {
            ////Fluent construction of an iOS notification
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
            if (_pushBroker == null)
                _pushBroker = new PushBroker();

            _pushBroker.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, APNSCerticateFilePassword, true));

            _pushBroker.QueueNotification(new AppleNotification()
                                       .ForDeviceToken(token)
                                       .WithAlert(json)
                                       .WithCategory(type.ToString())
                                                          .WithSound("sound.caf"));
        }

        public void NotifyIOSUserCustom(string token, string message, NotificationType type, IOSNOtifcationModel requestModel)
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

            payLoad.AddCustom("NotificationType", requestModel.Request.NotificationType);
            payLoad.AddCustom("SenderName", requestModel.Request.SenderName);
            payLoad.AddCustom("Badge", requestModel.Request.Badge);
            payLoad.AddCustom("SenderID", requestModel.Request.SenderID);
            payLoad.AddCustom("Message", requestModel.Request.Message);
            payLoad.AddCustom("DeviceType", requestModel.DeviceModel.DeviceType);
            payLoad.AddCustom("ExpertResponse", requestModel.Request.ExpertResponse);
            payLoad.AddCustom("DeviceToken", requestModel.DeviceModel.DeviceToken);
            payLoad.AddCustom("WorkOrderScopeID", requestModel.Request.WorkOrderScopeID);
            payLoad.AddCustom("ExpertID", requestModel.Request.ExpertID);
            payLoad.AddCustom("ReceiverROle", requestModel.Request.ReceiverRole);
            if (_pushBroker == null)
                Configure();
            //_pushBroker = new PushBroker();
            _pushBroker.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, APNSCerticateFilePassword, true));
            _pushBroker.QueueNotification(new AppleNotification(token, payLoad)//the recipient device id
                                                                               .WithAlert(message)//the message
                                                                               .WithBadge(requestModel.Request.Badge)
                                                                               .WithCategory(type.ToString())
                                                                               .WithSound("sound.caf")
                                                                               );
        }

        public void NotifyAndroidUserUsers(List<string> deviceIds, string message, NotificationType type)
        {
            //Fluent construction of an Android GCM Notification
            //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
            var noti = new GcmNotification();
            noti.RegistrationIds = deviceIds;
            _pushBroker.QueueNotification(noti.WithJson("{\"alert\":\"" + message + "\",\"badge\":6, \"type\":\"" + type.ToString() + "\"}"));
        }

        private void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Debug.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        private void LogNotification(INotification notification, NotificationStatus status)
        {
            if (notification.GetType() == typeof(AppleNotification))
            {
                var noti = (AppleNotification)notification;
                var type = (NotificationType)Enum.Parse(typeof(NotificationType), noti.Payload.Category);
                _pushNotificationService.AddNotificationLog(noti.Payload.Alert.Body,
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
                _pushNotificationService.AddNotificationLog(pList);
            }
        }

        private void NotificationSent(object sender, INotification notification)
        {
            LogNotification(notification, NotificationStatus.Sent);
            Debug.WriteLine("Sent: " + sender + " -> " + notification);
        }

        private void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            LogNotification(notification, NotificationStatus.Failed);
            Debug.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
            //throw notificationFailureException;
        }

        private void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Debug.WriteLine("Channel Exception: " + sender + " -> " + exception);
            //throw exception;
        }

        private void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
            //throw exception;
        }

        private void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Debug.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        private void ChannelDestroyed(object sender)
        {
            Debug.WriteLine("Channel Destroyed for: " + sender);
        }

        private void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Debug.WriteLine("Channel Created for: " + sender);
        }

        public void Dispose()
        {
            _pushBroker.StopAllServices();
        }
    }
}