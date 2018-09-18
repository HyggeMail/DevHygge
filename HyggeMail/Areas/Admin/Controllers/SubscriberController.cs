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
    public class SubscriberController : AdminBaseController
    {
        #region Variable Declaration
        private readonly ISubscriberManager _SubscriberManager;

        #endregion

        public SubscriberController(ISubscriberManager SubscriberManager, IErrorLogManager errorLogManager)
            : base(errorLogManager)
        {
            _SubscriberManager = SubscriberManager;
        }

        #region Subscriber Management

        //public ActionResult AddEditSubscriber(int? id)
        //{
        //    var model = new SubscriberModel();
        //    ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
        //    if (id > 0 && id != null)
        //        model = _SubscriberManager.GetSubscriberDetailsByID(Convert.ToInt32(id));
        //    return View(model);
        //}

        //[AjaxOnly, HttpPost]
        //public JsonResult AddUpdateSubscriberDetails(SubscriberModel model)
        //{
        //    ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
        //    var result = new ActionOutput();
        //    if (model.ID > 0)
        //        result = _SubscriberManager.UpdateSubscriberDetails(model);
        //    else
        //        result = _SubscriberManager.CreateSubscriber(model);
        //    return JsonResult(result);
        //}


        [HttpGet]
        public ActionResult ManageSubscriber()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
            var Subscribers = _SubscriberManager.GetSubscriberPagedList(PagingModel.DefaultModel("AddedOn", "Desc"), 0);
            return View(Subscribers);
        }

        [HttpGet]
        public ActionResult SubscriberDetails(int id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
            var Subscriber = _SubscriberManager.GetSubscriberDetailsByID(id);
            return View(Subscriber);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetSubscriberPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
            var modal = _SubscriberManager.GetSubscriberPagedList(model, 0);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_SubscriberListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult DeleteSubscriber(int SubscriberId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Subscriber;
            return JsonResult(_SubscriberManager.DeleteSubscriber(SubscriberId));
        }
        #endregion
    }
}