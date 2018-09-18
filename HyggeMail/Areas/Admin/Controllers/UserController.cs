using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly IEmailTemplateManager _templateManager;
        #endregion

        public UserController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager templateManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _templateManager = templateManager;
        }

        #region User Management

        [HttpGet]
        public ActionResult ManageUsers()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var users = _userManager.GetUserPagedList(PagingModel.DefaultModel("CreatedAt"));
            return View(users);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetUsersPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var modal = _userManager.GetUserPagedList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_userListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        public ActionResult AddUser()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return View(new AddUserModel());
        }

        [AjaxOnly, HttpPost]
        public JsonResult AddUserDetails(AddUserModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return JsonResult(_userManager.AddUserDetails(model));
        }

        public ActionResult EditUser(int userId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var userModel = new UserModel();
            userModel = _userManager.GetUserDetailsByUserId(userId).Object;
            return View(userModel);
        }

        [AjaxOnly, HttpPost]
        public JsonResult UpdateUserDetails(UserModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return JsonResult(_userManager.UpdateUserDetails(model));
        }

        [AjaxOnly, HttpPost]
        public JsonResult DeleteUser(int userId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return JsonResult(_userManager.DeleteUser(userId));
        }

        [HttpGet]
        public ActionResult ManageUserHistory(int uid)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var reqModel = new PagingModel();
            ViewBag.UserID = uid;
            reqModel.SortOrder = "Desc"; reqModel.SortBy = "AddedOn"; reqModel.UserID = uid;
            var users = _userManager.GetUserHistoryPageList(reqModel);
            return View(users);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetUserHistoryPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var modal = _userManager.GetUserHistoryPageList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_userHistory", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult ActivateUser(int userId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return JsonResult(_userManager.ActivateUser(userId));
        }

        [AjaxOnly, HttpPost]
        public JsonResult SendAccountVerificationMail(int userId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            return JsonResult(_userManager.SendAccountVerificationMail(userId));
        }

        #endregion
    }
}