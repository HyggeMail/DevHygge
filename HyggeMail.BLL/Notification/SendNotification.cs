using AutoMapper;
using HyggeMail.BLL.Common;
using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Notification
{
    public class SendNotification
    {

        public void SendMail()
        {
            using (var Context = new HyggeMailEntities())
            {
                try
                {
                    var template4Users = Context.Users.Where(p => p.CardsCount < 10 && !p.IsDeleted && p.Email != null && p.ActivatedUID != null && (p.IsActivated != null && (bool)p.IsActivated)).ToList();
                    var template5Users = Context.Users.Where(p => p.CardsCount == 10 && !p.IsDeleted && p.Email != null && p.ActivatedUID != null && (p.IsActivated != null && (bool)p.IsActivated)).ToList();
                    foreach (var item in template4Users)
                    {
                        var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.LittleBitsOfHappiness));
                        var Domain = Config.Link;
                        var result = Utilities.SendEMail(item.Email, emaildata.EmailSubject, emaildata.TemplateContent);
                        Context.ChroneLogs.Add(new ChroneLog()
                        {
                            UserId = item.UserID,
                            TemplateSent = TemplateTypes.LittleBitsOfHappiness.ToString(),
                            MailFiredOn = DateTime.Now
                        });
                        Context.SaveChanges();
                    }
                    foreach (var item in template5Users)
                    {
                        var emaildata = GetTemplate(Convert.ToInt32(TemplateTypes.StepsToBuyingCards));
                        var Domain = Config.Link;
                        var result = Utilities.SendEMail(item.Email, emaildata.EmailSubject, emaildata.TemplateContent);
                        Context.ChroneLogs.Add(new ChroneLog()
                        {
                            UserId = item.UserID,
                            TemplateSent = TemplateTypes.StepsToBuyingCards.ToString(),
                            MailFiredOn = DateTime.Now
                        });
                        Context.SaveChanges();
                    }
                }
                catch (Exception ex) { }
            }
        }

        private EmailTemplateModel GetTemplate(int type)
        {
            using (var Context = new HyggeMailEntities())
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
        }
    }
}
