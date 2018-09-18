using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    public class AddUpdateAdminImageCategoryModel
    {
        public int ID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
    public class AdminImageCategoryListingModel
    {
        public int ID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> ActivatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }

    public partial class AddUpdateAdminImageModel
    {
        public int ID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<HttpPostedFileBase> Image { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<bool> ByAdmin { get; set; }
        public AddUpdateAdminImageModel() {
            this.Categories = new List<SelectListItem>();
        }
    }

    public partial class AdminImageListingModel
    {
        public int ID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> ActivatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }

}
