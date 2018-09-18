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
using System.IO;

namespace HyggeMail.Areas.API
{
    [RoutePrefix("api/user")]
    public class UserController : BaseAPIController
    {
        private IUserManager _userManager;
        private IEmailTemplateManager _emailTemplateManager;
        private IErrorLogManager _errorLogManager;
        private INotificationStackManager _notificationStackManager;
        public UserController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager emailTemplateManager, INotificationStackManager notificationStackManager)
        {
            _userManager = userManager;
            _emailTemplateManager = emailTemplateManager;
            _errorLogManager = errorLogManager;
            _notificationStackManager = notificationStackManager;
        }


        #region Update Password

        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage UpdatePassword(ChangePasswordModel model)
        {
            model.userID = LOGGED_IN_USER.UserId;
            // _userManager.APILogout(LOGGED_IN_USER.SessionId, LOGGED_IN_USER.UserId);
            var data = _userManager.UpdateUserPassword(model);

            return new JsonContent(data).ConvertToHttpResponseOK();
        }

        #endregion

        #region Contact Us
        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage SubmitQuery(ContactUsModel model)
        {
            model.UserID = LOGGED_IN_USER.UserId;
            var data = _userManager.RequestContactUs(model);
            return new JsonContent(data).ConvertToHttpResponseOK();
        }

        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage UpdateProfile(UserModel model)
        {
            ActionOutput<apiUserDetail> result = new ActionOutput<apiUserDetail>();
            model.UserId = LOGGED_IN_USER.UserId;
            result = _userManager.UpdateUserDetailsApp(model, true);
            if (result.Status == ActionStatus.Successfull)
                return new JsonContent(result.Message, result.Status, result.Object).ConvertToHttpResponseOK();
            else
                return new JsonContent(result.Message, result.Status).ConvertToHttpResponseOK();
        }
        #endregion

        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage Logout()
        {
            //  _userManager.RemoveOldGCM(LOGGED_IN_USER.UserId);
            var result = _userManager.APILogout(LOGGED_IN_USER.SessionId, LOGGED_IN_USER.UserId);
            return new JsonContent(result).ConvertToHttpResponseOK();
        }

        //[HttpPost]
        //[ResponseType(typeof(Response<UserModel>))]
        //public HttpResponseMessage GetUserProfile()
        //{
        //    var result = new ActionOutput<UserModel>();
        //    try
        //    {
        //        result = _userManager.GetUserDetailsByUserId(LOGGED_IN_USER.UserId);
        //        return new JsonContent("User Profile", ActionStatus.Successfull, result.Object).ConvertToHttpResponseOK();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
        //    }
        //}

        [HttpPost]
        [ResponseType(typeof(Response<string>))]
        public HttpResponseMessage EditorResponse(EditorModel model)
        {
            var result = new ActionOutput<UserModel>();
            try
            {
                _errorLogManager.LogStringExceptionToDatabase(model.response);
                return new JsonContent("User Profile", ActionStatus.Successfull, result.Object).ConvertToHttpResponseOK();
            }
            catch (Exception ex)
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }

    }
}
