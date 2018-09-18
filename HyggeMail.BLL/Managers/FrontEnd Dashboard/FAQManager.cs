using HyggeMail.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Models;
using System.Linq.Dynamic;

namespace HyggeMail.BLL.Managers
{
    public class FAQManager : BaseManager, IFAQManager
    {
        public PagingResult<FAQModel> GetFAQPagedList(PagingModel model, int category = 0)
        {
            var result = new PagingResult<FAQModel>();
            var query = Context.FAQs.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (category > 0)
            {
                query = query.Where(z => z.CategoryID == category);
            }
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.Title.Contains(model.Search) || z.Description.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new FAQModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Faq List";
            result.TotalCount = query.Count();
            return result;
        }

        public ActionOutput CreateFAQ(FAQModel faqModel)
        {
            var existingfaq = Context.FAQs.Where(z => z.Title.Trim().ToLower() == faqModel.Title.Trim().ToLower() && z.IsDeleted == false).FirstOrDefault();
            if (existingfaq != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This faq Name already exists and is also not marked as deleted."
                };
            }
            else
            {
                var faq = Context.FAQs.Create();
                faq.AddedOn = DateTime.UtcNow;
                faq.Description = faqModel.Description;
                faq.IsDeleted = false;
                faq.UpdatedOn = DateTime.UtcNow;
                faq.CategoryID = (int)faqModel.CategoryID;
                faq.Title = faqModel.Title;
                faq.Description = faqModel.Description;
                Context.FAQs.Add(faq);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "faq Details Added Successfully."
                };
            }
        }

        public ActionOutput UpdateFAQDetails(FAQModel faqModel)
        {
            var faq = Context.FAQs.FirstOrDefault(z => z.ID == faqModel.ID);
            if (faq == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "faq doesn't Exist."
                };
            }
            var existingfaq = Context.FAQs.FirstOrDefault(z => z.ID != faqModel.ID && z.Title == faqModel.Title && z.IsDeleted != true);
            if (existingfaq != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This faq Name already exists and is also not marked as deleted."
                };
            }
            else
            {
                faq.Description = faqModel.Description;
                faq.Title = faqModel.Title;
                faq.CategoryID = (int)faqModel.CategoryID;
                faq.IsDeleted = faqModel.IsDeleted;
                faq.Description = faqModel.Description;
                faq.UpdatedOn = DateTime.UtcNow;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "faq Details Updated Successfully."
                };
            }
        }

        public FAQModel GetFAQDetailsByID(int faqID)
        {
            var faq = Context.FAQs.Find(faqID);
            return faq == null ? null : new FAQModel(faq);
        }

        public ActionOutput DeleteFAQ(int faqID)
        {
            var faq = Context.FAQs.Find(faqID);
            if (faq == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "faq doesn't Exist."
                };
            }
            else
            {
                faq.IsDeleted = true;
                faq.DeletedOn = DateTime.Now.Date;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "faq Deleted Successfully."
                };
            }
        }
    }
}
