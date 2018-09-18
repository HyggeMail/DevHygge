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
    public class MembershipManager : BaseManager, IMembershipManager
    {

        public PagingResult<MembershipModel> GetPlansPagedList(PagingModel model)
        {
            var result = new PagingResult<MembershipModel>();
            var query = Context.MembershipPlans.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.Name.Contains(model.Search) || z.Description.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new MembershipModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Plans List";
            result.TotalCount = query.Count();
            return result;
        }

        public ActionOutput CreatePlan(MembershipModel planModel)
        {
            var existingPlan = Context.MembershipPlans.Where(z => z.Name.Trim().ToLower() == planModel.Name.Trim().ToLower() && z.IsDeleted == false).FirstOrDefault();
            if (existingPlan != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This plan Name already exists and is also not marked as deleted."
                };
            }
            else
            {
                var plan = Context.MembershipPlans.Create();
                plan.CardsAllocated = planModel.CardsAllocated;
                plan.CreatedOn = DateTime.Now.Date;
                plan.Description = planModel.Description;
                plan.Discount = planModel.Discount;
                plan.Isactive = true;
                plan.IsDeleted = false;
                plan.Name = planModel.Name;
                plan.Rate = planModel.Rate;
                Context.MembershipPlans.Add(plan);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Plan Details Added Successfully."
                };
            }
        }

        public ActionOutput UpdatePlanDetails(MembershipModel planModel)
        {
            var plan = Context.MembershipPlans.FirstOrDefault(z => z.ID == planModel.ID);
            if (plan == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Plan doesn't Exist."
                };
            }
            var existingPlan = Context.MembershipPlans.FirstOrDefault(z => z.ID != planModel.ID && z.Name == planModel.Name && z.IsDeleted != true);
            if (existingPlan != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This plan Name already exists and is also not marked as deleted."
                };
            }
            else
            {
                plan.CardsAllocated = planModel.CardsAllocated;
                plan.Description = planModel.Description;
                plan.Discount = planModel.Discount;
                plan.Isactive = planModel.Isactive;
                plan.IsDeleted = planModel.IsDeleted;
                plan.Name = planModel.Name;
                plan.Rate = planModel.Rate;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Plan Details Updated Successfully."
                };
            }
        }

        public MembershipModel GetPlanDetailsByID(int planID)
        {
            var plan = Context.MembershipPlans.Find(planID);
            return plan == null ? null : new MembershipModel(plan);
        }

        public ActionOutput DeletePlane(int planID)
        {
            var plan = Context.MembershipPlans.Find(planID);
            if (plan == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Plan doesn't Exist."
                };
            }
            else
            {
                plan.IsDeleted = true;
                plan.Isactive = false;
                plan.DeletedOn = DateTime.Now.Date;
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Plan Deleted Successfully."
                };
            }
        }

    }
}
