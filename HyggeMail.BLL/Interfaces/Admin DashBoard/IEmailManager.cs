using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.DAL;

namespace HyggeMail.BLL.Interfaces
{
    public interface IEmailManager
    {
        /// <summary>
        /// This will be used to get all template list.
        /// </summary>
        /// <returns></returns>
        PagingResult<TemplateViewModel> GetEmailTemplateList(PagingModel model);

        /// <summary>
        /// This will be used to add or update email template
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        ActionOutput AddUpdateEmailTemplate(AddEditEmailTemplateModel templateModel);

        /// <summary>
        /// Will be used to get template by template id
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        AddEditEmailTemplateModel GetEmailTemplateByTemplateId(int templateId);

        /// <summary>
        /// This will be used to get email template by id
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        EmailTemplateModel GetEmailTemplateByType(int type);

        /// <summary>
        /// Send welcome mail to New User
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SendNewUserRegistrationEmail(int userID, User model);

        /// <summary>
        /// Send verification mail to New space owner
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SendVerifyMailToNewUser(int userID, User model);

        bool SendSignInMailtoUser(User user);

        bool SendOrderPlacedForUser(User user);

        bool SendOrderStatusChangeMailForUser(User user, string status);

        bool SendRejectionEmailToUser(int recipientCardID, string reason);

        void SendWelcomeMailToNewUser(User model);

        void SendCardWasMailedEmail(User model);

        void SendOrderStatusToAdmin(string Status,User model);

        ActionOutput GetTheGuideEmail(GetGuideModel GetGuideModel);
    }
}
