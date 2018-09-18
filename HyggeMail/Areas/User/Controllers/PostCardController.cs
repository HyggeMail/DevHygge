using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Web.Areas.User.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HyggeMail.Areas.User.Controllers
{
    public class PostCardController : UserAuthorisationController
    {
        #region Variable Declaration
        private readonly IImageManager _imageManager;
        private readonly IRecipientManager _recipientManager;
        private readonly IEditorManager _editorManager;
        private readonly IErrorLogManager _errorLogManager;
        private readonly IPaymentManager _paymentManager;
        #endregion
        public PostCardController(IErrorLogManager errorLogManager, IImageManager imageManager, IRecipientManager recipientManager, IEditorManager editorManager, IPaymentManager paymentManager)
            : base(errorLogManager)
        {
            _imageManager = imageManager;
            _recipientManager = recipientManager;
            _editorManager = editorManager;
            _errorLogManager = errorLogManager;
            _paymentManager = paymentManager;
        }

        [HttpGet]
        public ActionResult PostCards()
        {
            var cards = _editorManager.GetPostCardPagedList(new OrderPagingModel { PageNo = 1, RecordsPerPage = 12, SortBy = "AddedOn", SortOrder = "Desc" }, LOGGEDIN_USER.UserID);
            ViewBag.Membership = _paymentManager.GetTransactionDetailsByUserID(LOGGEDIN_USER.UserID);
            return View(cards);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardForPopup(int postCardID)
        {
            var modal = _editorManager.GetPostCardDetailsByID(postCardID, LOGGEDIN_USER.UserID);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_postcardDetailPopup", modal.Object));
            return JsonResult(resultString);
        }

        [Public]
        public ActionResult ShowPostCard(int postCardID, int userId)
        {
            var modal = _editorManager.GetPostCardDetailsByID(postCardID, userId);

            if (modal != null)
                return View(modal.Object);
            return View(new AddUpdateImageEditorModel());
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardsList(OrderPagingModel model)
        {
            model.RecordsPerPage = model.RecordsPerPage == 10 ? 12 : model.RecordsPerPage;
            var modal = _editorManager.GetPostCardPagedList(model, LOGGEDIN_USER.UserID);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_postcardListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [HttpGet]
        public ActionResult MyOrders()
        {
            var orders = _editorManager.GetMyPostCardOrdersPaggedList(new RecipientOrderPagingModel { shownsBydays = eShownRecipientOrdersByDays.Today, shownByStatus = eRecipientOrderStatus.New, PageNo = 1, RecordsPerPage = 12, IsOrderPlaced = 1, SortBy = "AddedOn", SortOrder = "Desc" }, LOGGEDIN_USER.UserID);
            ViewBag.Membership = _paymentManager.GetTransactionDetailsByUserID(LOGGEDIN_USER.UserID);
            return View(orders);
        }

        [HttpGet]
        public ActionResult CopyPostCard(int id)
        {
            Session["CardID"] = id;
            return RedirectToAction("Dashboard", "Home", new { area = "user" });
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetMyOrderList(RecipientOrderPagingModel model)
        {
            model.RecordsPerPage = model.RecordsPerPage == 10 ? 12 : model.RecordsPerPage;
            var modal = _editorManager.GetMyPostCardOrdersPaggedList(model, LOGGEDIN_USER.UserID);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_myorders", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult DeletePostCard(int postCardID)
        {
            var result = new ActionOutput();
            //result.Message = "Error";
            //result.Status = ActionStatus.Error;
            result = _editorManager.DeletePostCardByID(postCardID);
            return JsonResult(result);
        }
    }
}