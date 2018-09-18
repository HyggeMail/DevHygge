using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyggeMail.BLL.Models
{
    public static class Cookies
    {
        //replace xyz with required name as per your project
        public const string AuthorizationCookie = "xyzAuthorize";

        //replace xyz with required name as per your project
        public const string AuthorizationCookieMobile = "xyzAuthorizeMobile";

        //replace xyz with required name as per your project
        public const string AdminAuthorizationCookie = "xyzAdminAuthorize";

    }
    public static class SelectedAdminTab
    {
        public const string Users = "Users";
        public const string CMSManager = "CMSManager";
        public const string Templates = "Templates";
        public const string Images = "Images";
        public const string Postcard = "Postcard";
        public const string ContactUs = "ContactUs";
        public const string Membership = "Membership";
        public const string FAQ = "FAQ";
        public const string Transactions = "Transactions";
        public const string Subscriber = "Subscriber";
        public const string Testimonials = "Testimonials";
        public const string Blog = "Blog";
        public const string AddressBook = "AddressBook";
        
    }
    public static class AppDefaults
    {
        public const Int32 PageSize = 10;
        public const string TestimonialsPath = "~/Uploads/Testimonial/";
        public const string TestimonialsThumbPath = "~/Uploads/Testimonial/Thumb/";
        public const string BlogPath = "~/Uploads/Blog/";
        public const string BlogThumbPath = "~/Uploads/Blog/Thumb/";
    }

    public static class EmailConst 
    {
        public const string UserName = "@{UserName}";
    }
}