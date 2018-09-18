#region Default Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
#endregion

#region Custom Namespaces
using HyggeMail.Attributes;
using HyggeMail.Controllers;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Interfaces;
#endregion

namespace HyggeMail.Areas.Admin.Controllers
{
    /// <summary>
    /// This controller will work as a base controller for the admin section of the application
    /// </summary>
    [NoCache]
    public class AdminBaseController : BaseController
    {
        
        public AdminBaseController(IErrorLogManager errorLogManager)
            : base(errorLogManager)
        { 
        }

        /// <summary>
        /// This will be used to chek admin user authorization
        /// </summary>
        /// <param name="filter_context"></param>
        protected override void OnAuthorization(AuthorizationContext filter_context)
        {
            HttpCookie auth_cookie = Request.Cookies[Cookies.AdminAuthorizationCookie];

            #region If auth cookie is present
            if (auth_cookie != null)
            {
                #region If LoggedInUser is null
                if (LOGGEDIN_USER == null)
                {
                    FormsAuthenticationTicket auth_ticket = FormsAuthentication.Decrypt(auth_cookie.Value);
                    LOGGEDIN_USER = new JavaScriptSerializer().Deserialize<UserDetails>(auth_ticket.UserData);

                    System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(auth_ticket), null);
                }
                #endregion

                ViewBag.LOGGEDIN_USER = LOGGEDIN_USER;
            }
            #endregion

            #region if authorization cookie is not present and the action method being called is not marked with the [Public] attribute
            else if (!filter_context.ActionDescriptor.GetCustomAttributes(typeof(Public), false).Any())
            {
                if (!Request.IsAjaxRequest()) filter_context.Result = RedirectToAction("Index", "Home", new { returnUrl = Server.UrlEncode(Request.RawUrl), area = "Admin" });
                else filter_context.Result = Json(new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Authentication Error"
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            if (auth_cookie != null)
            {
                #region If Logged User is null
                if (LOGGEDIN_USER == null)
                {
                    FormsAuthenticationTicket auth_ticket = FormsAuthentication.Decrypt(auth_cookie.Value);
                    LOGGEDIN_USER = new JavaScriptSerializer().Deserialize<UserDetails>(auth_ticket.UserData);
                    System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(auth_ticket), null);
                }
                if (filter_context.ActionDescriptor.ActionName == "Index" && filter_context.ActionDescriptor.ControllerDescriptor.ControllerName == "Home")
                {
                    filter_context.Result = RedirectToAction("dashboard", "home", new { area = "Admin" });
                }
                #endregion
                ViewBag.LOGGEDIN_USER = LOGGEDIN_USER;
            }
            #region if authorization cookie is not present and the action method being called is not marked with the [Public] attribute
            else if (!filter_context.ActionDescriptor.GetCustomAttributes(typeof(Public), false).Any())
            {
                if (!Request.IsAjaxRequest()) filter_context.Result = RedirectToAction("index", "home", new { returnUrl = Server.UrlEncode(Request.RawUrl), area = "Admin" });
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
            #endregion

            SetActionName(filter_context.ActionDescriptor.ActionName, filter_context.ActionDescriptor.ControllerDescriptor.ControllerName);
        }

        /// <summary>
        /// this will be used to create admin user authentication cookie after login
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="is_persistent"></param>
        /// <param name="custom_data"></param>
        protected override void CreateCustomAuthorisationCookie(String user_name, Boolean is_persistent, String custom_data)
        {
            FormsAuthenticationTicket auth_ticket =
                new FormsAuthenticationTicket(
                    1, user_name,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    is_persistent, custom_data, ""
                );

            String encrypted_ticket_ud = FormsAuthentication.Encrypt(auth_ticket);
            HttpCookie auth_cookie_ud = new HttpCookie(Cookies.AdminAuthorizationCookie, encrypted_ticket_ud);
            if (is_persistent) auth_cookie_ud.Expires = auth_ticket.Expiration;
            System.Web.HttpContext.Current.Response.Cookies.Add(auth_cookie_ud);
        }

        /// <summary>
        /// this will be used to log out from the admin section
        /// </summary>
        /// <returns></returns>
        [HttpGet, Public]
        public override ActionResult SignOut()
        {
            HttpCookie auth_cookie = Request.Cookies[Cookies.AdminAuthorizationCookie];
            auth_cookie.Expires = DateTime.Now.AddDays(-30);
            Response.Cookies.Add(auth_cookie);
            return Redirect(Url.Action("Index", "Home", new { area = "Admin" }));
        }

        /// <summary>
        /// This will be used to set action name
        /// </summary>
        /// <param name="actionName"></param>
        private void SetActionName(string actionName, string controllerName)
        {
            //ViewBag.ControllerActionName = controllerName + " " + actionName;
            ViewBag.ControllerName = controllerName;
        }
    }
}