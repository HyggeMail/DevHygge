﻿using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.Admin.Controllers
{
    public class CMSController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly ICMSManager _cmsManager;
        #endregion

        public CMSController(IUserManager userManager, IErrorLogManager errorLogManager, ICMSManager cmsManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _cmsManager = cmsManager;
        }

        #region CMS Page Management

        /// <summary>
        /// This Will be used to get Template List
        /// </summary>
        /// <returns></returns>
        public ActionResult CMSManager()
        {
            ViewBag.SelectedTab = SelectedAdminTab.CMSManager;
            var templates = _cmsManager.GetCMSPageList(PagingModel.DefaultModel());
            return View(templates);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetCMSPagesPagedList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.CMSManager;
            PagingResult<CMSPageViewModel> modal = _cmsManager.GetCMSPageList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_pageListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);

        }


        [HttpGet]
        public ActionResult EditPageContent(int pageId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.CMSManager;
            EditCMSPageModel model = _cmsManager.GetPageContentByPageId(pageId);
            if (model == null)
            {
                model = new EditCMSPageModel();
            }
            return View(model);
        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddUpdatePageContent(EditCMSPageModel model)
        {
            var result = _cmsManager.UpdatePageContent(model);
            return JsonResult(result);
        }
        #endregion
    }
}