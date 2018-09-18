using HyggeMail.DAL;
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
    /// <summary>
        /// Testimonial model this will used to add Testimonial
        /// </summary>
        public class TestimonialModel
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string Name { get; set; }
            public string ImageName { get; set; }
            public string Description { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public bool? IsDeleted { get; set; }
            public DateTime? DeletedOn { get; set; }
            public int? UserFK { get; set; }
            public DateTime? AddedOn { get; set; }
            public string AddedBy { get; set; }
            public UserTypes UserType { get; set; }
            public TestimonialModel()
            { }
            public TestimonialModel(tblTestimonial testimonialObj)
            {
                this.ID = testimonialObj.ID;
                this.Title = testimonialObj.Title;
                this.Description = testimonialObj.Description;
                this.IsActive = testimonialObj.IsActive;
                this.ActivatedOn = testimonialObj.ActivatedOn;
                this.IsDeleted = testimonialObj.IsDeleted;
                this.DeletedOn = testimonialObj.DeletedOn;
                this.AddedOn = testimonialObj.AddedOn;
                 this.ImageName = testimonialObj.Image;
                this.Name = testimonialObj.Name;
         
            
            }
        }
        /// <summary>
        /// Testimonial model this will used to add Testimonial
        /// </summary>
        public class AddTestimonialModel
        {
            public int ID { get; set; }
            [AllowHtml, Required(ErrorMessage = "*Required")]
            public string Title { get; set; }
            [AllowHtml, Required(ErrorMessage = "*Required")]
            [StringLength(500)]
            public string Description { get; set; }
            // [FileSize(10000000, ErrorMessage = "The file size should not be exceed 10MB.")]
            // [Required(ErrorMessage = "Upload image ")]
            public HttpPostedFileBase Image { get; set; }
            public string ImageName { get; set; }
            [Required(ErrorMessage = "*Required")]
            public string Name { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public bool? IsDeleted { get; set; }
            public DateTime? DeletedOn { get; set; }
            public int? UserFK { get; set; }
            public UserTypes userType { get; set; }
            public DateTime? AddedOn { get; set; }
            public virtual User User { get; set; }
            public AddTestimonialModel()
            { }
            public AddTestimonialModel(tblTestimonial testimonialObj)
            {
                this.ID = testimonialObj.ID;
                this.Title = testimonialObj.Title;
                this.ImageName = testimonialObj.Image;
                this.Description = testimonialObj.Description;
                this.IsActive = testimonialObj.IsActive;
                this.ActivatedOn = testimonialObj.ActivatedOn;
                this.IsDeleted = testimonialObj.IsDeleted;
                this.DeletedOn = testimonialObj.DeletedOn;
                this.AddedOn = testimonialObj.AddedOn;
                this.Name = testimonialObj.Name;
            }


        }

        /// <summary>
        /// Testimonial model this will used to add Testimonial
        /// </summary>
        public class EditTestimonialModel
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public bool? IsDeleted { get; set; }
            public DateTime? DeletedOn { get; set; }
            public int? UserFK { get; set; }
            public DateTime? AddedOn { get; set; }
            public virtual User User { get; set; }
            public EditTestimonialModel()
            { }
            public EditTestimonialModel(tblTestimonial testimonialObj)
            {
                this.ID = testimonialObj.ID;
                this.Title = testimonialObj.Title;
                this.Description = testimonialObj.Description;
                this.IsActive = testimonialObj.IsActive;
                this.ActivatedOn = testimonialObj.ActivatedOn;
                this.IsDeleted = testimonialObj.IsDeleted;
                this.DeletedOn = testimonialObj.DeletedOn;
                this.AddedOn = testimonialObj.AddedOn;
              }


        }
  
}
