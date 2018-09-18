using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using AutoMapper;


namespace HyggeMail.BLL.Managers
{
    public class ContactUsManager : BaseManager, IContactUsManager
    {
        PagingResult<WebContactUsModel> IContactUsManager.GetContactUsPagedList(PagingModel model, int userID)
        {
            var result = new PagingResult<WebContactUsModel>();
            var query = Context.ContactUs.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (userID > 0)
                query = query.Where(c => c.UserID == userID);
            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.Name.Contains(model.Search) || z.Name.Contains(model.Search));
            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<ContactU>, List<WebContactUsModel>>(list.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Contact us List";
            result.TotalCount = query.Count();
            return result;
        }
        ActionOutput IContactUsManager.GetUnseenContactUsCount()
        {
            var ContactUs = Context.ContactUs.Where(x => x.IsSeenByAdmin != true);
            return new ActionOutput() { Message = "Record deleted successfully", Status = ActionStatus.Successfull, AvailableTokens = ContactUs.Count() };
        }
        ActionOutput IContactUsManager.SetSeenAllRecord()
        {
            var ContactUs = Context.ContactUs.Where(x => x.IsSeenByAdmin != true);
            ContactUs.ToList().ForEach(x => { x.IsSeenByAdmin = true; });
            Context.SaveChanges();
            return new ActionOutput() { Message = "Record deleted successfully", Status = ActionStatus.Successfull, AvailableTokens = ContactUs.Count() };
        }
        ActionOutput IContactUsManager.DeleteContactUsByID(int ContactUsID)
        {
            var ContactUs = Context.ContactUs.FirstOrDefault(c => c.ID == ContactUsID);
            ContactUs.IsDeleted = true;
            ContactUs.DeletedOn = DateTime.UtcNow;
            Context.SaveChanges();
            return new ActionOutput() { Message = "Record deleted successfully", Status = ActionStatus.Successfull };
        }
        ActionOutput IContactUsManager.ResolveContactUsByID(int ContactUsID)
        {
            var message = "";
            var ContactUs = Context.ContactUs.FirstOrDefault(c => c.ID == ContactUsID);
            ContactUs.IsResolved = true;
            ContactUs.ResolvedOn = DateTime.UtcNow;
            message = "Resolved successfully";
            Context.SaveChanges();
            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput<WebContactUsModel> IContactUsManager.GetContactUsByID(int ContactUsID = 0)
        {
            var recipient = Context.ContactUs.Where(z => z.ID == ContactUsID).FirstOrDefault();
            if (recipient != null)
            {
                var model = new WebContactUsModel();
                model = Mapper.Map<ContactU, WebContactUsModel>(recipient, model);
                return new ActionOutput<WebContactUsModel>() { Message = "Details", Status = ActionStatus.Successfull, Object = model };
            }
            else
            {
                return new ActionOutput<WebContactUsModel>()
                {
                    Status = ActionStatus.Error,
                    Message = "Record not exist."
                };
            }

        }
    }
}
