using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using HyggeMail.Framework.Api;
using HyggeMail.Framework.Api.Helpers;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Managers;
using HyggeMail.Areas.API;
namespace HyggeMail.Attributes
{
    public class AuthenticateUser : Attribute { }
    
    public class Public : Attribute { }

    public class MemberAccess : Attribute { }

    public class DontValidate : Attribute { }

    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    public class ModuleAccessAttribute : ActionMethodSelectorAttribute
    {
        private int id { get; set; }

        public ModuleAccessAttribute(int id)
        {
            this.id = id;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    /// <summary>
    /// This will be used to set No Cache For Controller Actions
    /// </summary>
    public class NoCacheAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
    }

    /// <summary>
    /// This will be used to skip model validations
    /// </summary>
    public class IgnoreModelErrorsAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        private string keysString;

        public IgnoreModelErrorsAttribute(string keys)
            : base()
        {
            this.keysString = keys;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModelStateDictionary modelState = filterContext.Controller.ViewData.ModelState;
            string[] keyPatterns = keysString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < keyPatterns.Length; i++)
            {
                string keyPattern = keyPatterns[i]
                    .Trim()
                    .Replace(@".", @"\.")
                    .Replace(@"[", @"\[")
                    .Replace(@"]", @"\]")
                    .Replace(@"\[\]", @"\[[0-9]+\]")
                    .Replace(@"*", @"[A-Za-z0-9]+");
                IEnumerable<string> matchingKeys = modelState.Keys.Where(x => Regex.IsMatch(x, keyPattern));
                foreach (string matchingKey in matchingKeys)
                    modelState[matchingKey].Errors.Clear();
            }
        }
    }

    public class HandelExceptionFilter : Attribute { }

    public class HandelExceptionAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        public string DeviceToken { get; set; }
        [Inject]
        public IErrorLogManager _errorLogManager { get; set; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            _errorLogManager = new ErrorLogManager();
            var ex = actionExecutedContext.Exception;
            try
            {
                //Log exception in database
                _errorLogManager.LogExceptionToDatabase(ex);
            }
            catch (Exception)
            {
                System.IO.StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog.txt"), true);
                    sw.WriteLine(ex.Message);
                    sw.WriteLine("http://jsonformat.com/");
                    sw.WriteLine(ex); sw.WriteLine(""); sw.WriteLine("");
                }
                catch { }
                finally { sw.Close(); }
            }

            _errorLogManager.LogExceptionToDatabase(ex);
            System.IO.StreamWriter sw1 = null;
            sw1 = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog.txt"), true);
            sw1.WriteLine(ex.Message);
            sw1.WriteLine("http://jsonformat.com/");
            sw1.WriteLine(ex); sw1.WriteLine(""); sw1.WriteLine("");
            sw1.Close(); 

            actionExecutedContext.Response = new JsonContent("An Unexpected Error Has Occured!", ActionStatus.Error).ConvertToHttpResponseOK();
        }


    }

    /// <summary>
    /// Checks if the incomming user is authorized at the time any function in about to execute
    /// </summary>
    public class CheckAuthorizationFilter : Attribute { }

    public class CheckAuthorizationAttribute : System.Web.Http.AuthorizeAttribute
    {
        [Inject]
        public IUserManager _userManager { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // _userManager = new UserManager();
            //if (actionContext.Request.Method != HttpMethod.Post)
            //{
            //    actionContext.Response = new JsonContent("Only POST requests are allowed.", Status.Failed).ConvertToHttpResponseOK();
            //}
            
            _userManager = new UserManager();

            var baseController = (BaseAPIController)actionContext.ControllerContext.Controller;
            var secretKey = Config.HyggeMailCodeSecretKey;
         
            var clientHash = actionContext.Request.Headers.GetValues("ClientHash").FirstOrDefault();
            var timeStamp = actionContext.Request.Headers.GetValues("Timestamp").FirstOrDefault();
         
            var DeviceToken = actionContext.Request.Headers.GetValues("DeviceToken").FirstOrDefault();
            var DeviceType = actionContext.Request.Headers.GetValues("DeviceType").FirstOrDefault();

            var skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<SkipAuthorization>().Any();
            var skipAuthentication = actionContext.ActionDescriptor.GetCustomAttributes<SkipAuthentication>().Any();
            var mayNeedAuthentication = actionContext.ActionDescriptor.GetCustomAttributes<MayNeedAuthentication>().Any();

            var sessionToken = actionContext.Request.Headers.Any(m => m.Key == "SessionToken") ? actionContext.Request.Headers.GetValues("SessionToken").FirstOrDefault() : null;
            if (clientHash == null)
            {
                actionContext.Response = new JsonContent("Required hash encountered!", ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = Sessionstatus.Expired }).ConvertToHttpResponseOK();
                return;
            }
            if (timeStamp == null)
            {
                actionContext.Response = new JsonContent("Required timestamp encountered!", ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = Sessionstatus.Expired }).ConvertToHttpResponseOK();
                return;
            }
            if (DeviceToken == null)
            {
                actionContext.Response = new JsonContent("Required device Token!", ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = Sessionstatus.Expired }).ConvertToHttpResponseOK();
                return;
            }
            //if (mayNeedAuthentication)
            //{
            //    if (sessionToken != null) SetLoggedInUser(actionContext, sessionToken);
            //    return;
            //}

            bool isValid = false;
            Guid guidOutput = Guid.Empty;
            if (!string.IsNullOrEmpty(sessionToken)) 
            {
                isValid = Guid.TryParse(sessionToken, out guidOutput);
            }

            if (skipAuthentication && isValid == false)
            {
                var validationHash = Utilities.HashCode(string.Format("{0}{1}", timeStamp, secretKey));
                if (!validationHash.Equals(clientHash, StringComparison.InvariantCultureIgnoreCase))
                {
                    actionContext.Response = new JsonContent("Request could not be authenticated. Invalid hash encountered!", ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = Sessionstatus.Expired }).ConvertToHttpResponseOK();
                }
            }
            else
            {
                var validationHash = Utilities.HashCode(string.Format("{0}{1}{2}", sessionToken, timeStamp, secretKey));
                if (!validationHash.Equals(clientHash, StringComparison.InvariantCultureIgnoreCase))
                {
                    actionContext.Response = new JsonContent("Request could not be authenticated. Invalid hash encountered!", ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = Sessionstatus.Expired }).ConvertToHttpResponseOK();
                    return;
                }
                var session = _userManager.ValidateSessionAuth(sessionToken);
                if (session.Status == ActionStatus.Error)
                {
                    Sessionstatus sessionStatus = Sessionstatus.Present;
                    if (session.Message == "Invalid Session")
                        sessionStatus = Sessionstatus.Invalid;
                    else if (session.Message == "Expired Session")
                        sessionStatus = Sessionstatus.Expired;

                    actionContext.Response = new JsonContent(session.Message, ActionStatus.Error, new UserSession() { sessionStatus = false, sessionStatusType = sessionStatus }).ConvertToHttpResponseNAUTH();
                }
                else
                {
                    baseController.LOGGED_IN_USER = new ApiUserModel
                    {
                        UserId = session.Object.UserId,
                        Email = session.Object.Email,
                        SessionId = session.Object.SessionId,
                        FullName = session.Object.Name,
                        UserType = session.Object.UserType
                    };
                }
            }

            //if (!skipAuthorization)
            //{
            //    var appType = actionContext.Request.Headers.Any(m => m.Key == "AppType") ? Convert.ToInt32(actionContext.Request.Headers.GetValues("AppType").FirstOrDefault()) : 1;
            //    var sessionId = new Guid(sessionToken);
            //    UserModel loginSession = new UserModel();//_userManager.ValidateUserSession(sessionId, appType);

            //    if (loginSession == null)
            //    {
            //        actionContext.Response = new JsonContent("Request could not be authorized. Invalid session Id encountered!", Status.Failed).ConvertToHttpResponseOK();
            //    }
            //    else
            //    {
            //        //baseController.LOGGED_IN_USER = new ApiUserModel
            //        //{
            //        //    UserId = loginSession.UserId,
            //        //    Email = loginSession.Email,
            //        //    SessionId = loginSession.SessionId,
            //        //    FullName = loginSession.FullName,
            //        //    DateCreated = loginSession.DateCreated,
            //        //    DateModified = loginSession.DateModified,
            //        //    IsDeleted = loginSession.IsDeleted
            //        //};
            //    }
            //}
        }
    }

    /// <summary>
    /// methods marked with this will not be checked for authorization
    /// </summary>
    public class SkipAuthorization : Attribute { }

    /// <summary>
    /// methods marked with this will not be checked for authentication
    /// </summary>
    public class SkipAuthentication : Attribute { }

    /// <summary>
    /// validates the incomming model
    /// </summary>
    ///
    public class ValidateModel : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                //Check made for attribute if SkipModelValidation attribute is exist then skip this validation error 
                if (!actionContext.ActionDescriptor.GetCustomAttributes<SkipModelValidation>(false).Any())
                    actionContext.Response = new JsonContent("Validation Error!", ActionStatus.Error).ConvertToHttpResponseOK();
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            UserManager baseManager = new UserManager();
            baseManager.Dispose();
            base.OnActionExecuted(actionExecutedContext);
        }
    }

    public class MayNeedAuthentication : Attribute { }
    //This attribute skip the model validation if any exist
    public class SkipModelValidation : Attribute { }
}