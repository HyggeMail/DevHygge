using HyggeMail.DAL;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public class PayPalModel
    {
        public APIContext ApiContext { get; set; }
        public string ReturenUrl { get; set; }
        public string CancelUrl { get; set; }
        public Guid Id { get; set; }
        public string MembershipName { get; set; }
        public int UserID { get; set; }
        public int PlanID { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class PayPalTransaction
    {
        public string TransactionID { get; set; }
        public string Gateway { get; set; }
        public string Status { get; set; }
        public string TransactionDate { get; set; }
        public string MembershipPlan { get; set; }
        public string UserName { get; set; }
        public decimal? Amount { get; set; }
        public int? PendingCardCount { get; set; }
        public int? PackageCardCount { get; set; }
        public bool IsBasic { get; set; }
        public PayPalTransaction() { }

        public PayPalTransaction(UserTransaction obj)
        {
            this.TransactionID = obj.TransactionID;
            this.Gateway = obj.Gateway;
            this.Status = obj.Status;
            this.TransactionDate = Convert.ToDateTime(obj.TransactionDate).ToShortDateString();
            this.MembershipPlan = obj.MembershipPlan.Name;
            this.UserName = obj.User.FirstName + " " + obj.User.LastName;
            this.Amount = obj.TransactionAmount;
            this.IsBasic = false;
            this.PackageCardCount = obj.MembershipPlan.CardsAllocated;
            this.PendingCardCount = obj.User.CardsCount;
        }
    }
}
