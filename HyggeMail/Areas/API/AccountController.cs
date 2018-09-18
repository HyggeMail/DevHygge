using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using HyggeMail.Framework.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HyggeMail.Framework.Api.Helpers;
using HyggeMail.BLL.Notification;
using HyggeMail.BLL.Managers;
using HyggeMail.Framework.Notifications;
using HyggeMail.BLL.Common;
namespace HyggeMail.Areas.API
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseAPIController
    {
        private IUserManager _userManager;
        private IEmailTemplateManager _emailTemplateManager;
        private IErrorLogManager _errorLogManager;
        private INotificationStackManager _notificationStackManager;
        public AccountController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager emailTemplateManager, INotificationStackManager notificationStackManager)
        {
            _userManager = userManager;
            _emailTemplateManager = emailTemplateManager;
            _errorLogManager = errorLogManager;
            _notificationStackManager = notificationStackManager;
        }

        /// <summary>
        /// Get Currently Logged in expert weekly availability
        /// </summary>
        /// <returns>AvailabilityConfigurationModel</returns>
        [HttpGet, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<ResponseBase>))]
        public HttpResponseMessage WelcomeAPI()
        {
            return new JsonContent("Welcome", ActionStatus.Successfull, null).ConvertToHttpResponseOK();
        }

        [HttpPost, SkipAuthentication, SkipModelValidation]
        [ResponseType(typeof(Response<UserModel>))]
        public HttpResponseMessage Register(UserRegistrationModel model)
        {
            ActionOutput result = new ActionOutput();
            result = _userManager.SignUpUser(model);
            if (result.Status == ActionStatus.Successfull)
                return new JsonContent(result).ConvertToHttpResponseOK();
            else
                return new JsonContent(result.Message, result.Status).ConvertToHttpResponseOK();
        }

        [HttpPost, SkipAuthentication]
        [ResponseType(typeof(Response<apiUserDetail>))]
        public HttpResponseMessage login(LoginModal model)
        {
            PushNotify _notify = new PushNotify();
            // Check last session exist or not
            if (_userManager.IsAlreadySessionExist(model))
            {
                //remove the all previous sessions of user
                _userManager.ExpirePreviousSessions(model);
                //var lastSession = _userManager.GetLastSessionDetails(model);
                //// if exist then send a notification that your session has expired
                //if (lastSession != null)
                //{

                //if (lastSession.DeviceType == DeviceType.Android)
                //{
                //    var gcmModel = new GCM_Session_Expired()
                //    {
                //        GCM_ID = "GCM_SESSION_EXPIRED",
                //        Message = Messages.GCM_SESSION_EXPIRED
                //    };
                //    var notificationStack = new NotificationStackModel()
                //    {
                //        UserId = lastSession.UserId,
                //        Priority = Priority.High,
                //        Status = BLL.Models.NotificationStatus.Pending,
                //        DeviceId = lastSession.DeviceToken,
                //        Message = gcmModel,
                //    };
                //    var notificatioStack = _notificationStackManager.AddOrUpdateStack(notificationStack);
                //}
                // }
            }
            var result = _userManager.AuthenticateUserOnMobile(model);
            if (result.Status == ActionStatus.Successfull)
            {
                try
                {
                    if (result.Object.DeviceType == (short)DeviceType.IOS)
                        PushNotifier.NotifyIOSUser(result.Object.DeviceToken, "Login successfully", NotificationType.MessageAlert);
                }
                catch (Exception ex)
                {
                    _errorLogManager.LogStringExceptionToDatabase(ex.Message);
                }
                return new JsonContent("Login Successfull", result.Status, result.Object).ConvertToHttpResponseOK();
            }
            return new JsonContent(result.Message, result.Status).ConvertToHttpResponseOK();
        }

        [HttpPost, SkipAuthentication]
        [ResponseType(typeof(ResponseBase))]
        public HttpResponseMessage ForgotPassword(ForgetPasswordModel model)
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
                    data.Message = "Password reset process interrupted due to some reason. Please contact HelpDesk.";
                    data.Status = ActionStatus.Error;
                }
            }
            else
            {
                data.Message = "This email doesnt exist";
                data.Status = ActionStatus.Error;
            }
            return new JsonContent(data.Message, data.Status).ConvertToHttpResponseOK();
        }

        #region Facebook Authentication from APP
        /// <summary>
        /// Get state list by Country ID
        /// </summary>
        [HttpPost, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<apiUserDetail>))]
        public HttpResponseMessage aFacebookAuthentication(FacebookUserDetailsApp model)
        {
            var DeviceToken = Request.Properties.GetType();
            var DeviceType = Request.Headers.GetValues("DeviceType").FirstOrDefault();

            model.IsMobile = true;
            var result = _userManager.FaceBookAuthenticationApp(model);
            return new JsonContent(result.Message, result.Status, result.Object).ConvertToHttpResponseOK();
        }
        #endregion

        #region Countries, States, Cities
        /// <summary>
        /// Get Country List
        /// </summary>
        [HttpGet, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<List<System.Web.Mvc.SelectListItem>>))]
        public HttpResponseMessage GetCountries()
        {
            var list = _userManager.GetCountries();
            return new JsonContent("Total Countries", ActionStatus.Successfull, list).ConvertToHttpResponseOK();
        }

        /// <summary>
        /// Get state list by Country ID
        /// </summary>
        [HttpGet, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<List<System.Web.Mvc.SelectListItem>>))]
        public HttpResponseMessage GetStateByCountryID(string countryID)
        {
            var stateList = _userManager.GetStateByCountry(Convert.ToInt32(countryID));
            return new JsonContent("Total States", ActionStatus.Successfull, stateList).ConvertToHttpResponseOK();
        }

        /// <summary>
        /// Get state list by Country ID
        /// 
        /// </summary>
        [HttpGet, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<List<System.Web.Mvc.SelectListItem>>))]
        public HttpResponseMessage GetCityByStateID(string stateID)
        {
            var cityList = _userManager.GetCityByState(Convert.ToInt32(stateID));
            return new JsonContent("Total Cities", ActionStatus.Successfull, cityList).ConvertToHttpResponseOK();
        }

        /// <summary>
        /// Get request type list
        /// </summary>
        [HttpGet, SkipAuthentication, SkipAuthorization]
        [ResponseType(typeof(Response<List<System.Web.Mvc.SelectListItem>>))]
        public HttpResponseMessage GetRequestType()
        {
            var typeList = Utilities.EnumToList(typeof(ContactRequestType)); ;
            return new JsonContent("Types", ActionStatus.Successfull, typeList).ConvertToHttpResponseOK();
        }

        #endregion

    }
}
