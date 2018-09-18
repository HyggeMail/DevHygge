using HyggeMail.Attributes;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Web.Areas.User.Controllers;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace HyggeMail.Areas.User.Controllers
{
    public class HomeController : UserAuthorisationController
    {
        #region Variable Declaration
        private readonly IImageManager _imageManager;
        private readonly IRecipientManager _recipientManager;
        private readonly IEditorManager _editorManager;
        private readonly IUserManager _userManager;
        private readonly IErrorLogManager _errorLogManager;
        private readonly IPaymentManager _paymentManager;
        private readonly ICMSManager _cmsManager;
        private readonly IFAQManager _faqManager;

        #endregion

        public HomeController(IErrorLogManager errorLogManager, IImageManager imageManager, IRecipientManager recipientManager, IEditorManager editorManager, IUserManager userManager, IPaymentManager paymentManager, ICMSManager cmsManager, IFAQManager faqManager)
            : base(errorLogManager)
        {
            _imageManager = imageManager;
            _recipientManager = recipientManager;
            _editorManager = editorManager;
            _errorLogManager = errorLogManager;
            _userManager = userManager;
            _paymentManager = paymentManager;
            _cmsManager = cmsManager;
            _faqManager = faqManager;
        }

        //
        // GET: /User/Home/
        public ActionResult Dashboard(int? cid)
        {
            var changeTempUserId = LOGGEDIN_USER.UserID;
            bool IsCopy = false;
            ViewBag.IsCopyCard = false;
            if (Session["CardID"] != null)
            {
                IsCopy = true;
                ViewBag.IsCopyCard = true;
                cid = (int)Session["CardID"];
                Session.Remove("CardID");
            }

            if (Session["DemoCardUserId"] != null)
            {
                var userId = (int)Session["DemoCardUserId"];

                if (userId > 0)
                {
                    changeTempUserId = userId;
                    ViewBag.IsCopyCard = true;
                    Session.Remove("DemoCardUserId");
                }
            }

            var userCards = _userManager.GetUserCardStatus(changeTempUserId);
            ViewBag.StepGuidanceStatus = _userManager.GetUserCardStepGuidance(changeTempUserId);
            if (userCards.Status == ActionStatus.Successfull)
            {
                var UploadModel = new AddUpdateAdminImageModel();
                var editorModel = new AddUpdateImageEditorModel();
                if (cid > 0 && cid != null)
                {
                    var existRecord = _editorManager.GetPostCardDetailsByID(Convert.ToInt32(cid), changeTempUserId);
                    if (existRecord.Status == ActionStatus.Successfull)
                    {
                        if (existRecord.Object.IsOrderPlaced)
                        {
                            //  return RedirectToActionPermanent("PostCards", "PostCard", new { Area = "User" });

                            //cid = null;
                            editorModel = existRecord.Object;
                            ViewBag.IsCopyCard = true;
                        }
                        else
                            editorModel = existRecord.Object;
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                }
                UploadModel.Categories = _imageManager.GetImageCategoryDDLList();
                ViewBag.UploadModel = UploadModel;
                ActionOutput<ImagesByCategoryViewModel> modal = _imageManager.GetImagesByCategoryWise(LOGGEDIN_USER.UserID);
                ViewBag.RecentlyUsedImageList = _imageManager.GetImagesByCategoryWise(LOGGEDIN_USER.UserID, "").List.Where(x => x.IsRecent).ToList();
                ViewBag.OrderID = cid;
                if (cid > 0 && cid != null && IsCopy != true)
                    editorModel.ID = Convert.ToInt32(cid);

                return View(editorModel);
            }
            else
            {
                TempData["CardMessage"] = userCards.Message;
                return RedirectToAction("PaymentPlans", "Payment", new { area = "User" });
            }
        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult DontShowAgain()
        {
            var result = _userManager.DontShowAgain(LOGGEDIN_USER.UserID);
            return JsonResult(result);
        }
        public ActionResult Dashboard1()
        {
            var UploadModel = new AddUpdateAdminImageModel();
            UploadModel.Categories = _imageManager.GetImageCategoryDDLList();
            ViewBag.UploadModel = UploadModel;
            return View();
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddUpdateAdminImage(AddUpdateAdminImageModel model)
        {
            model.ByAdmin = false;
            model.AddedBy = LOGGEDIN_USER.UserID;
            var result = _imageManager.AddUpdateAdminImage(model);
            return JsonResult(result);
        }

        [AjaxOnly, HttpPost]
        public JsonResult ImagesByCategoryModel(string keyword)
        {
            var emoji = _imageManager.GetEmojiFromLocalFolder();
            ActionOutput<ImagesByCategoryViewModel> modal = _imageManager.GetImagesByCategoryWise(LOGGEDIN_USER.UserID, keyword);
            List<string> resultString = new List<string>();
            List<string> emojistring = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_Images", modal));
            resultString.Add(RenderRazorViewToString("Partials/_emojiPartial", emoji));
            return JsonResult(resultString);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetUnSplashImages(string keyword, int pageNumber)
        {
            if (pageNumber == 0)
                pageNumber = 1;

            var model = _imageManager.GetUnSplashImages(pageNumber, keyword);
            List<string> resultString = new List<string>();
            List<string> emojistring = new List<string>();
            if (model != null && model.Object != null)
                resultString.Add(RenderRazorViewToString("Partials/_UnSplashImages", model.Object));
            return JsonResult(resultString);
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddEditImage(AddUpdateAdminImageModel model)
        {
            model.ByAdmin = false;
            model.AddedBy = LOGGEDIN_USER.UserID;
            var result = _imageManager.AddUpdateAdminImage(model);
            return JsonResult(result);
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult DeleteImage(int id)
        {
            var result = _imageManager.DeleteAdminImage(id);
            return JsonResult(result);
        }

        public ActionResult FabricDemo()
        {
            ActionOutput<ImagesByCategoryViewModel> modal = _imageManager.GetImagesByCategoryWise(LOGGEDIN_USER.UserID);
            return View(modal);
        }
        public ActionResult Thankyou(int postCardID)
        {
            var postCard = _editorManager.GetPostCardDetailsByID(LOGGEDIN_USER.UserID, postCardID);
            return View();
        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult PostCardSubmit(AddUpdateImageEditorModel model)
        {
            model.UserID = LOGGEDIN_USER.UserID;
            var result = _editorManager.AddUpdatePostCard(model);
            if (result.Object != null)
            {
                result.Object.AddedOn = result.Object.AddedOn.Value.AddMinutes(15);
            }
            return Json(result);
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public ActionResult PrintPostCard(AddUpdateImageEditorModel model)
        {

            List<string> resultString = new List<string>();
            if (LOGGEDIN_USER.UserType == UserTypes.User)
            {
                var print = new PostCardFrontBack();
                print.cardBack = model.CardBackWithFrame != null ? model.CardBackWithFrame : model.CardBack;
                print.cardFront = model.CardFront;
                var result = new List<string>();
                var byteArray = new PartialViewAsPdf("Partials/_pdfPostCard", print)
                {
                    FileName = string.Format("HyggeMail-PostCard.pdf",
                        LOGGEDIN_USER.FirstName, LOGGEDIN_USER.LastName),
                    //PageMargins = new Rotativa.Options.Margins(7, 0, 0, 0),
                    //PageHeight = 127,
                    //PageWidth = 177.8,
                    //PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.A3,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageMargins = { Left = 0, Right = 0, Top = 11, Bottom = 0 }, // it's in millimeters
                    PageWidth = 150, // it's in millimeters
                    PageHeight = 200,
                }.BuildPdf(this.ControllerContext);
                //var path = HttpContext.Server.MapPath("/Uploads/PostCard-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                var fileName = string.Format("HyggeMail-{0}-{1}.pdf", "PostCard", Utilities.GetTimestamp(DateTime.UtcNow));
                var path = Utilities.GetPath(AttacmentsPath.UserProfileImages, fileName);
                var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();
                result.Add(AttacmentsPath.UserProfileImages.Replace("~/", "../../") + fileName);
                return Json(new ActionOutput() { Results = result, Status = ActionStatus.Successfull });
            }
            return Json(new ActionOutput());
        }


        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult OrderCancel(int postCardOrder)
        {
            var result = _editorManager.CancelPostCardByID(postCardOrder);
            return JsonResult(result);
        }
        public ActionResult MyProfile()
        {
            var user = _userManager.GetUserDetailsByUserId(LOGGEDIN_USER.UserID);
            ViewBag.Membership = _paymentManager.GetTransactionDetailsByUserID(LOGGEDIN_USER.UserID);
            return View(user.Object);
        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult UpdateProfileDetails(UserModel model)
        {
            var result = _userManager.UpdateUserDetails(model);
            var newuser = _userManager.GetUserDetailsByUserId(LOGGEDIN_USER.UserID).Object;
            LOGGEDIN_USER.FirstName = newuser.FirstName;
            LOGGEDIN_USER.LastName = newuser.LastName;
            LOGGEDIN_USER.ImageName = newuser.ImageName;
            LOGGEDIN_USER.ImageLink = newuser.ImagePath;
            UpdateCustomAuthorisationCookie(new JavaScriptSerializer().Serialize(LOGGEDIN_USER));
            return Json(result);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult UpdatePassword(ResetPasswordModel model)
        {
            model.UserId = LOGGEDIN_USER.UserID;
            var result = _userManager.ChangePassword(model);
            return Json(result);
        }
        public ActionResult MySetting()
        {
            var result = _userManager.GetUserNotificationSettings(LOGGEDIN_USER.UserID);
            ViewBag.Membership = _paymentManager.GetTransactionDetailsByUserID(LOGGEDIN_USER.UserID);
            return View(result.Object);

        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult UpdateNotificationSettings(UserNotificationSetting model)
        {
            model.UserID = LOGGEDIN_USER.UserID;
            var result = _userManager.SetUserNotificationSettings(model);
            return Json(result);
        }

        [Public]
        [AllowAnonymous]
        public ActionResult GetUserDashboardForApp(string Token, int cardid)
        {
            var userid = _userManager.GetSessionByToken(Token);
            if (userid > 0)
            {
                var userCards = _userManager.GetUserCardStatus(userid);
                ViewBag.StepGuidanceStatus = _userManager.GetUserCardStepGuidance(userid);
                if (userCards.Status == ActionStatus.Successfull)
                {
                    var UploadModel = new AddUpdateAdminImageModel();
                    var editorModel = new AddUpdateImageEditorModel();
                    if (cardid > 0)
                    {
                        var existRecord = _editorManager.GetPostCardDetailsByID(Convert.ToInt32(cardid), userid);
                        if (existRecord.Status == ActionStatus.Successfull)
                        {
                            if (!existRecord.Object.IsOrderPlaced)
                                editorModel = existRecord.Object;
                        }
                        else
                            return RedirectToAction("GetUserDashboardForApp", "Home", new { Token = Token, cardid = 0 });
                    }
                    UploadModel.Categories = _imageManager.GetImageCategoryDDLList();
                    ViewBag.UploadModel = UploadModel;
                    ActionOutput<ImagesByCategoryViewModel> modal = _imageManager.GetImagesByCategoryWise(userid);
                    ViewBag.RecentlyUsedImageList = _imageManager.GetImagesByCategoryWise(userid, "").List.Where(x => x.IsRecent).ToList();
                    ViewBag.OrderID = cardid;
                    if (cardid > 0)
                        editorModel.ID = Convert.ToInt32(cardid);
                    return View(editorModel);
                }
                else
                {
                    TempData["Msg"] = "Greetings! You have used up all your tokens.Would you like to purchase a membership?";
                    return RedirectToAction("ErrorPage");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid User";
                return RedirectToAction("ErrorPage");
            }
        }


        [Public]
        [AllowAnonymous]
        public ActionResult GetCMSForApp(int PageType)
        {
            var result = new EditCMSPageModel();
            if (PageType == (int)CMSPageType.AboutUs)
                result = _cmsManager.GetPageContentByPageType((int)CMSPageType.AboutUs);
            if (PageType == (int)CMSPageType.TermsAndConditions)
                result = _cmsManager.GetPageContentByPageType((int)CMSPageType.TermsAndConditions);
            if (PageType == (int)CMSPageType.PrivacyPolicy)
                result = _cmsManager.GetPageContentByPageType((int)CMSPageType.PrivacyPolicy);

            return View(result);
        }

        [Public]
        [AllowAnonymous]
        public ActionResult GetFaq()
        {
            ViewBag.Message = "Your contact page.";
            PagingModel mod = new PagingModel();
            mod.RecordsPerPage = 10000;
            mod.PageNo = 1;
            mod.SortBy = "AddedOn";
            mod.SortOrder = "Desc";
            var faqs = _faqManager.GetFAQPagedList(mod, (int)eFAQCategory.FAQ);

            return View(faqs.List);
        }


        [Public]
        [AllowAnonymous]
        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}