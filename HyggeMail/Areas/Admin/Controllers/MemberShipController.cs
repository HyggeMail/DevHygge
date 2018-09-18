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
    public class MemberShipController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IMembershipManager _membershipManager;
        #endregion

        public MemberShipController(IMembershipManager membershipManager, IErrorLogManager errorLogManager)
            : base(errorLogManager)
        {
            _membershipManager = membershipManager;
        }

        #region membership Management

        public ActionResult AddEditPlan(int? id)
        {
            var model = new MembershipModel();
            ViewBag.SelectedTab = SelectedAdminTab.Membership;
            if (id > 0 && id != null)
                model = _membershipManager.GetPlanDetailsByID(Convert.ToInt32(id));
            return View(model);
        }

        [AjaxOnly, HttpPost]
        public JsonResult AddUpdatePlanDetails(MembershipModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var result = new ActionOutput();
            if (model.ID > 0)
                result = _membershipManager.UpdatePlanDetails(model);
            else
                result = _membershipManager.CreatePlan(model);
            return JsonResult(result);
        }


        [HttpGet]
        public ActionResult Managemembership()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Membership;
            var plans = _membershipManager.GetPlansPagedList(PagingModel.DefaultModel("CreatedOn", "Desc"));
            return View(plans);
        }

        [HttpGet]
        public ActionResult membershipDetails(int id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Membership;
            var membership = _membershipManager.GetPlanDetailsByID(id);
            return View(membership);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetmembershipPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Membership;
            var modal = _membershipManager.GetPlansPagedList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_membershipListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult Deletemembership(int membershipId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Membership;
            return JsonResult(_membershipManager.DeletePlane(membershipId));
        }


        #endregion
    }
}