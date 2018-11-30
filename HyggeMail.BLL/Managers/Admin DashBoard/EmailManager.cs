using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Configuration;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using AutoMapper;
using HyggeMail.DAL;
using HyggeMail.MailChimp;

namespace HyggeMail.BLL.Managers
{
    public class EmailManager : BaseManager, IEmailManager
    {

        public EmailManager() { }

        PagingResult<TemplateViewModel> IEmailManager.GetEmailTemplateList(PagingModel model)
        {
            var result = new PagingResult<TemplateViewModel>();
            var query = Context.EmailTemplates.OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.TemplateName.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new TemplateViewModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Template List";
            result.TotalCount = query.Count();
            return result;
        }


        ActionOutput IEmailManager.AddUpdateEmailTemplate(AddEditEmailTemplateModel templateModel)
        {
            var existingTemplate = Context.EmailTemplates.FirstOrDefault(z => z.TemplateId == templateModel.TemplateId);
            if (existingTemplate == null)
            {
                Context.EmailTemplates.Add(new EmailTemplate
                {
                    TemplateName = templateModel.TemplateName,
                    EmailSubject = templateModel.EmailSubject,
                    TemplateContent = templateModel.TemplateContent,
                    TemplateStatus = templateModel.TemplateStatus,
                    CreatedOn = DateTime.UtcNow,
                    TemplateType = templateModel.TemplateType
                });
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Template Added Sucessfully."
                };
            }
            else
            {
                existingTemplate.EmailSubject = templateModel.EmailSubject;
                existingTemplate.TemplateContent = templateModel.TemplateContent;
                existingTemplate.TemplateStatus = templateModel.TemplateStatus;
                existingTemplate.UpdatedOn = DateTime.UtcNow;
                existingTemplate.TemplateType = templateModel.TemplateType;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Template Updated Sucessfully."
                };
            }
        }

        AddEditEmailTemplateModel IEmailManager.GetEmailTemplateByTemplateId(int templateId)
        {
            var existingTemplate = Context.EmailTemplates.FirstOrDefault(z => z.TemplateId == templateId);
            if (existingTemplate != null)
                return new AddEditEmailTemplateModel(existingTemplate);
            else
                return null;
        }

        EmailTemplateModel IEmailManager.GetEmailTemplateByType(int type)
        {
            return GetTemplate(type);
        }


        bool IEmailManager.SendNewUserRegistrationEmail(int userID, User model)
        {
            var retVal = false;
            var user = Context.Users.Where(x => x.UserID == userID).FirstOrDefault();
            //Send welcome Email
            EmailTemplateModel emaildata = GetTemplate(1);
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.ActivatedUID = token;
            Context.SaveChanges();
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;
        }

        bool IEmailManager.SendRejectionEmailToUser(int recipientCardID, string reason)
        {
            var retVal = false;
            var CARD = Context.UserPostCardRecipients.Where(x => x.ID == recipientCardID).FirstOrDefault();
            //Send welcome Email
            EmailTemplateModel emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.RejectionEmail));
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, CARD.UserPostCard.User.FirstName + ' ' + CARD.UserPostCard.User.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "REASON" + gt, reason);
            var Domain = Config.Link;
            string rejurl = string.Format("{0}/Home/RejectionReason?id={1}", Domain, recipientCardID);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "REJECTREASON" + gt, rejurl);

            if (CARD.UserPostCard.ShipmentDate.HasValue)
            {
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DATE" + gt, CARD.UserPostCard.ShipmentDate.Value.ToString("MMM dd yyyy"));
                string url = string.Format("{0}/User/PostCard/ShowPostCard?postCardID={1}&userid={2}", Domain, CARD.UserPostCard.ID, CARD.UserPostCard.UserID);
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);
            }
            var result = Utilities.SendEMail(CARD.UserPostCard.User.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;
        }


        bool IEmailManager.SendVerifyMailToNewUser(int userID, User model)
        {
            var retVal = false;

            var user = Context.Users.Where(x => x.UserID == userID).FirstOrDefault();
            //Send welcome Email
            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.VerificationAccount));
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.ActivatedUID = token;
            Context.SaveChanges();
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            var Domain = Config.Link;
            string url = string.Format("{0}/Account/EmailVerification?token={1}", Domain, token);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);

            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;

        }

        void IEmailManager.SendWelcomeMailToNewUser(User model)
        {
            var userName = string.Format("{0} {1}", model.FirstName, model.LastName);
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            //Send welcome Email
            var user = Context.Users.Where(x => x.UserID == model.UserID).FirstOrDefault();
            user.ActivatedUID = token;
            Context.SaveChanges();
            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.SignUpEmail));
            var Domain = Config.Link;
            var lt = "&lt;%";
            var gt = "%&gt;";
            string url = string.Format("{0}/Account/EmailVerification?token={1}", Domain, token);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName).Replace(lt + "URL" + gt, url);

            var result = Utilities.SendEMail(model.Email, emaildata.EmailSubject, emaildata.TemplateContent);

        }

        private EmailTemplateModel GetTemplate(int type)
        {
            var Template = Context.EmailTemplates.FirstOrDefault(z => z.TemplateType == type);

            if (Template != null)
            {
                EmailTemplateModel newModel = new EmailTemplateModel();
                newModel = Mapper.Map<EmailTemplate, EmailTemplateModel>(Template, newModel);
                return newModel;
            }
            else
                return null;
        }



        bool IEmailManager.SendSignInMailtoUser(User user)
        {
            var retVal = false;

            //Send welcome Email
            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.UserSigninDetails));
            var decryptPassword = Utilities.DecryptPassword(user.Password, true);
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "EMAIL" + gt, user.Email);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "PASSWORD" + gt, decryptPassword);
            var Domain = Config.Link;
            string url = string.Format("{0}/Account/Login", Domain);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);

            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;
        }

        bool IEmailManager.SendOrderPlacedForUser(User user)
        {
            var retVal = false;

            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.OrderPlaced));
            var decryptPassword = Utilities.DecryptPassword(user.Password, true);
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DATETIME" + gt, Convert.ToDateTime(DateTime.UtcNow).ToString("MMM dd yyyy hh:mm tt"));
            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;
        }

        bool IEmailManager.SendOrderStatusChangeMailForUser(User user, string status)
        {
            var retVal = false;

            //var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.OrderStatusChange));
            //var decryptPassword = Utilities.DecryptPassword(user.Password, true);
            //var lt = "&lt;%";
            //var gt = "%&gt;";
            //emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            //emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "STATUS" + gt, status);
            //emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DATETIME" + gt, Convert.ToDateTime(DateTime.UtcNow).ToString("MMM dd yyyy hh:mm tt"));
            //var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            //if (result == true)
            //{
            //    retVal = true;
            //}
            return retVal;
        }


        void IEmailManager.SendCardWasMailedEmail(User model)
        {
            //Complete Card Mail

            var userName = string.Format("{0} {1}", model.FirstName, model.LastName);

            var user = Context.Users.Where(x => x.UserID == model.UserID).FirstOrDefault();

            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.CardWasMailedEmail));

            if (emaildata != null)
            {
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(EmailConst.UserName, userName);

                var result = Utilities.SendEMail(model.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            }
        }

        void IEmailManager.SendOrderStatusToAdmin(string Status, User model)
        {
            var email = Config.DemoPostCardEmail;
            var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.OrderStatusAdmin));
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, model.FirstName + ' ' + model.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "STATUS" + gt, Status);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DATETIME" + gt, Convert.ToDateTime(DateTime.UtcNow).ToString("MMM dd yyyy hh:mm tt"));
            var result = Utilities.SendEMail(email, emaildata.EmailSubject, emaildata.TemplateContent);
        }

        public ActionOutput GetTheGuideEmail(GetGuideModel GetGuideModel)
        {
            try
            {
                var email = GetGuideModel.EMAIL;
                var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.GetTheGuideEmail));
                var lt = "&lt;%";
                var gt = "%&gt;";
                var Domain = Config.Link;
                string Durl = string.Format("{0}/Home/DownloadGuide", Domain);
                string Jurl = "https://mailchi.mp/3a986420cc90/join-hyggemail-newsletter";
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DURL" + gt, Durl);
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "JURL" + gt, Jurl);
                // emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "DATETIME" + gt, Convert.ToDateTime(DateTime.UtcNow).ToString("MMM dd yyyy hh:mm tt"));
                var result = Utilities.SendEMail(email, emaildata.EmailSubject, emaildata.TemplateContent);
                MailChimpService.AddOrUpdateListMember(subscriberEmail: GetGuideModel.EMAIL, listId: System.Configuration.ConfigurationManager.AppSettings["SubListId"]);
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Guide Sent Successfully."
                };
            }
            catch (Exception ex)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public void SendNotificationEmail()
        {
            using (var Context = new HyggeMailEntities())
            {
                var template4Users = Context.Users.Where(p => p.CardsCount < 10 && !p.IsDeleted && p.Email != null && p.ActivatedUID != null && p.IsActivated != null).ToList();
                var template5Users = Context.Users.Where(p => p.CardsCount == 10 && !p.IsDeleted && p.Email != null && p.ActivatedUID != null && p.IsActivated != null).ToList();
                foreach (var item in template4Users)
                {
                    var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.LittleBitsOfHappiness));
                    var Domain = Config.Link;
                    var result = Utilities.SendEMail(item.Email, emaildata.EmailSubject, emaildata.TemplateContent);
                }
                foreach (var item in template5Users)
                {
                    var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.StepsToBuyingCards));
                    var Domain = Config.Link;
                    var result = Utilities.SendEMail(item.Email, emaildata.EmailSubject, emaildata.TemplateContent);
                }
            }
        }
    }

}