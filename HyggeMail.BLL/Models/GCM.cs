using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HyggeMail.BLL.Models
{
    public class DeviceModel
    {
        public string DeviceToken { get; set; }
        public DeviceType DeviceType { get; set; }
        public int UserId { get; set; }
    }
    #region GCM Models

    public interface IGCMResponseModel
    {
        Guid NotificationId { get; set; }

        int ReceiverId { get; set; }

        string GetJson();
    }

    public class GCMResponseModel : IGCMResponseModel
    {
        public int SenderId { get; set; }
        public string GCM_ID { get; set; }
        public string SenderName { get; set; }
        public Guid NotificationId { get; set; }
        public string WorkOrderName { get; set; }
        public int ReceiverId { get; set; }

        public string GetJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }


    public class GCM_Session_Model
    {
        public DeviceModel DeviceModel { get; set; }
        public GCM_Session_Expired GCMResponseModel { get; set; }
    }

    public class GCM_Session_Expired : GCMResponseModel
    {
        public string SenderPhotoURl { get; set; }
        public string Message { get; set; }
        public DeviceModel DeviceModel { get; set; }
        public int WorkOrderID { get; set; }

        public GCM_Session_Expired()
        {
            GCM_ID = "GCM_Session_Expired";
        }
    }
    #endregion GCM Models
}
