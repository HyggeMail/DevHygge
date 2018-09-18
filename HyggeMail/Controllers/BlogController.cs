#region Default Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
#endregion

#region Custom Namespaces
using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using Ninject;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Models.Admin;
#endregion

namespace HyggeMail.Controllers
{
    public class BlogController : BaseController
    {
        #region Variable Declaration

        private readonly IBlogManager _blogManager;
        private const int RecordsPerPage = 6;
        private const string SortBy = "AddedOn";
        private const string SortOrder = "Desc";
        #endregion

        public BlogController(IErrorLogManager errorLogManager, IBlogManager blogManager)
            : base(errorLogManager)
        {
            _blogManager = blogManager;
        }



        [Public]
        public ActionResult Index()
        {
            ViewBag.RecordsPerPage = RecordsPerPage;
            ViewBag.SortBy = SortBy;
            ViewBag.SortOrder = SortOrder;

            var blogs = _blogManager.GetBlogPageList(new PagingModel { PageNo = 1, RecordsPerPage = RecordsPerPage, SortBy = SortBy, SortOrder = SortOrder });
            return View(blogs);
        }

        [AjaxOnly, HttpPost, Public]
        public JsonResult GetBlogPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            PagingResult<BlogModel> modal = _blogManager.GetBlogPageList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_blogListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);

        }

        [Public]
        public ActionResult Description(string id)
        {
            if (string.IsNullOrEmpty(id))
               return RedirectToAction("Index");

            var model = _blogManager.GetBlogByEncodeId(id);
            return View(model);
        }

        [AjaxOnly, HttpPost,Public]
        public JsonResult GetFeaturedArticleBlog()
        {
            var modal = _blogManager.GetFeaturedArticleBlog();
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_featuredArticleBlog", modal));
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);

        }

        
    }
}