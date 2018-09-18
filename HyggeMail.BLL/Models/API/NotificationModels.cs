using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HyggeMail.BLL.Common;
namespace HyggeMail.BLL.Models
{
    public class PushNotificationViewModel
    {
        public int SerialNo { get; set; }
        public int PushNotificationId { get; set; }
        public string DeviceId { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public short DeviceTypeId { get; set; }
        public Nullable<short> TypeId { get; set; }
        public short StatusId { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public DeviceType DeviceType { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationType Type { get; set; }

        public string strDevice { get { return ((DeviceType)DeviceTypeId).ToEnumWordify(); } }
        public string strStatus { get { return ((NotificationStatus)StatusId).ToEnumWordify(); } }
        public string strType { get { return ((NotificationType)TypeId).ToEnumWordify(); } }
    }

    public class NotificationModel
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string TextMessage { get; set; }
        public NotificationType NotiType { get; set; }
    }

    public class TypingStatus
    {
        public DeviceModel DeviceModel { get; set; }
        public string Status { get; set; }
    }
    #region IOS Notification

    public class IOS_Notification : GCMResponseModel
    {
        public int WorkorderId { get; set; }
        public string WorkorderIntro { get; set; }
        public string WorkOrderDesc { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public int ReceiverId { get; set; }
        public int Badge { get; set; }
        public string Message { get; set; }
        public int WOrkOrderStatus { get; set; }
        public int ExpertResponse { get; set; }
        public int? WorkOrderScopeID { get; set; }
        public int? ExpertID { get; set; }
        public int ReceiverRole { get; set; }
        public int NotificationType { get; set; }
    }
    public class IOSNOtifcationModel
    {
        public DeviceModel DeviceModel { get; set; }
        public IOS_Notification Request { get; set; }
    }

    #endregion


}
