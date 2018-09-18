using HyggeMail.BLL.Common;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models.Admin
{

    public class BlogModel      
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsFeaturedArticle { get; set; }
        public string EncodeId { get; set; }

        public BlogModel()
        { }
        public BlogModel(BlogDetail model)
        {
            this.ID = model.ID;
            this.Title = model.Title;
            this.Description = model.Description;
            this.IsActive = model.IsActive;
            this.IsDeleted = model.IsDeleted;
            this.AddedOn = model.AddedOn;
            this.ImageName = model.Image;
            this.Name = model.Name;
            this.IsFeaturedArticle = model.IsFeaturedArticle;
            this.EncodeId = Utilities.EncodeString(model.ID.ToString());
        }
    }

    public class AddBlogModel
    {
        public int ID { get; set; }
        [AllowHtml, Required(ErrorMessage = "*Required")]
        public string Title { get; set; }
        [AllowHtml, Required(ErrorMessage = "*Required")]
        public string Description { get; set; }
        //[FileSize(10000000, ErrorMessage = "The file size should not be exceed 10MB.")]
        //[Required(ErrorMessage = "Upload image ")]
        public HttpPostedFileBase Image { get; set; }
        public string ImageName { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsFeaturedArticle { get; set; }

        public AddBlogModel()
        { }
        public AddBlogModel(BlogDetail model)
        {
            this.ID = model.ID;
            this.Title = model.Title;
            this.ImageName = model.Image;
            this.Description = model.Description;
            this.IsActive = model.IsActive;
            this.IsDeleted = model.IsDeleted;
            this.AddedOn = model.AddedOn;
            this.Name = model.Name;
            this.IsFeaturedArticle = model.IsFeaturedArticle;
        }


    }
}
