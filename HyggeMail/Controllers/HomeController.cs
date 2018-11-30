#region Default Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
#endregion

#region Custom Namespaces
using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using Ninject;
using HyggeMail.BLL.Models;
using System.IO;
using HyggeMail.MailChimp;
using HyggeMail.BLL.Common;
using System.Configuration;
#endregion

namespace HyggeMail.Controllers
{
    /// <summary>
    /// Home Controller 
    /// Created On: 10/04/2015
    /// </summary>
    //[Authorize]
    public class HomeController : BaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly ICMSManager _cmsManager;
        private readonly IMembershipManager _membershipManager;
        private readonly IFAQManager _faqManager;
        private readonly ISubscriberManager _subsManager;
        private readonly ITestimonialManager _testimonialManager;
        private readonly IImageManager _imageManager;
        private readonly IEditorManager _editorManager;
        private readonly IRecipientManager _recipientManager;
        private readonly IEmailManager _EmailManager;

        #endregion

        public HomeController(IUserManager userManager, IErrorLogManager errorLogManager, ICMSManager cmsManager, IMembershipManager membershipManager, IFAQManager faqManager, ISubscriberManager subsManager, ITestimonialManager testimonialManager, IImageManager imageManager,
            IEditorManager editorManager, IRecipientManager _recipientManager, IEmailManager _emailManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _cmsManager = cmsManager;
            _membershipManager = membershipManager;
            _faqManager = faqManager;
            _subsManager = subsManager;
            _testimonialManager = testimonialManager;
            _imageManager = imageManager;
            _editorManager = editorManager;
            this._recipientManager = _recipientManager;
            this._EmailManager = _emailManager;
        }

        /// <summary>
        /// Index View 
        /// </summary>
        /// <returns></returns>
        [Public]
        public ActionResult Index()
        {
            // by using this way we can call required methods.
            ViewBag.WelcomeMessage = _userManager.GetWelcomeMessage();
            ViewBag.TestiMonials = _testimonialManager.GetAllTestimonials();
            //ViewBag.PostCardCount = _cmsManager.GetAdminDashboardDetails().Postcards;
            //ViewBag.GetLatestUserPostCard = _imageManager.GetLatestUserPostCard();
            return View();
        }

        /// <summary>
        /// Index View 
        /// </summary>
        /// <returns></returns>
        [Public]
        public ActionResult Index1()
        {
            // by using this way we can call required methods.
            ViewBag.WelcomeMessage = _userManager.GetWelcomeMessage();
            ViewBag.TestiMonials = _testimonialManager.GetAllTestimonials();
            //ViewBag.PostCardCount = _cmsManager.GetAdminDashboardDetails().Postcards;
            //ViewBag.GetLatestUserPostCard = _imageManager.GetLatestUserPostCard();
            return View();
        }

        [HttpPost]
        public JsonResult GetTerms(int spotDealID)
        {
            var data = _cmsManager.GetPageContentByPageId((int)CMSPageType.TermsAndConditions);
            List<string> resultString = new List<string>();

            resultString.Add(RenderRazorViewToString("Partials/_terms", new object()));
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost, Public]
        //  [CaptchaValidator]
        public JsonResult SignUp(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
                return Json(_userManager.SignUpUser(model), JsonRequestBehavior.AllowGet);
            else
                return Json(new ActionOutput() { Message = "Please check all fields.", Status = ActionStatus.Error }, JsonRequestBehavior.AllowGet);
        }

