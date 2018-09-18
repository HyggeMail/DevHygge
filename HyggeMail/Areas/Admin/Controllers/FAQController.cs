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
    public class FAQController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IFAQManager _faqManager;

        #endregion

        public FAQController(IFAQManager faqManager, IErrorLogManager errorLogManager)
            : base(errorLogManager)
        {
            _faqManager = faqManager;
        }

        #region faq Management

        public ActionResult AddEditFAQ(int? id)
        {
            var model = new FAQModel();
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            if (id > 0 && id != null)
                model = _faqManager.GetFAQDetailsByID(Convert.ToInt32(id));
            return View(model);
        }

        [AjaxOnly, HttpPost]
        public JsonResult AddUpdatefaqDetails(FAQModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            var result = new ActionOutput();
            if (model.ID > 0)
                result = _faqManager.UpdateFAQDetails(model);
            else
                result = _faqManager.CreateFAQ(model);
            return JsonResult(result);
        }


        [HttpGet]
        public ActionResult ManageFAQ()
        {
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            var faqs = _faqManager.GetFAQPagedList(PagingModel.DefaultModel("AddedOn", "Desc"), 0);
            return View(faqs);
        }

        [HttpGet]
        public ActionResult faqDetails(int id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            var faq = _faqManager.GetFAQDetailsByID(id);
            return View(faq);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetfaqPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            var modal = _faqManager.GetFAQPagedList(model, 0);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_faqListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult Deletefaq(int faqId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.FAQ;
            return JsonResult(_faqManager.DeleteFAQ(faqId));
        }
        #endregion
    }
}