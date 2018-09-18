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
using HyggeMail.BLL.Interfaces.Admin_DashBoard;
using HyggeMail.Framework.Notifications;
namespace HyggeMail.Areas.API
{
    [RoutePrefix("api/home")]
    public class HomeController : BaseAPIController
    {
        private IUserManager _userManager;
        private IEmailTemplateManager _emailTemplateManager;
        private IErrorLogManager _errorLogManager;
        private INotificationStackManager _notificationStackManager;
        private IRecipientManager _recipientManager;
        private IEditorManager _editorManager;
        private IAddressBookManager _addressBookManager;
        private IPaymentManager _paymentManager;

        public HomeController(IUserManager userManager, IErrorLogManager errorLogManager, IEmailTemplateManager emailTemplateManager, INotificationStackManager notificationStackManager, IRecipientManager recipientManager, IEditorManager editorManager, IPaymentManager paymentManager, IAddressBookManager addressBookManager)
        {
            _userManager = userManager;
            _emailTemplateManager = emailTemplateManager;
            _errorLogManager = errorLogManager;
            _notificationStackManager = notificationStackManager;
            _recipientManager = recipientManager;
            _editorManager = editorManager;
            _paymentManager = paymentManager;
            _addressBookManager = addressBookManager;
        }

        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage AddressBook()
        {
            ActionOutput<RecipientDetails> modal = _recipientManager.GetUserRecipients(LOGGED_IN_USER.UserId);
            try
            {
                if (modal.List.Count > 0)
                    return new JsonContent(modal.Message, modal.Status, modal.List).ConvertToHttpResponseOK();
                else
                    return new JsonContent("No Address Found", ActionStatus.Successfull, modal.List).ConvertToHttpResponseOK();
            }
            catch (Exception ex)
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }

        [HttpGet]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage DeleteBookAddress(int id)
        {
            try
            {
                var result = _addressBookManager.DeleteBookAddressById(LOGGED_IN_USER.UserId, id);
                return new JsonContent(result.Message, result.Status).ConvertToHttpResponseOK();
            }
            catch
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }

        }

        [HttpGet]
        [ResponseType(typeof(Response<ActionOutput>))]
        public HttpResponseMessage DeletePostCard(int id)
        {
            try
            {
                var result = _editorManager.DeletePostCardByID(id); ;
                return new JsonContent(result.Message, result.Status).ConvertToHttpResponseOK();
            }
            catch
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }

        }


        [HttpPost]
        [ResponseType(typeof(Response<ActionOutput<RecipientDetails>>))]
        public HttpResponseMessage AddUpdateBookAddress(AddUpdateRecipientModel model)
        {
            try
            {
                model.UserID = LOGGED_IN_USER.UserId;
                var result = _addressBookManager.AddUpdateAddressBook(model);
                return new JsonContent(result.Message, result.Status, result.Object).ConvertToHttpResponseOK();
            }
            catch
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }

        [HttpPost]
        [ResponseType(typeof(Response<PagingResult<PostCardListingModel>>))]
        public HttpResponseMessage PostCardsList(OrderPagingModel model)
        {
            // model.RecordsPerPage = model.RecordsPerPage == 10 ? 12 : model.RecordsPerPage;
            var modal = _editorManager.GetPostCardPagedList(model, LOGGED_IN_USER.UserId);
            try
            {
                if (modal.List.Count > 0)
                    return new JsonContent(modal.Message, modal.Status, modal.List).ConvertToHttpResponseOK();
                else
                    return new JsonContent(modal.Message, ActionStatus.Successfull, modal.List).ConvertToHttpResponseOK();
            }
            catch (Exception ex)
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }

        [HttpPost]
        [ResponseType(typeof(Response<PagingResult<RecipientPostCardListingModel>>))]
        public HttpResponseMessage MyOrderList(RecipientOrderPagingModel model)
        {
            //model.RecordsPerPage = model.RecordsPerPage == 10 ? 12 : model.RecordsPerPage;
            var modal = _editorManager.GetMyPostCardOrdersPaggedList(model, LOGGED_IN_USER.UserId);
            try
            {
                if (modal.List.Count > 0)
                    return new JsonContent(modal.Message, modal.Status, modal.List).ConvertToHttpResponseOK();
                else
                    return new JsonContent(modal.Message, ActionStatus.Successfull, modal.List).ConvertToHttpResponseOK();
            }
            catch (Exception ex)
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }

        [HttpPost]
        [ResponseType(typeof(Response<PagingResult<PayPalTransaction>>))]
        public HttpResponseMessage TransactionsList(PagingModel model)
        {
            model.UserID = LOGGED_IN_USER.UserId;
            model.SortBy = "TransactionDate";
            model.SortOrder = "Desc";
            var modal = _paymentManager.GetTransactionPagedList(model);
            try
            {
                if (modal.List.Count > 0)
                    return new JsonContent(modal.Message, modal.Status, modal.List).ConvertToHttpResponseOK();
                else
                    return new JsonContent(modal.Message, ActionStatus.Successfull, modal.List).ConvertToHttpResponseOK();
            }
            catch (Exception ex)
            {
                return new JsonContent("Internal Server Error", ActionStatus.Error, null).ConvertToHttpResponseOK();
            }
        }
    }
}
