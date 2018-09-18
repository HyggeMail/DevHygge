using HyggeMail.BLL.Common;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    public class CMSPageViewModel
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageTitle { get; set; }
        public DateTime CreatedOn { get; set; }

        public CMSPageViewModel()
        {

        }

        internal CMSPageViewModel(CMSPage pageContent)
        {
            this.PageId = pageContent.PageId;
            this.PageTitle = pageContent.PageTitle;
            this.CreatedOn = pageContent.CreatedOn;
            this.PageName = pageContent.PageName;

        }
    }

    public class EditCMSPageModel
    {
        public int PageId { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string PageName { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string PageTitle { get; set; }
        [AllowHtml]
        public string PageContent { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        public EditCMSPageModel()
        {

        }

        internal EditCMSPageModel(CMSPage pageContent)
        {
            this.PageId = pageContent.PageId;
            this.PageName = pageContent.PageName;
            this.PageTitle = pageContent.PageTitle;
            this.PageContent = pageContent.PageContent;
            this.MetaTitle = pageContent.MetaTitle;
            this.MetaKeywords = pageContent.MetaKeywords;
            this.MetaDescription = pageContent.MetaDescription;

        }


    }
    public class AdminDashboardCountModel
    {
        public AdminDashboardDetails Users { get; set; }
        public AdminDashboardDetails Postcards { get; set; }
        public AdminDashboardDetails Orders { get; set; }
        public AdminDashboardDetails TotalIncome { get; set; }
        public AdminDashboardDetails Messages { get; set; }
    }
    public class AdminDashboardDetails
    {
        public string Daily { get; set; }
        public string Weekly { get; set; }
        public string Monthly { get; set; }
        public string Yearly { get; set; }
        public string Total { get; set; }
        public string Unresolved { get; set; }

    }
     public class CurrentWeek
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

    }
}
