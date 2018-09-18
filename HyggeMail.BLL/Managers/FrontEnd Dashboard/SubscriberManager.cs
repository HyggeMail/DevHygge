using HyggeMail.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Models;
using System.Linq.Dynamic;
using HyggeMail.MailChimp;
namespace HyggeMail.BLL.Managers.FrontEnd_Dashboard
{
    public class SubscriberManager : BaseManager, ISubscriberManager
    {
        public PagingResult<SubscriberModel> GetSubscriberPagedList(PagingModel model, int category = 0)
        {
            var result = new PagingResult<SubscriberModel>();
            var query = Context.Subscribers.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.EmailID.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new SubscriberModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Subscriber List";
            result.TotalCount = query.Count();
            return result;
        }

        public ActionOutput SubmitSubscriberEmail(SubscriberModel SubscriberModel)
        {
            var existingSubscriber = Context.Subscribers.Where(z => z.IsDeleted == false && z.EmailID == SubscriberModel.EmailID).FirstOrDefault();
            if (existingSubscriber != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This Subscriber email already exists and is also not marked as deleted."
                };
            }
            else
            {
                var Subscriber = Context.Subscribers.Create();
                Subscriber.AddedOn = DateTime.UtcNow;
                Subscriber.IsDeleted = false;
                Subscriber.EmailID = SubscriberModel.EmailID;
                Context.Subscribers.Add(Subscriber);
                Context.SaveChanges();
                MailChimpService.AddOrUpdateListMember(subscriberEmail: SubscriberModel.EmailID, listId: System.Configuration.ConfigurationManager.AppSettings["SubListId"]);
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Subscriber Details Added Successfully."
                };
            }
        }

        public ActionOutput DeleteSubscriber(int SubscriberID)
        {
            var Subscriber = Context.Subscribers.Find(SubscriberID);
            if (Subscriber == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Subscriber email doesn't Exist."
                };
            }
            else
            {
                Subscriber.IsDeleted = true;
                Subscriber.DeletedOn = DateTime.Now.Date;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Subscriber email Deleted Successfully."
                };
            }
        }

        public SubscriberModel GetSubscriberDetailsByID(int SubscriberID)
        {
            var Subscriber = Context.Subscribers.Find(SubscriberID);
            return Subscriber == null ? null : new SubscriberModel(Subscriber);
        }
    }
}
