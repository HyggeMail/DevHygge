using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;

namespace HyggeMail.BLL.Managers
{
    public class EmailTemplateManager : BaseManager, IEmailTemplateManager
    {

        PagingResult<TemplateViewModel> IEmailTemplateManager.GetEmailTemplateList(PagingModel model)
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


        ActionOutput IEmailTemplateManager.AddUpdateEmailTemplate(AddEditEmailTemplateModel templateModel)
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
                existingTemplate.TemplateName = templateModel.TemplateName;
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




        AddEditEmailTemplateModel IEmailTemplateManager.GetEmailTemplateByTemplateId(int templateId)
        {
            var existingTemplate = Context.EmailTemplates.FirstOrDefault(z => z.TemplateId == templateId);
            if (existingTemplate != null)
                return new AddEditEmailTemplateModel(existingTemplate);
            else
                return null;
        }
    }
}
