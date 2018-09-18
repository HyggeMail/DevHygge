using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Configuration;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using AutoMapper;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;


namespace HyggeMail.BLL.Managers
{
    public class TestimonialManager : BaseManager, ITestimonialManager
    {
        PagingResult<TestimonialModel> ITestimonialManager.GetTestimonialPageList(PagingModel model, int userID, UserTypes userType)
        {
              var result = new PagingResult<TestimonialModel>();
            var query = Context.tblTestimonials.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.Title.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new TestimonialModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Testimonial List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput ITestimonialManager.AddUpdateTestimonial(AddTestimonialModel testimonialModel)
        {
            try
            {
                if (testimonialModel.ID > 0)
                {
                    var testimonial = Context.tblTestimonials.Where(z => z.ID == testimonialModel.ID && z.IsDeleted != true).FirstOrDefault();
                    if (testimonial != null)
                    {
                        testimonial.Title = testimonialModel.Title;
                        if (testimonialModel.Image == null)
                            testimonial.Image = testimonialModel.ImageName;
                        else
                            testimonial.Image = Utilities.SaveImage(testimonialModel.Image, AppDefaults.TestimonialsPath, AppDefaults.TestimonialsThumbPath);

                        testimonial.Name = testimonialModel.Name;
                        testimonial.Description = testimonialModel.Description;
                        Context.SaveChanges();
                        return new ActionOutput
                        {
                            Status = ActionStatus.Successfull,
                            Message = "Sucessfully Updated."
                        };
                    }
                    else
                    {
                        return new ActionOutput
                        {
                            Status = ActionStatus.Error,
                            Message = "No testimonial found."
                        };
                    }
                }
                else
                {
                    var newTestimonial = new tblTestimonial();

                    newTestimonial.Title = testimonialModel.Title;
                    newTestimonial.Name = testimonialModel.Name;
                    newTestimonial.Description = testimonialModel.Description;
                    newTestimonial.IsActive = testimonialModel.IsActive;
                    newTestimonial.ActivatedOn = DateTime.UtcNow;
                    newTestimonial.AddedOn = DateTime.UtcNow;
                    newTestimonial.Image = Utilities.SaveImage(testimonialModel.Image, AppDefaults.TestimonialsPath, AppDefaults.TestimonialsThumbPath);
                    newTestimonial.IsDeleted = testimonialModel.IsDeleted;
                    newTestimonial.DeletedOn = testimonialModel.DeletedOn;
                    Context.tblTestimonials.Add(newTestimonial);
                    Context.SaveChanges();
                    return new ActionOutput
                    {
                        Status = ActionStatus.Successfull,
                        Message = "Sucessfully Added."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Internal Server error."
                };
            }
        }

        ActionOutput<TestimonialModel> ITestimonialManager.GetAllTestimonials()
        {
            var result = new ActionOutput<TestimonialModel>();
            var testimonials = Context.tblTestimonials.Where(x => x.IsDeleted != true).OrderByDescending(x => Guid.NewGuid()).Take(3).AsEnumerable();
            var list = testimonials.Select(x => new TestimonialModel(x)).ToList();
            if (list!=null && list.Count > 0)
            {
                return new ActionOutput<TestimonialModel>
                {
                    Status = ActionStatus.Successfull,
                    Message = "Testimonial List",
                    List = list
                };
            }
            else
                return null;
        }

        AddTestimonialModel ITestimonialManager.GetTestimonialById(int Id)
        {
            var testimonial = Context.tblTestimonials.FirstOrDefault(x => x.ID == Id);
            if (testimonial == null)
                return null;
            else
                return new AddTestimonialModel(testimonial);
        }

        ActionOutput ITestimonialManager.DeleteTestimonial(int Id)
        {
            var testimonial = Context.tblTestimonials.Where(z => z.ID == Id).FirstOrDefault();
            if (testimonial == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Testimonial Not Exist."
                };
            }
            else
            {
                Context.tblTestimonials.Remove(testimonial);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Testimonial Deleted Successfully."
                };
            }
        }

    
    }
}
