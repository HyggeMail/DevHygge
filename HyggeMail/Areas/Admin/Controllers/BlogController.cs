using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Models.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.Admin.Controllers
{
    public class BlogController : AdminBaseController
    {
        #region Variable Declaration

        private readonly IBlogManager _blogManager;
        #endregion

        public BlogController(IErrorLogManager errorLogManager, IBlogManager blogManager)
            : base(errorLogManager)
        {
            _blogManager = blogManager;
        }

        #region  Blogs

        public ActionResult AddBlog()
        {
            var model = new AddBlogModel();
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            return View(model);
        }

        public ActionResult EditBlog(int blogID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            var blogModel = new AddBlogModel();
            blogModel = _blogManager.GetBlogById(blogID);
            return View("AddBlog", blogModel);
        }
        public ActionResult DisplayBlog(int blogID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            var blogModel = new AddBlogModel();
            blogModel = _blogManager.GetBlogById(blogID);
            return View(blogModel);
        }

        [AjaxOnly, HttpPost]
        public JsonResult AddBlogDetails(AddBlogModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            return Json(_blogManager.AddUpdateBlog(model), JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateBlogDetails(AddBlogModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            return Json(_blogManager.AddUpdateBlog(model), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageBlog()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            var blogs = _blogManager.GetBlogPageList(new PagingModel { PageNo = 1, RecordsPerPage = AppDefaults.PageSize, SortBy = "AddedOn", SortOrder = "Desc" });
            ViewBag.IsFeaturedArticleExist = _blogManager.IsFeaturedArticleExist();
            return View(blogs);
        }

        [AjaxOnly, HttpPost]
        public JsonResult DeleteBlog(int blogId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            return Json(_blogManager.DeleteBlog(blogId), JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetBlogPagingList(PagingModel model)
        {
            ViewBag.IsFeaturedArticleExist = _blogManager.IsFeaturedArticleExist();
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            PagingResult<BlogModel> modal = _blogManager.GetBlogPageList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_blogListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);

        }

        [AjaxOnly, HttpPost]
        public JsonResult UpdateBlogFeaturedArticleStatus(int blogId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Blog;
            return Json(_blogManager.UpdateBlogFeaturedArticleStatus(blogId), JsonRequestBehavior.AllowGet);
        }

       

        #endregion
    }
}