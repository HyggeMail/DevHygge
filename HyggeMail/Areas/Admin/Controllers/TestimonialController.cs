using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HyggeMail.Areas.Admin.Controllers
{
    public class TestimonialController : AdminBaseController
    {
        #region Variable Declaration

        private readonly ITestimonialManager _testimonialManager;
        #endregion

        public TestimonialController(IErrorLogManager errorLogManager, ITestimonialManager testimonialManager)
            : base(errorLogManager)
        {
            _testimonialManager = testimonialManager;
        }

        #region  Testimonials
        public ActionResult AddTestimonials()
        {
            var model = new AddTestimonialModel();
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            return View(model);
        }

        public ActionResult EditTestimonials(int testimonialID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            var testimonialModel = new AddTestimonialModel();
            testimonialModel = _testimonialManager.GetTestimonialById(testimonialID);
            return View(testimonialModel);
        }
        public ActionResult DisplayTestimonial(int testimonialID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            var testimonialModel = new AddTestimonialModel();
            testimonialModel = _testimonialManager.GetTestimonialById(testimonialID);
            return View(testimonialModel);
        }
        
        [AjaxOnly, HttpPost]
        public JsonResult AddtestimonialDetails(AddTestimonialModel model)
        {
            model.UserFK = LOGGEDIN_USER.UserID;
            model.userType = UserTypes.Admin;
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            return Json(_testimonialManager.AddUpdateTestimonial(model), JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdatetestimonialDetails(AddTestimonialModel model)
        {
            model.userType = UserTypes.Admin;
            model.UserFK = LOGGEDIN_USER.UserID;
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            return Json(_testimonialManager.AddUpdateTestimonial(model), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageTestimonials()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            var testimonials = _testimonialManager.GetTestimonialPageList(new PagingModel { PageNo = 1, RecordsPerPage = AppDefaults.PageSize, SortBy = "AddedOn", SortOrder = "Desc" }, LOGGEDIN_USER.UserID, UserTypes.Admin);
            return View(testimonials);
        }
        [AjaxOnly, HttpPost]
        public JsonResult Deletetestimonial(int testimonialId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Testimonials;
            return Json(_testimonialManager.DeleteTestimonial(testimonialId), JsonRequestBehavior.AllowGet);
        }
        [AjaxOnly, HttpPost]
        public JsonResult GettestimonialsPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            // model.SortOrder = "Desc";
            PagingResult<TestimonialModel> modal = _testimonialManager.GetTestimonialPageList(model, LOGGEDIN_USER.UserID, UserTypes.Admin);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_testimonialListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}