using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using HyggeMail.BLL;
using HyggeMail.BLL.Managers;

namespace HyggeMail.Web.Areas.User.Controllers
{
    public class UserAuthorisationController : BaseController
    {
        private readonly IErrorLogManager _errorLogManager;
        static readonly IUserManager _UserManager;

        static UserAuthorisationController()
        {
            _UserManager = new UserManager();
        }

        public UserAuthorisationController(IErrorLogManager errorLogManager)
            : base(errorLogManager)
        {
            _errorLogManager = errorLogManager;
        }


        /// <summary>
        /// This will be used to check user authorization
        /// </summary>
        /// <param name="filter_context"></param>
        protected override void OnAuthorization(AuthorizationContext filter_context)
        {
            HttpCookie auth_cookie = Request.Cookies[Cookies.AuthorizationCookie];
            HttpCookie admin_auth_cookie = Request.Cookies[Cookies.AdminAuthorizationCookie];
            HttpCookie auth_cookie_ud = Request.Cookies[Cookies.AuthorizationCookieMobile];
            var requestString = Convert.ToString(filter_context.HttpContext.Request.QueryString);

            if (auth_cookie_ud != null)
            {
                FormsAuthenticationTicket auth_ticket = FormsAuthentication.Decrypt(auth_cookie_ud.Value);
                LOGGEDIN_USER = new JavaScriptSerializer().Deserialize<UserDetails>(auth_ticket.UserData);
                return;
            }

            #region If auth cookie is present
            if (auth_cookie != null)
            {
                #region If Logged User is null
                if (LOGGEDIN_USER == null)
                {
                    try
                    {
                        FormsAuthenticationTicket auth_ticket = FormsAuthentication.Decrypt(auth_cookie.Value);
                        LOGGEDIN_USER = new JavaScriptSerializer().Deserialize<UserDetails>(auth_ticket.UserData);
                        System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(auth_ticket), null);
                    }
                    catch (Exception exc)
                    {
                        if (auth_cookie != null)
                        {
                            auth_cookie.Expires = DateTime.Now.AddDays(-30);
                            Response.Cookies.Add(auth_cookie);
                            filter_context.Result = RedirectToAction("index", "home");
                            base.LogExceptionToDatabase(exc);
                        }

                    }
                }
                if ((filter_context.ActionDescriptor.ActionName == "Index" || filter_context.ActionDescriptor.ActionName == "SignUp") && filter_context.ActionDescriptor.ControllerDescriptor.ControllerName == "Home")
                {
                    filter_context.Result = RedirectToAction("Dashboard", "home", new { area = "user" });
                }
                #endregion

                ViewBag.LOGGEDIN_USER = LOGGEDIN_USER;
            }
            #endregion


            else if (requestString != null && requestString.Contains("Token"))
            {
                var queryString = filter_context.HttpContext.Request.QueryString.ToString();
                var splitQuery = queryString.Split('&');
                if (splitQuery != null && splitQuery.Count() > 1)
                {

                    var token = splitQuery[0].ToString().Split('=')[1].ToString();
                    var userid = _UserManager.GetSessionByToken(token);
                    var user = _UserManager.GetUserById(userid);
                    if (user != null)
                    {
                        var data = new UserDetails
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
                        CreateCustomAuthorisationCookieForMobile(user.FirstName + " " + user.LastName, false, new JavaScriptSerializer().Serialize(data));
                        HttpCookie auth_cookie_udmob = Request.Cookies[Cookies.AuthorizationCookieMobile];
                        FormsAuthenticationTicket auth_ticket = FormsAuthentication.Decrypt(auth_cookie_udmob.Value);
                        LOGGEDIN_USER = new JavaScriptSerializer().Deserialize<UserDetails>(auth_ticket.UserData);
                    }
                }
            }

            #region if authorization cookie is not present and the action method being called is not marked with the [Public] attribute
            else if (!filter_context.ActionDescriptor.GetCustomAttributes(typeof(Public), false).Any())
            {
                if (!Request.IsAjaxRequest()) filter_context.Result = RedirectToAction("index", "home", new { returnUrl = Server.UrlEncode(Request.RawUrl) });
                else filter_context.Result = Json(new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Authentication Error"
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region if authorization cookie is not present and the action method being called is marked with the [Public] attribute
            else
            {
                LOGGEDIN_USER = new UserDetails { IsAuthenticated = false };
                ViewBag.LOGGEDIN_USER = LOGGEDIN_USER;
            }

            if (filter_context.ActionDescriptor.GetCustomAttributes(typeof(Public), false).Any()) { }
            else
            {
                if (LOGGEDIN_USER != null && LOGGEDIN_USER.IsAuthenticated == false)
                {
                    filter_context.Result = RedirectToAction("Index", "Login", new { area = "" });
                }
            #endregion
                if (LOGGEDIN_USER == null || LOGGEDIN_USER.UserType != UserTypes.User && !Request.IsAjaxRequest())
                {
                    if (filter_context.ActionDescriptor.ActionName.ToLower() == "Home")
                    {
                        TempData["returnUrl"] = Server.UrlEncode(Request.RawUrl);
                        filter_context.Result = RedirectToAction("Index", "Home", new { area = "", returnUrl = Request.RawUrl });
                    }
                    else
                    {
                        filter_context.Result = RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }


            base.SetActionName(filter_context.ActionDescriptor.ActionName, filter_context.ActionDescriptor.ControllerDescriptor.ControllerName);

        }

        /// <summary>
        /// used to update user authorization cookie after login
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="is_persistent"></param>
        /// <param name="custom_data"></param>
        protected virtual void UpdateCustomAuthorisationCookie(String custom_data)
        {
            var cookie = Request.Cookies[Cookies.AuthorizationCookie];
            FormsAuthenticationTicket authTicketExt = FormsAuthentication.Decrypt(cookie.Value);

            FormsAuthenticationTicket auth_ticket =
            new FormsAuthenticationTicket(
                1, authTicketExt.Name,
                authTicketExt.IssueDate,
                authTicketExt.Expiration,
                authTicketExt.IsPersistent, custom_data, String.Empty
            );
            String encryptedTicket = FormsAuthentication.Encrypt(auth_ticket);
            cookie = new HttpCookie(Cookies.AuthorizationCookie, encryptedTicket);
            if (authTicketExt.IsPersistent) cookie.Expires = auth_ticket.Expiration;
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }


        /// <summary>
        /// Will be used to logout from the application
        /// </summary>
        /// <returns></returns>
        [HttpGet, Public]
        public virtual ActionResult SignOut()
        {
            HttpCookie auth_cookie = Request.Cookies[Cookies.AuthorizationCookie];
            if (auth_cookie != null)
            {
                Request.Cookies.Remove("HyggeMail");
                auth_cookie.Expires = DateTime.Now.AddDays(-30);
                auth_cookie.Value = null;
                Response.SetCookie(auth_cookie);
                Response.Cookies.Add(auth_cookie);
            }

            return Redirect(Url.Action("Index", "Home", new { area = "" }));
        }
    }
}