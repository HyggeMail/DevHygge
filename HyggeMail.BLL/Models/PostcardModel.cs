using HyggeMail.BLL.Common;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    class PostcardModel
    {
    }
    public class AddUpdateImageEditorModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string CardFront { get; set; }
        public string CardBack { get; set; }

        public string CardFrontPath { get; set; }
        public string CardBackPath { get; set; }

        public string CardFrontJson { get; set; }
        public string CardBackJson { get; set; }
        public DateTime? ShippedOn { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public bool IncludeAddress { get; set; }
        public bool IsOrderPlaced { get; set; }
        public List<UserRecipientModel> Recipients { get; set; }
        public List<PostCardSelectedImages> SelectedImages { get; set; }
        public bool IsCopyCard { get; set; }
        public string CardBackJsonWithIFrame { get; set; }
        public string CardBackPathWithIFrame { get; set; }
        public string CardBackWithFrame { get; set; }

        public AddUpdateImageEditorModel()
        {
            this.Recipients = new List<UserRecipientModel>();
            this.SelectedImages = new List<PostCardSelectedImages>();
        }
    }
    public class PostCardResultModel
    {
        public int ID { get; set; }
        public string CardFront { get; set; }
        public string CardBack { get; set; }
        public string CardBackWithFrame { get; set; }
        public DateTime? AddedOn { get; set; }
        public PostCardResultModel() { }
        public PostCardResultModel(UserPostCard book)
        {
            if (book.CardFrontPath != null)
                this.CardFront = book.CardFrontPath.Replace("~/", "../../");
            if (book.CardBackWithFrame != null)
                this.CardBackWithFrame = book.CardBackWithFrame.Replace("~/", "../../");
            if (book.CardBackPath != null)
                this.CardBack = book.CardBackPath.Replace("~/", "../../");
            this.ID = book.ID;
            this.AddedOn = book.AddedOn;
        }
    }
    public class PostCardFrontBack
    {
        public int UserID { get; set; }
        public string cardFront { get; set; }
        public string cardBack { get; set; }
    }
    public class UserRecipientModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public bool IsApproved { get; set; }
    }
    public class PostCardSelectedImages
    {
        public int ID { get; set; }
        public int AdminImageID { get; set; }
    }
    public class PostCardListingModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int OrderID { get; set; }
        public eOrderStatus orderStatus { get; set; }
        public string Username { get; set; }

        public string CardFront { get; set; }
        public string CardBack { get; set; }

        public string CardFrontPath { get; set; }
        public string CardBackPath { get; set; }

        public DateTime ShippedOn { get; set; }
        public bool IsCancel { get; set; }
        public DateTime CancelledOn { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public bool IncludeAddress { get; set; }
        public bool IsOrderPlaced { get; set; }

        public Nullable<bool> IsApproved { get; set; }
        public Nullable<System.DateTime> ApprovedOn { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<System.DateTime> RejectedOn { get; set; }

        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public List<UserRecipientModel> Recipients { get; set; }
        public List<SelectListItem> OrderSelectList { get; set; }
        public PostCardListingModel()
        {
            this.OrderSelectList = new List<SelectListItem>();
            this.OrderSelectList = Utilities.EnumToList(typeof(eOrderStatus));
        }

    }

    public class RecipientPostCardListingModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int OrderID { get; set; }
        public eOrderStatus orderStatus { get; set; }
        public string Username { get; set; }
        public string UserAddress { get; set; }
        public string NewLineAdress { get; set; }
        public bool IncludeAddress { get; set; }

        public string CardFront { get; set; }
        public string CardBack { get; set; }

        public string CardFrontPath { get; set; }
        public string CardBackPath { get; set; }

        public string CardBackPathWithIFrame { get; set; }
        public string CardBackWithFrame { get; set; }

        public DateTime ShippedOn { get; set; }
        public bool IsCancel { get; set; }
        public DateTime CancelledOn { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsError { get; set; }
        public Nullable<System.DateTime> ApprovedOn { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<System.DateTime> RejectedOn { get; set; }

        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public int CardStatus { get; set; }
        public int UserPostCardID { get; set; }

        public List<SelectListItem> OrderSelectList { get; set; }
        public RecipientPostCardListingModel()
        {
            this.OrderSelectList = new List<SelectListItem>();
            this.OrderSelectList = Utilities.EnumToList(typeof(eOrderStatus));
        }
    }


    public class RejectWithReasonModel
    {
        public int RecipientCardID { get; set; }
        public string Reason { get; set; }
    }

    public class UserPostCardViewModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }

        public string CardFront { get; set; }
        public string CardBack { get; set; }

        public string CardFrontPath { get; set; }
        public string CardBackPath { get; set; }
    }

    public class PostCardFBModel
    {
        public int PostCardId { get; set; }
        public int UserId { get; set; }
        public string Image { get; set; }
    }

}
