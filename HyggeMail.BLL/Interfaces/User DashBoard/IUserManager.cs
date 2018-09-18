using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Interfaces
{
    public interface IUserManager
    {
        /// <summary>
        /// Dummy Method for testing purpose:  Get Welcome Message
        /// </summary>
        /// <returns></returns>
        string GetWelcomeMessage();

        /// <summary>
        /// This will be used to get user listing model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PagingResult<UserListingModel> GetUserPagedList(PagingModel model);

        /// <summary>
        /// Update User Details from Admin Panel
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        ActionOutput UpdateUserDetails(UserModel userDetails, bool fromDashboard = false);

        /// <summary>
        /// Update User Details from App
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>

        ActionOutput<apiUserDetail> UpdateUserDetailsApp(UserModel userDetails, bool ProfileUpdate);

        /// <summary>
        /// Add User Details from Admin Panel
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        ActionOutput AddUserDetails(AddUserModel userDetails);
        ActionOutput SignUpUser(UserRegistrationModel userDetails);
        UserModel UserLogin(LoginModal model);
        UserModel AdminLogin(LoginModal model);
        UserModel GetAdminDetails();
        /// <summary>
        /// Get User Details by User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionOutput<UserModel> GetUserDetailsByUserId(int userId);

        /// <summary>
        /// Delete User By User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionOutput DeleteUser(int userId);

        ActionOutput ActivateUser(int userId);

        ActionOutput SendAccountVerificationMail(int userId);

        bool SendForgotPassword(int userID, User model);
        bool SendNewUserRegistrationEmail(int userID, User model);
        bool SendVerifyMailToNewUser(int userID, User model);
        ActionOutput UpdateUserPassword(ChangePasswordModel model);
        UserModel GetUserByToken(string token);
        ActionOutput ChangePassword(ResetPasswordModel model);
        string SetUserActive(int Id);
        UserModel GetUserById(int userid);

        bool ResetPassword(UserModel user);

        /// <summary>
        /// Get state by country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        List<SelectListItem> GetStateByCountry(Int32 country);
        List<SelectListItem> GetCityByState(Int64 state);
        List<SelectListItem> GetCountries();
        List<SelectListItem> GetStateNameByStateID(int stateID);
        List<SelectListItem> GetCityNameByCityID(int cityID);
        UserModel GetUserByEmail(string Email);
        UserModel GetUserByForgotToken(string Email);
        ActionOutput<apiUserDetailShort> FaceBookAuthentication(FacebookUserDetails userDetails);
        ActionOutput<apiUserDetail> FaceBookAuthenticationApp(FacebookUserDetailsApp userDetails);
        ActionOutput CheckAlreadyFaceBookAuthenticated(FacebookUserDetails userDetails);
        UserModel LoginWithFaceBook(FacebookUserDetails userDetails);
        ActionOutput<apiUserDetail> ValidateSession(string sessionId);
        ActionOutput<apiUserDetailShort> ValidateSessionAuth(string sessionId);
        ActionOutput WebRequestContactUs(WebContactUsModel model);
        ActionOutput GetUserCardStatus(int userID);
        #region API Manager
        bool IsAlreadySessionExist(LoginModal model);
        DeviceModel GetLastSessionDetails(LoginModal model);
        ActionOutput<apiUserDetail> AuthenticateUserOnMobile(LoginModal model);
        ActionOutput APILogout(string sessionId, int userID);
        void ExpirePreviousSessions(LoginModal user);
        ActionOutput RequestContactUs(ContactUsModel model);

        List<SelectListItem> GetCities();
        List<SelectListItem> GetStates();
        int GetSessionByToken(string Token);

        #endregion

        ActionOutput SetUserNotificationSettings(UserNotificationSetting model);
        ActionOutput<UserNotificationSetting> GetUserNotificationSettings(int userId);

        PagingResult<UserHistoryModel> GetUserHistoryPageList(PagingModel model);
        ActionOutput AddHistory(UserHistoryModel UserHistoryModel);
        ReceiptentOrder ViewRecipientOrder(int OrderID);
        bool GetUserCardStepGuidance(int userID);
        ActionOutput DontShowAgain(int userID);
    }
}
