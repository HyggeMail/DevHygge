using HyggeMail.Attributes;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.Admin.Controllers
{
    public class PostCardController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly IEditorManager _editorManager;
        private readonly IEmailTemplateManager _templateManager;
        #endregion

        public PostCardController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager templateManager, IEditorManager editorManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _templateManager = templateManager;
            _editorManager = editorManager;
        }

        #region PostCard Management

        [HttpGet]
        public ActionResult ManagePostCard()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            var users = _editorManager.GetPostCardPagedList(new OrderPagingModel { PageNo = 1, RecordsPerPage = 10, IsOrderPlaced = 1, SortBy = "AddedOn", SortOrder = "Desc" }, 0);
            return View(users);
        }
        [HttpGet]
        public ActionResult PostCardDemo()
        {
            return View();
        }
        [HttpGet]
        public ActionResult PostCardDetails(int id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            var postcard = _editorManager.GetPostCardDetailsByID(id);
            return View(postcard);
        }

        public PartialViewResult aa()
        {
            return PartialView();
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardPagingList(OrderPagingModel model)
        {
            model.IsOrderPlaced = 1;
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            var modal = _editorManager.GetPostCardPagedList(model, 0);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_postcardListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
        [AjaxOnly, HttpPost]
        public JsonResult DeletePostCard(int PostCardId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.DeletePostCardByID(PostCardId));
        }
        [AjaxOnly, HttpPost]
        public JsonResult ApprovePostCard(int PostCardId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.ApprovePostCardByID(PostCardId));
        }


        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public ActionResult PrintPostCard(PostCardFrontBack model)
        {
            var user = _userManager.GetUserDetailsByUserId(model.UserID);
            List<string> resultString = new List<string>();
            model.cardFront = Utilities.ImagePathToBase64(model.cardFront);
            var result = new List<string>();
            var byteArray = new PartialViewAsPdf("Partials/_pdfPostCard", model)
            {
                FileName = string.Format("HyggeMail-PostCard.pdf",
                    LOGGEDIN_USER.FirstName, LOGGEDIN_USER.LastName),
                //PageMargins = new Rotativa.Options.Margins(7, 0, 0, 0),
               // PageHeight = 127,
               // PageWidth = 177.8,
               // PageOrientation = Rotativa.Options.Orientation.Portrait,
               // PageSize = Rotativa.Options.Size.A4

                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 }, // it's in millimeters
                PageWidth = 150, // it's in millimeters
                PageHeight = 200,
                

            }.BuildPdf(this.ControllerContext);

          


            //var path = HttpContext.Server.MapPath("/Uploads/PostCard-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            var fileName = string.Format("HyggeMail-{0}-{1}.pdf", "PostCard", Utilities.GetTimestamp(DateTime.UtcNow));
            var path = Utilities.GetPath(AttacmentsPath.UserProfileImages, fileName);
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();
            result.Add(AttacmentsPath.UserProfileImages.Replace("~/", "../../../") + fileName);
            _userManager.AddHistory(new UserHistoryModel() { UserFK = model.UserID, Type = "Order", Status = "Order Placed", TokenChange = "", AddedOn = DateTime.UtcNow, TokenAvailable = user.Object.CardsCount });
            return Json(new ActionOutput() { Results = result, Status = ActionStatus.Successfull });
        }

        public ActionResult PrintPDF()
        {
            PostCardFrontBack model = new PostCardFrontBack();
            return View(model);
        }

        [AjaxOnly, HttpPost]
        public JsonResult RejectPostCard(int PostCardId)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.RejectPostCardByID(PostCardId));
        }

        [AjaxOnly, HttpPost]
        public JsonResult UpdateOrderStatus(int orderID, short status)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.UpdateOrderStatus(orderID, status));
        }

        [AjaxOnly, HttpPost]
        public JsonResult ApproveReciptent(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.ApproveReceiptent(ReceiptentID));
        }

        [AjaxOnly, HttpPost]
        public JsonResult DisapproveReciptent(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.DispproveReceiptent(ReceiptentID));
        }

        [HttpGet]
        public ActionResult ManageOrders()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            var orders = _editorManager.GetPostCardOrdersPaggedList(new RecipientOrderPagingModel { shownsBydays = eShownRecipientOrdersByDays.Today, shownByStatus = eRecipientOrderStatus.New, PageNo = 1, RecordsPerPage = 10, IsOrderPlaced = 1, SortBy = "AddedOn", SortOrder = "Desc" }, 0);
            return View(orders);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardRecipientPagingList(RecipientOrderPagingModel model)
        {
            // model.IsOrderPlaced = 1;
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            var modal = _editorManager.GetPostCardOrdersPaggedList(model, 0);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_postcardRecipientListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult RejectWithReason(RejectWithReasonModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.RejectWithReason(model));
        }

        [AjaxOnly, HttpPost]
        public JsonResult SentToError(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.SentToError(ReceiptentID));
        }
        [AjaxOnly, HttpPost]
        public JsonResult CompleteRecipientPostCard(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.CompleteRecipientPostCard(ReceiptentID));
        }
        [AjaxOnly, HttpPost]
        public JsonResult ApproveRecipientPostCard(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.ApproveReceiptent(ReceiptentID));
        }

        public ActionResult ViewRecipientOrder(int oid)
        {
            var data = _userManager.ViewRecipientOrder(oid);
            return View(data);
        }
        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardBackSideJsonResult(int ReceiptentID)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Postcard;
            return JsonResult(_editorManager.GetPostCardBackSideJsonResult(ReceiptentID));
        }
        #endregion
    }
}