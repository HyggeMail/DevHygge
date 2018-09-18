using HyggeMail.Attributes;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Web.Areas.User.Controllers;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.User.Controllers
{
    public class PaymentController : UserAuthorisationController
    {
        private readonly IPaymentManager _paymentManager;
        private readonly IMembershipManager _membershipManager;
        public PaymentController(IErrorLogManager errorLogManager, IPaymentManager paymentManager, IMembershipManager membershipManager)
            : base(errorLogManager)
        {
            _paymentManager = paymentManager;
            _membershipManager = membershipManager;
        }

        public ActionResult PaymentWithPaypal(int? id)
        {
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    Session["PlanID"] = id;
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/User/Payment/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    string curl = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Convert.ToString(ConfigurationManager.AppSettings["PayPalCancelAction"]);
                    string rurl = baseURI + "guid=" + guid;
                    PayPalModel model = new PayPalModel();
                    model.Id = Guid.NewGuid();
                    model.PlanID = id ?? 0;
                    model.CancelUrl = curl;
                    model.ReturenUrl = rurl;
                    model.ApiContext = apiContext;
                    model.Currency = "USD";
                    var createdPayment = _paymentManager.MakePaymetWithPaypal(model);
                    if (createdPayment != null)
                    {
                        var links = createdPayment.links.GetEnumerator();
                        string paypalRedirectUrl = null;
                        while (links.MoveNext())
                        {
                            Links lnk = links.Current;
                            if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                paypalRedirectUrl = lnk.href;
                            }
                        }
                        Session.Add(guid, createdPayment.id);
                        return Redirect(paypalRedirectUrl);
                    }
                    else
                    {
                        ViewBag.Message = "Some Error Occured during payment. Please try again or contact Site Administrator";
                        return View("FailureView");
                    }

                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        TempData["TransactionResult"] = "Some Error Occured during payment. Please try again or contact Site Administrator";
                        return RedirectToAction("PostPaymentFailure");
                    }
                    else
                    {
                        var OriginalPrice = Convert.ToDecimal(Session["PayPalAmount"]);
                        decimal amt = Convert.ToDecimal(executedPayment.transactions[0].related_resources[0].sale.amount.total);
                        var rate = OriginalPrice > 0 ? OriginalPrice : amt;
                        var planID = Convert.ToInt32(Session["PlanID"]);
                        var message = _paymentManager.SavePaypalTransaction(executedPayment, planID, "Paypal", rate, LOGGEDIN_USER.UserID);
                        TempData["PaymentID"] = executedPayment.id;
                        TempData["Message"] =  message.Message;
                        TempData["AvailableTokens"] = message.AvailableTokens;
                        return RedirectToAction("PostPaymentSuccess", "Payment", new {area="user" });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["TransactionResult"] = "Some Error Occured during payment. Please try again or contact Site Administrator";
                return RedirectToAction("PostPaymentFailure", "Payment", new { area = "user" });                
            }


        }


        public ActionResult FailureView()
        {
            return View();
        }

        public ActionResult PostPaymentSuccess()
        {
            return View();
        }

        public ActionResult PostPaymentFailure()
        {
            return View();
        }

        public ActionResult CancelPayPalTransaction()
        {
            var token = Request.QueryString["token"];
            ViewBag.Token = token;
            return View();
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            Payment payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
        [HttpGet]
        public ActionResult PaymentPlans()
        {
            var getPlans = _membershipManager.GetPlansPagedList(new PagingModel() { RecordsPerPage = 100, SortBy = "ID", SortOrder = "Asc" });
            return View(getPlans);
        }

        public ActionResult TransactionHistory()
        {
            var userID = LOGGEDIN_USER.UserID;
            var data = _paymentManager.GetTransactionPagedList(new PagingModel { PageNo = 1, RecordsPerPage = AppDefaults.PageSize, SortBy = "TransactionDate", SortOrder = "Desc",UserID=userID });
            return View(data);
        }

        [HttpPost, AjaxOnly]
        public JsonResult GetTransactionsPagingList(PagingModel model)
        {
            model.UserID = LOGGEDIN_USER.UserID;
            model.SortBy = "TransactionDate";
            model.SortOrder = "Desc";
            var modal = _paymentManager.GetTransactionPagedList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partial/_transactionPartial", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }
    }
}