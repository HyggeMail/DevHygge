using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Controllers;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HuggeMail.Controllers
{
    public class AccountController : BaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly ICMSManager _cmsManager;
        private readonly IEmailManager _emailManager;
        private readonly IErrorLogManager _errorManager;
        #endregion
        public const int RecordsPerPage = 12;

        public AccountController(IUserManager userManager, IErrorLogManager errorLogManager, ICMSManager cmsManager, IEmailManager emailManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _cmsManager = cmsManager;
            ViewBag.RecordsPerPage = RecordsPerPage;
            _emailManager = emailManager;
            _errorManager = errorLogManager;
        }
        //
        // GET: /Account/
        public ActionResult Register()
        {
            return View();
        }

        //
        // GET: /Account/
        public ActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// This will handle Vendor login request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AjaxOnly, HttpPost, Public]
        public JsonResult UserLogin(LoginModal model)
        {
            //to do: Implement user login
            //var data = _userManager.AdminLogin(model);
            var user = _userManager.UserLogin(model);
            var data = new ActionOutput<UserDetails>();
            BindLoginDetails(ref data, ref user, ref model);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void BindLoginDetails(ref ActionOutput<UserDetails> data, ref UserModel user, ref LoginModal model)
        {
            if (user != null)
            {
                if (user.IsActivated == true)
                {
                    data.Status = ActionStatus.Successfull;
                    data.Object = new UserDetails
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserEmail = user.Email,
                        ImageLink = user.ImagePath,
                        UserName = user.Email,
                        IsAuthenticated = true,
                        UserID = user.UserId,
                        //   UserImage = user.Image,
                        UserType = UserTypes.User,
                        // LastUpdated = user.LastUpdated
                    };
                }
                else
                {
                    data.Status = ActionStatus.Error;
                    data.Message = "Registration is not Complete. Hygge Mail has sent you a verification email with a link. Please click that link to complete your registration.";
                }
            }
            else
            {
                data.Status = ActionStatus.Error;
                data.Message = "Your HyggeMail username or password was entered incorrectly.Please reset your Password. Don’t have a HyggeMail account? Create One.";
            }
            if (data.Status == ActionStatus.Successfull)
            {
                var user_data = data.Object;
                //if (model.RememberMe == true)
                //{
                //    SetRemember(model);
                //}
                CreateCustomAuthorisationCookie(model.UserName, model.RememberMe, new JavaScriptSerializer().Serialize(user_data));
            }
        }

        /// <summary>
        /// Email Varification
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Public]
        public ActionResult EmailVerification(string token)
        {
            if (token != "")
            {
                var user = _userManager.GetUserByToken(token);
                if (user != null)
                {
                    if (user.IsActivated != true)
                    {
                        var Verify = _userManager.SetUserActive(user.UserId);
                        if (Verify != "")
                            ViewBag.Success = Verify;
                    }
                    else
                        ViewBag.Success = "Your Account already verified";

                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// This will handle user forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AjaxOnly, HttpPost, Public]
        public JsonResult ForgetPassword(ForgetPasswordModel model)
        {
            var data = new ActionOutput();
            var user = _userManager.GetUserByEmail(model.UserEmail);
            if (user != null)
            {
                var resetPassword = _userManager.ResetPassword(user);
                if (resetPassword == true)
                {
                    data.Message = "Please check your email";
                    data.Status = ActionStatus.Successfull;
                }
                else
                {
                    data.Message = "Email sent request failed.";
                    data.Status = ActionStatus.Error;
                }
            }
            else
            {
                data.Message = "This email doesnt exist";
                data.Status = ActionStatus.Error;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Post : Reset Password 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [AjaxOnly, HttpPost, Public]
        public ActionResult SubmitResetPassword(ResetPasswordModel model)
        {
            var data = new ActionOutput();

            if (model.UserId > 0 && model.type == (int)UserTypes.User)
            {
                var result = _userManager.ChangePassword(model);
                if (result.Status == ActionStatus.Successfull)
                {
                    data.Message = "Password reset successfully"; data.Status = ActionStatus.Successfull;
                }
                else
                {
                    data.Message = "Error occured"; data.Status = ActionStatus.Error;
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            data.Message = "Unexpected error";
            data.Status = ActionStatus.Error;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [Public]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                //var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                //if (UserId != null)
                //{
                //    properties.Dictionary[XsrfKey] = UserId;
                //}
                //context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}
        ////
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            //if (loginInfo == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //Session["UserName"] = loginInfo.DefaultUserName;
            //Session["Email"] = loginInfo.Email;
            //Session["Name"] = loginInfo.ExternalIdentity.Name;
            //ViewBag.ReturnUrl = returnUrl;
            //Session["Provider"] = loginInfo.Login.LoginProvider;
            //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            //Session["ExtUsrID"] = loginInfo.Login.ProviderKey;
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = "", Password = "" });


        }
        #region FaceBook Graph


        [Public]
        public ActionResult FbSignIn()
        {
            string app_id = Convert.ToString(ConfigurationManager.AppSettings["Client_Id"]);
            string app_secret = Convert.ToString(ConfigurationManager.AppSettings["FacebookAppSecret"]);
            string scope = Convert.ToString(ConfigurationManager.AppSettings["App_Scope"]);
            string RedirectUrl = Convert.ToString(ConfigurationManager.AppSettings["RedirectUrlFB"]);

            return Redirect(string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, RedirectUrl, scope));
        }
        [Public]
        private FacebookUserDetails GetDetails(string AccessToken)
        {
            FacebookUserDetails details = new FacebookUserDetails();
            Uri eatTargetUri = new Uri("https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + ConfigurationManager.AppSettings["Client_Id"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "&fb_exchange_token=" + AccessToken);
            HttpWebRequest eat = (HttpWebRequest)HttpWebRequest.Create(eatTargetUri);

            StreamReader eatStr = new StreamReader(eat.GetResponse().GetResponseStream());
            string eatToken = eatStr.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] eatWords = eatToken.Split('&');
            string extendedAccessToken = eatWords[0];

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,email,birthday,relationship_status,locale,link&access_token=" + AccessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;

            dynamic stuff = JObject.Parse(jsondata);

            details.Name = stuff.first_name + " " + stuff.last_name;
            details.Gender = stuff.gender;
            details.ID = stuff.id;
            details.BirthDay = stuff.birthday;
            details.Email = stuff.email;
            details.Location = stuff.location;
            /*You can get other dynamic variables*/
            return details;
        }
        [Public]
        public ActionResult RedirectHandler()
        {
            string app_id = Convert.ToString(ConfigurationManager.AppSettings["Client_Id"]);
            string app_secret = Convert.ToString(ConfigurationManager.AppSettings["FacebookAppSecret"]);
            string scope = Convert.ToString(ConfigurationManager.AppSettings["App_Scope"]);
            string AccessCode = Convert.ToString(Request["code"]);
            string access_token = string.Empty;
            string RedirectUrl = Convert.ToString(ConfigurationManager.AppSettings["RedirectUrlFB"]);
            if (!string.IsNullOrEmpty(AccessCode))
            {
                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}",
                    app_id, RedirectUrl, scope, AccessCode, app_secret);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();

                    JavaScriptSerializer sr = new JavaScriptSerializer();
                    string jsondata = jsonResponse;

                    dynamic DynamicData = JObject.Parse(jsondata);

                    access_token = DynamicData.access_token;


                }


                FacebookUserDetails user = GetDetails(access_token);

                ActionOutput<apiUserDetailShort> result = _userManager.FaceBookAuthentication(user);
                if (result.Status == ActionStatus.Successfull && result.Message != "Already Registered.")
                {
                    var auth = _userManager.LoginWithFaceBook(user);
                    CreateCustomAuthorisationCookie(auth.Email, false, new JavaScriptSerializer().Serialize(new UserDetails(auth)));
                    return RedirectToActionPermanent("MyProfile", "Home", new { area = "user" });
                }
                else
                {
                    if (result.Message == "Already Registered.")
                    {
                        var existUserDetails = _userManager.GetUserDetailsByUserId(result.Object.UserId).Object;
                        CreateCustomAuthorisationCookie(existUserDetails.Email, false, new JavaScriptSerializer().Serialize(new UserDetails(existUserDetails)));
                        if (existUserDetails.Address == null && (existUserDetails.CountryID == null || string.IsNullOrEmpty(existUserDetails.CountryID)))
                        {
                            return RedirectToActionPermanent("MyProfile", "Home", new { area = "user" });
                        }
                        return RedirectToActionPermanent("Dashboard", "Home", new { area = "user" });
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }
        #endregion
    }
}