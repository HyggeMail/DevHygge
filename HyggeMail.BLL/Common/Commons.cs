using HyggeMail.BLL.Common;
using HyggeMail.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyggeMail.BLL.Models
{
    public class ActionOutputBase
    {
        public ActionStatus Status { get; set; }
        public String Message { get; set; }
        public List<String> Results { get; set; }
    }

    public class ActionOutput<T> : ActionOutputBase
    {
        public T Object { get; set; }
        public List<T> List { get; set; }
    }

    public class ActionOutput : ActionOutputBase
    {
        public Int32 ID { get; set; }
        public Int32 AvailableTokens { get; set; }
    }

    public class PagingResult<T>
    {
        public List<T> List { get; set; }
        public int TotalCount { get; set; }
        public ActionStatus Status { get; set; }
        public String Message { get; set; }
    }

    public class PagingModel : CustomPagingModel
    {
        public int PageNo { get; set; }
        public int RecordsPerPage { get; set; }
        public PagingModel()
        {
            if (PageNo <= 1)
            {
                PageNo = 1;
            }
            if (RecordsPerPage <= 0)
            {
                RecordsPerPage = AppDefaults.PageSize;
            }
        }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public int UserID { get; set; }
        public string Search { get; set; }

        public static PagingModel DefaultModel(string sortBy = "CreatedOn", string sortOder = "Asc")
        {
            return new PagingModel { PageNo = 1, RecordsPerPage = 10, SortBy = sortBy, SortOrder = sortOder };
        }
    }

    public class UserHistoryPaggigModel : PagingModel
    {
        public int userID { get; set; }
    }

    public class OrderPagingModel : PagingModel
    {
        public string ShowOrderBy { get; set; }
        public string SortByOrder { get; set; }
        public eAdminPostOrderStatus Status { get; set; }
        public int IsOrderPlaced { get; set; }
    }

    public class RecipientOrderPagingModel : PagingModel
    {
        public eRecipientOrderStatus shownByStatus { get; set; }
        public eShownRecipientOrdersByDays shownsBydays { get; set; }
        public int IsOrderPlaced { get; set; }
    }

    public class CustomPagingModel
    {
        public bool Checked { get; set; }
    }

    public class UserDetails
    {
        public Int32 UserID { get; set; }
        public String FirstName { get; set; }
        public UserTypes UserType { get; set; }
        public String UserName { get; set; }
        public String LastName { get; set; }
        public String UserEmail { get; set; }
        public bool? IsActive { get; set; }
        public string Image { get; set; }
        public string ImageLink { get; set; }
        public string ImageName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsAuthenticated { get; set; }
        public string facebookID { get; set; }
        public AuthenticationType eAuthenticationType { get; set; }
        public UserDetails()
        { }
        public UserDetails(UserModel usermodel)
        {
            FirstName = usermodel.FirstName;
            LastName = usermodel.LastName;
            UserEmail = usermodel.Email;
            UserName = usermodel.Email;
            IsAuthenticated = true;
            UserID = usermodel.UserId;
            UserType = (UserTypes)usermodel.UserType;
            eAuthenticationType = usermodel.eAuthenticationType;
            facebookID = usermodel.facebookID;
            ImageLink = usermodel.ImagePath;
        }
    }

    public class apiUserDetailShort
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool EmailVerfied { get; set; }
        public string DeviceToken { get; set; }
        public short DeviceType { get; set; }
        public string CompanyName { get; set; }
        public int UserType { get; set; }
        public bool IsGeneralPreferenceExist { get; set; }
        public string Phone { get; set; }
        public short? OnStep { get; set; }
        public bool? hasWorkOrder { get; set; }
        public bool? IsSuspend { get; set; }
        [JsonProperty(PropertyName = "Count")]
        public int? NotificationCount { get; set; }
    }


    public class apiUserDetail
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int? CardsCount { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public int? UserType { get; set; }
        public bool? IsActivated { get; set; }
        public AuthenticationType eAuthenticationType { get; set; }
        public string DeviceToken { get; set; }
        public short DeviceType { get; set; }
        public string facebookID { get; set; }
        public bool IsLoginWithFb { get; set; }
        public string countryId { get; set; }
        public string stateId { get; set; }
        public string cityId { get; set; }
        public string SessionId { get; set; }

        public apiUserDetail(User userObj, List<country> allcountries, List<state> allstates, List<city> allcities, string sessionId)
        {
            this.UserId = userObj.UserID;
            this.FirstName = userObj.FirstName;
            this.LastName = userObj.LastName;
            this.DeviceToken = userObj.DeviceToken ?? "";
            this.SessionId = sessionId;
            //this.DeviceType = (short)userObj.DeviceType;
            if (userObj.UserDetail != null)
            {
                this.Zip = userObj.UserDetail.Zip ?? "";
                if (userObj.UserDetail.Country != null)
                {
                    int cid = 0;
                    int.TryParse(userObj.UserDetail.Country, out cid);
                    if (cid > 0)
                        this.CountryName = allcountries.FirstOrDefault(p => p.id == cid).name;
                    else
                        this.CountryName = userObj.UserDetail.Country;
                }
                else
                    this.CountryName = "";

                if (userObj.UserDetail.State != null)
                {
                    int sid = 0;
                    int.TryParse(userObj.UserDetail.State, out sid);
                    if (sid > 0)
                        this.StateName = allstates.FirstOrDefault(p => p.id == sid).name;
                    else
                        this.StateName = userObj.UserDetail.State;
                }
                else
                    this.StateName = "";

                if (userObj.UserDetail.City != null)
                {
                    int ccid = 0;
                    int.TryParse(userObj.UserDetail.City, out ccid);
                    if (ccid > 0)
                        this.CityName = allcities.FirstOrDefault(p => p.cityID == ccid).cityName;
                    else
                        this.CityName = userObj.UserDetail.City;
                }
                else
                    this.CityName = "";

                this.countryId = userObj.UserDetail.Country ?? "";
                this.stateId = userObj.UserDetail.State ?? "";
                this.cityId = userObj.UserDetail.City ?? "";
                this.facebookID = userObj.UserDetail.FacebookID ?? "";
                this.Address = userObj.UserDetail.Address ?? "";
                this.Phone = userObj.UserDetail.Phone ?? "";
                this.ImagePath = userObj.UserDetail.ProfileImagePath ?? "";
                // this.ImageName = userObj.UserDetail.ProfileImageName ?? "";
            }
            this.UserType = userObj.UserType;
            this.IsActivated = userObj.IsActivated;
            this.CardsCount = userObj.CardsCount ?? 0;
            // this.eAuthenticationType = (AuthenticationType)userObj.AuthenticationType;
            this.Email = userObj.Email ?? "";
        }
    }

    public class ExceptionModal
    {
        public Exception Exception { get; set; }
        public UserDetails User { get; set; }
        public string FormData { get; set; }
        public string QueryData { get; set; }
        public string RouteData { get; set; }
        public string BrowserName { get; set; }
    }

    public class ExceptionReturnModal
    {
        public string ErrorID { get; set; }
        public string ErrorText { get; set; }
        public bool DatabaseLogStatus { get; set; }
    }


}