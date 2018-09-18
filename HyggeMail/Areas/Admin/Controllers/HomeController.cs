#region Default Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
#endregion

#region Custom Namespaces
using HyggeMail.Attributes;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Interfaces;
#endregion

namespace HyggeMail.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly IEmailTemplateManager _templateManager;
        private readonly ICMSManager _cmsManager;
        private readonly IPaymentManager _paymentManager;
        private readonly ITestimonialManager _testimonialManager;
        #endregion

        public HomeController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager templateManager, ICMSManager cmsManager, IPaymentManager paymentManager, ITestimonialManager testimonialManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _paymentManager = paymentManager;
            _templateManager = templateManager;
            _cmsManager = cmsManager;
            _testimonialManager = testimonialManager;
        }

        /// <summary>
        /// Admin : Login Page
        /// </summary>
        /// <returns></returns>
        [HttpGet, Public]
        public ActionResult Index()
        {
            return View(new LoginModal());
        }

        /// <summary>
        /// This will handle user login request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AjaxOnly, HttpPost, Public]
        public JsonResult Login(LoginModal model)
        {
            //to do: Implement user login
            //var data = _userManager.AdminLogin(model);
            var data = new ActionOutput<UserDetails>();
            var adminLogin = _userManager.AdminLogin(model);
            if (adminLogin != null)
            {
                data.Status = ActionStatus.Successfull;
                data.Object = new UserDetails
                {
                    UserID = adminLogin.UserId,
                    FirstName = model.UserName,
                    UserName = model.UserName,
                    IsAuthenticated = true
                };
            }
            else
            {
                data.Status = ActionStatus.Error;
                data.Message = "Invalid Credentials.";
            }
            if (data.Status == ActionStatus.Successfull)
            {
                var user_data = data.Object;
                CreateCustomAuthorisationCookie(model.UserName, false, new JavaScriptSerializer().Serialize(user_data));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dashboard Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Dashboard()
        {
            var model = _cmsManager.GetAdminDashboardDetails();

            return View(model);
        }

        public ActionResult UserTransactions()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Transactions;
            var data = _paymentManager.GetTransactionPagedList(new PagingModel { PageNo = 1, RecordsPerPage = AppDefaults.PageSize, SortBy = "TransactionDate", SortOrder = "Desc", UserID = 0 });
            return View(data);
        }

        public ActionResult UpdatePassword()
        {
            var model = new ChangePasswordModel();

            return View(model);
        }
        /// <summary>
        /// This will update the password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AjaxOnly, HttpPost, Public]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            model.userID = _userManager.GetAdminDetails().UserId;
            var result = _userManager.UpdateUserPassword(model);
            SignOut();
            return Json(new ActionOutput { Status = result.Status, Message = result.Message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, AjaxOnly]
        public JsonResult GetTransactionsPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Transactions;
            model.UserID = 0;
            var modal = _paymentManager.GetTransactionPagedList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partial/_transactionPartial", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        public ActionResult ViewTransactionDetails(int tid)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Users;
            var data = _paymentManager.GetTransactionDetailsByTransID(tid);
            return View(data);
        }
    }
}