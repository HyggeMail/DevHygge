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
    public class RecipientController : UserAuthorisationController
    {
        #region Variable Declaration
        private readonly IImageManager _imageManager;
        private readonly IRecipientManager _recipientManager;
        private readonly IPaymentManager _paymentManager;
        #endregion

        public RecipientController(IErrorLogManager errorLogManager, IImageManager imageManager, IRecipientManager recipientManager, IPaymentManager paymentManager)
            : base(errorLogManager)
        {
            _imageManager = imageManager;
            _recipientManager = recipientManager;
            _paymentManager = paymentManager;
        }


        [HttpGet]
        public ActionResult MyRecipient()
        {
            ActionOutput<RecipientGroupModel> modal = _recipientManager.GetRecipientByAlphabetic(LOGGEDIN_USER.UserID);
            ViewBag.Membership = _paymentManager.GetTransactionDetailsByUserID(LOGGEDIN_USER.UserID);
            return View(modal);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardsList(PagingModel model)
        {
            var modal = _recipientManager.GetRecipientPagedList(model, LOGGEDIN_USER.UserID);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_recipientPagedList", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }


        //
        [AjaxOnly, HttpPost]
        public JsonResult DeleteRecipientByID(int id)
        {
            var result = _recipientManager.DeleteRecipientByID(LOGGEDIN_USER.UserID, id);
            return JsonResult(result);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetRecipientByID(int id)
        {
            ActionOutput<AddUpdateRecipientModel> modal = new ActionOutput<AddUpdateRecipientModel>();
            modal.Object = new AddUpdateRecipientModel();
            if (id > 0)
            {
                modal = _recipientManager.GetRecipientByID(LOGGEDIN_USER.UserID, id);
            }

            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_AddRecipient", modal.Object));
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetUserRecipients(string keyword)
        {
            ActionOutput<RecipientGroupModel> modal = _recipientManager.GetRecipientByAlphabetic(LOGGEDIN_USER.UserID, keyword);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_RecipientListing", modal));
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetPostCardUserRecipients(int cid, string keyword)
        {
            ActionOutput<RecipientGroupModel> modal = _recipientManager.GetPostCardRecipientByAlphabetic(cid, LOGGEDIN_USER.UserID, keyword, false);
            List<string> resultString = new List<string>();
            var reciptentModel = modal.List.ToList();
            resultString.Add(RenderRazorViewToString("Partials/_RecipientListing", reciptentModel));
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetAdminAddressBook(int cid, string keyword)
        {
            ActionOutput<RecipientGroupModel> modal = _recipientManager.GetPostCardRecipientByAlphabetic(cid, LOGGEDIN_USER.UserID, keyword, true);
            List<string> resultString = new List<string>();
            var reciptentModel = modal.List.ToList();
            resultString.Add(RenderRazorViewToString("Partials/_adminAddressBookListing", reciptentModel));
            return JsonResult(resultString);
        }


        [AjaxOnly, HttpPost]
        public JsonResult GetRecipientList(string keyword)
        {
            ActionOutput<RecipientGroupModel> modal = _recipientManager.GetRecipientByAlphabetic(LOGGEDIN_USER.UserID, keyword);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_addressbookList", modal));
            return JsonResult(resultString);
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddEditRecipient(AddUpdateRecipientModel model)
        {
            model.UserID = LOGGEDIN_USER.UserID;
            var result = _recipientManager.AddUpdateRecipient(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}