        [Public]
        public ActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordModel();
            if (token != "")
            {
                var user = _userManager.GetUserByForgotToken(token);
                if (user != null)
                {
                    model.UserId = user.UserId;
                    model.type = (int)UserTypes.User;
                }
                else
                    return RedirectToActionPermanent("Index", "Home");
            }
            return View(model);
        }

        [AjaxOnly, HttpPost, Public]
        public JsonResult CountryList()
        {
            var data = _userManager.GetCountries();
            return Json(new ActionOutput<SelectListItem> { List = data, Status = ActionStatus.Successfull, Message = "List" }, JsonRequestBehavior.AllowGet);
        }


        [AjaxOnly, HttpPost, Public]
        public JsonResult StateByCountry(string country)
        {
            if (!string.IsNullOrEmpty(country) && country.All(char.IsDigit))
            {
                var states = _userManager.GetStateByCountry(Convert.ToInt32(country));
                return Json(new ActionOutput<SelectListItem> { List = states, Status = ActionStatus.Successfull, Message = "State List" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new ActionOutput<SelectListItem> { Status = ActionStatus.Error, Message = "State List" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly, HttpPost, Public]
        public JsonResult CityByState(string state)
        {
            if (!string.IsNullOrEmpty(state) && state.All(char.IsDigit))
            {
                var list = _userManager.GetCityByState(Convert.ToInt32(state));
                return Json(new ActionOutput<SelectListItem> { List = list, Status = ActionStatus.Successfull, Message = "City List" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new ActionOutput<SelectListItem> { Status = ActionStatus.Error, Message = "State List" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// About Us Page
        /// </summary>
        /// <returns></returns>
        [Public]
        //  [Authorize]
        public ActionResult About()
        {
            var result = _cmsManager.GetPageContentByPageType((int)CMSPageType.AboutUs);
            return View(result);
        }

        /// <summary>
        /// Contact Us Page
        /// </summary>
        /// <returns></returns>
        [Public]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            var model = new WebContactUsModel();
            return View(model);
        }
        [AjaxOnly, HttpPost, Public]
        public JsonResult SubmitContactUs(WebContactUsModel model)
        {
            var result = _userManager.WebRequestContactUs(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Public]
        public ActionResult Faq()
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

        [AjaxOnly, HttpPost, Public]
        public JsonResult GetFAQByCategory(eFAQCategory cat, string search)
        {
            var modal = _faqManager.GetFAQPagedList(new PagingModel() { Search = search, RecordsPerPage = 1000, SortBy = "UpdatedOn", SortOrder = "Desc" }, (int)cat);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_faqListing", modal.List));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        [Public]
        public ActionResult PaymentPlans()
        {
            if (LOGGEDIN_USER.IsAuthenticated)
            {
                return RedirectToAction("PaymentPlans", "Payment", new { area = "User" });
            }
            var getPlans = _membershipManager.GetPlansPagedList(new PagingModel() { RecordsPerPage = 100, SortBy = "ID", SortOrder = "Asc" });
            return View(getPlans);
        }

        [Public]
        public ActionResult Privacy()
        {
            var result = _cmsManager.GetPageContentByPageType((int)CMSPageType.PrivacyPolicy);
            return View(result);
        }
        [Public]
        public ActionResult Terms()
        {
            var result = _cmsManager.GetPageContentByPageType((int)CMSPageType.TermsAndConditions);
            return View(result);
        }
        [Public]
        public ActionResult MobileApp()
        {
            var result = _cmsManager.GetPageContentByPageType((int)CMSPageType.MobileApp);
            return View(result);
        }

        [AjaxOnly, HttpPost, Public]
        public JsonResult SubmitSubscriber(SubscriberModel model)
        {
            return Json(_subsManager.SubmitSubscriberEmail(model), JsonRequestBehavior.AllowGet);
        }


        [AjaxOnly, HttpPost, Public]
        public JsonResult GetGuide(GetGuideModel model)
        {
            return Json(_EmailManager.GetTheGuideEmail(model), JsonRequestBehavior.AllowGet);
        }

        [Public]
        public virtual FileResult DownloadGuide()
        {
            string fullPath = Path.Combine(Server.MapPath("~/Uploads/Guide/"), "HyggeMail.pdf");
            return File(fullPath, "application/pdf", "HyggeMailGuide");
        }

        //[Public]
        //public ActionResult JoinProgramme(string q)
        //{
        //    try
        //    {
        //        var mail = Utilities.DecodeString(q);
        //        var result = MailChimpService.AddOrUpdateListMember(subscriberEmail: mail, listId: ConfigurationManager.AppSettings["DownListId"]);
        //        ViewBag.result = result;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.result = ex.Message;
        //        return View();
        //    }
        //}


        [AjaxOnly, HttpPost, Public]
        public JsonResult GetDemoPostCardListing()
        {
            var modal = _editorManager.GetDemoPostCardListing();

            if (modal.Status == ActionStatus.Successfull && modal.List != null)
            {
                List<string> resultString = new List<string>();
                resultString.Add(RenderRazorViewToString("Partials/_demoPostCardsListing", modal.List));
                return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "", Results = resultString }, JsonRequestBehavior.AllowGet);
            }
            return Json(new ActionOutput { Status = ActionStatus.Successfull, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDemoCard(int cid, int userId)
        {
            Session["DemoCardUserId"] = userId;
            return RedirectToAction("Dashboard", "Home", new { area = "User", cid = cid });
        }

        [Public]
        public ActionResult RejectionReason(int id)
        {
            ViewBag.Reason = _recipientManager.GetRejectionReasonByCardId(id);
            return View();
        }

    }

}