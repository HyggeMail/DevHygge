using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public class ReceiptentOrder
    {
        public int OrderID { get; set; }
        public string DatePlaced { get; set; }
        public string DueTo { get; set; }
        public string BackImage { get; set; }
        public string FrontImage { get; set; }
        public string UserName { get; set; }
        public string ReceiptentName { get; set; }
        public string ReceiptentAddress { get; set; }

        public ReceiptentOrder() { }
        public ReceiptentOrder(UserPostCardRecipient obj)
        {
            this.OrderID = obj.UserPostCardID ?? 0;
            this.DatePlaced = Convert.ToDateTime(obj.UserPostCard.AddedOn).ToString("hh:mm tt dd MMM yyyy");
            this.DueTo = Convert.ToDateTime(obj.UserPostCard.ShipmentDate).ToString("hh:mm tt dd MMM yyyy");
            this.BackImage = obj.UserPostCard.CardBackPath;
            this.FrontImage = obj.UserPostCard.CardFrontPath;
            this.UserName = obj.UserPostCard.User.FirstName + " " + obj.UserPostCard.User.LastName;
            this.ReceiptentName = obj.Name;
            this.ReceiptentAddress = obj.Address + ", " + obj.City + ", " + obj.State + ", " + obj.Zip + ", " + obj.Country;
        }
    }
}
