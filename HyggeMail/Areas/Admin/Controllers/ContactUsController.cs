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
    public class ContactUsController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IContactUsManager _ContactUsManager;
        private readonly IEditorManager _editorManager;
        private readonly IEmailTemplateManager _templateManager;
        #endregion

        public ContactUsController(IContactUsManager ContactUsManager, IErrorLogManager errorLogManager, IEmailTemplateManager templateManager, IEditorManager editorManager)
            : base(errorLogManager)
        {
            _ContactUsManager = ContactUsManager;
            _templateManager = templateManager;
            _editorManager = editorManager;
        }

        #region ContactUs Management

        [HttpGet]
        public ActionResult ManageContactUs()
        {
            var r = _ContactUsManager.SetSeenAllRecord();
            ViewBag.SelectedTab = SelectedAdminTab.ContactUs;
            var ContactUss = _ContactUsManager.GetContactUsPagedList(PagingModel.DefaultModel("AddedOn", "Desc"), 0);
            return View(ContactUss);
        }
        public int GetUnseenContacts()
        {
            var result = _ContactUsManager.GetUnseenContactUsCount();
            return result.AvailableTokens;
        }
        [AjaxOnly, HttpPost]
        public JsonResult GetContactUsPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.ContactUs;
            var modal = _ContactUsManager.GetContactUsPagedList(model, 0);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_contactListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult DeleteContactUs(int ContactUsId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.ContactUs;
            return JsonResult(_ContactUsManager.DeleteContactUsByID(ContactUsId));
        }
        [AjaxOnly, HttpPost]
        public JsonResult ResolvedContactUs(int ContactUsId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.ContactUs;
            return JsonResult(_ContactUsManager.ResolveContactUsByID(ContactUsId));
        }

        [HttpGet]
        public ActionResult ViewMessage(int id)
        {
            var msg = _ContactUsManager.GetContactUsByID(id);
            ViewBag.SelectedTab = SelectedAdminTab.ContactUs;
            return View(msg.Object);
        }

        #endregion
    }
}