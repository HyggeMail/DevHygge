using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HyggeMail.BLL.Models
{
    public enum ActionStatus
    {
        Successfull = 1,
        Error = 2,
        LoggedOut = 3,
        Unauthorized = 4,
        Failed = 5,
    }
    public enum TemplateTypes
    {
        [Description("Welcome Email")]
        WelcomeEmail = 1,
        [Description("Forget Password")]
        ForgetPassword = 2,
        [Description("Reset Password")]
        ResetPassword = 3,
        [Description("Verification Account")]
        VerificationAccount = 4,
        [Description("User SignIn Details")]
        UserSigninDetails = 5,
        [Description("Reset Password and Verify Account")]
        ResetPasswordandVerifyAccount = 6,
        [Description("Order Placed")]
        OrderPlaced = 7,
        [Description("OrderStatusChange")]
        OrderStatusChange = 8,
        [Description("RejectionEmail")]
        RejectionEmail = 9,
        [Description("Sign Up Email")]
        SignUpEmail = 10,
        [Description("Card Was Mailed Email")]
        CardWasMailedEmail = 11,
        [Description("Order Status Admin")]
        OrderStatusAdmin = 12,
        [Description("Get The Guide Email")]
        GetTheGuideEmail = 13
    }

    public enum eFAQCategory
    {

        [Description("GENERAL")]
        FAQ = 1,
        [Description("For Business")]
        ForBusiness = 2,
        [Description("Redactor")]
        Redactor = 3,
        [Description("Mobile App")]
        MobileApp = 4,
        [Description("Rullers")]
        Rullers = 5
    }

    public enum eOrderStatus
    {
        [Description("Order Placed")]
        OrderPlaced = 1,
        [Description("In Progress")]
        InProgress = 2,
        [Description("Approved")]
        Approved = 3,
        [Description("Shipped")]
        Shipped = 4
    }
    public enum eRecipientOrderStatus
    {
        [Description("New")]
        New = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Completed")]
        Completed = 3,
        [Description("Errors")]
        Errors = 4,
        [Description("Rejected")]
        Rejected = 5
    }

    public enum eShownRecipientOrdersByDays
    {
        [Description("Past")]
        Past = 1,
        [Description("Today")]
        Today = 2,
        [Description("Future")]
        Future = 3,
    }


    public enum ContactRequestType
    {
        [Description("User related")]
        UserRelated = 1,
        [Description("Postcard")]
        Postcard = 2,
    }

    public enum Sessionstatus
    {
        Present = 1,
        Expired = 2,
        Invalid = 3,
    }
    public enum eShowOrderStatus
    {
        All = 1,
        Shipped = 2,
        Invalid = 3,
    }
    public static class AttacmentsPath
    {
        public const string AdminImages = "~/Uploads/Attachments/";
        public const string AdminImagesThumb = "~/Uploads/Attachments/Thumb/";
        public const string UserProfileImages = "~/Uploads/Users/";
    }
    public enum UserTypes
    {
        [Description("Admin")]
        Admin = 1,
        [Description("User")]
        User = 2,
    }
    public enum eAdminPostOrderStatus
    {
        [Description("New")]
        New = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Completed")]
        Completed = 3

    }
    public enum AuthenticationType
    {
        [Description("Normal")]
        Normal = 1,
        [Description("Facebook")]
        Facebook = 2,
    }


    public enum CMSPageType
    {
        [Description("AboutUs")]
        AboutUs = 1,
        [Description("Terms And Conditions")]
        TermsAndConditions = 2,
        [Description("Privacy Policy")]
        PrivacyPolicy = 3,
        [Description("Mobile App")]
        MobileApp = 4,

    }

    public enum DeviceType
    {
        Android = 1,
        IOS = 2
    }

    public enum NotificationType
    {
        MessageAlert = 1
    }
    public enum NotificationStatus
    {
        Seen = 0,
        Sent = 1,
        Failed,
        Pending
    }
    public enum Priority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

    public enum CardStatusTypes
    {
        [Description("InProgress")]
        InProgress = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Completed")]
        Completed = 3,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Error")]
        Error = 5,
        [Description("Cancelled")]
        Cancelled = 6
    }
}