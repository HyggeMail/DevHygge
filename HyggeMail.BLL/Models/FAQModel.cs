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
    public class FAQModel
    {
        public int ID { get; set; }
        [Required]
        public eFAQCategory CategoryID { get; set; }
        [Required]
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        public string Description { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public List<SelectListItem> CategoryTypeList { get; set; }

        public FAQModel()
        {

            this.CategoryTypeList = new List<SelectListItem>();
            this.CategoryTypeList = Utilities.EnumToList(typeof(eFAQCategory));
        }

        public FAQModel(FAQ model)
        {
            this.ID = model.ID;
            this.CategoryID = (eFAQCategory)model.CategoryID;
            this.Title = model.Title;
            this.Description = model.Description;
            this.AddedOn = model.AddedOn;
            this.UpdatedOn = model.UpdatedOn;
            this.IsDeleted = model.IsDeleted;
            this.DeletedOn = model.DeletedOn;
            this.CategoryTypeList = Utilities.EnumToList(typeof(eFAQCategory));
        }


    }

}
