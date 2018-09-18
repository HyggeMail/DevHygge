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

namespace HyggeMail.BLL.Managers
{
    public class CMSManager : BaseManager, ICMSManager
    {

        PagingResult<CMSPageViewModel> ICMSManager.GetCMSPageList(Models.PagingModel model)
        {
            var result = new PagingResult<CMSPageViewModel>();
            var query = Context.CMSPages.OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.PageName.Contains(model.Search) || z.PageTitle.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new CMSPageViewModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Page List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput ICMSManager.UpdatePageContent(EditCMSPageModel pageContent)
        {
            var existingPage = Context.CMSPages.FirstOrDefault(z => z.PageId == pageContent.PageId);
            if (existingPage == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Page not exists."
                };
            }
            else
            {
                existingPage.PageName = pageContent.PageName;
                existingPage.PageTitle = pageContent.PageTitle;
                existingPage.PageContent = pageContent.PageContent;
                existingPage.MetaTitle = pageContent.MetaTitle;
                existingPage.MetaKeywords = pageContent.MetaKeywords;
                existingPage.MetaDescription = pageContent.MetaDescription;
                existingPage.UpdatedOn = DateTime.UtcNow;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "CMS Page Updated Sucessfully."
                };
            }

        }

        EditCMSPageModel ICMSManager.GetPageContentByPageId(int pageId)
        {
            var existingPage = Context.CMSPages.FirstOrDefault(z => z.PageId == pageId);
            if (existingPage != null)
                return new EditCMSPageModel(existingPage);
            else
                return null;
        }
        AdminDashboardCountModel ICMSManager.GetAdminDashboardDetails()
        {
            AdminDashboardCountModel model = new AdminDashboardCountModel();
            model.Users = new AdminDashboardDetails();
            model.Orders = new AdminDashboardDetails();
            model.TotalIncome = new AdminDashboardDetails();
            model.Messages = new AdminDashboardDetails();
            var currentWeek = Utilities.GetCurrentWeek();
            var currentMonth = Utilities.GetCurrentMonth();
            var currentYear = Utilities.GetCurrentYear();
            var users = Context.Users;
            var orders = Context.UserOrders;
            var Income = Context.UserTransactions.Where(x => x.Status.ToLower() == "approved");
            var messages = Context.ContactUs;

            //Bind users count.
            model.Users.Daily = Convert.ToString(users.Where(j => j.ActivatedOn.Value.Year == DateTime.UtcNow.Year
                       && j.ActivatedOn.Value.Month == DateTime.UtcNow.Month
                       && j.ActivatedOn.Value.Day == DateTime.UtcNow.Day).Count());
            model.Users.Weekly = Convert.ToString(users.Where(x => x.ActivatedOn.Value > currentWeek.Start).Count());
            model.Users.Monthly = Convert.ToString(users.Where(x => x.ActivatedOn.Value > currentMonth.Start).Count());
            model.Users.Yearly = Convert.ToString(users.Where(x => x.ActivatedOn.Value > currentYear.Start).Count());
            model.Users.Total = Convert.ToString(users.Count());

            //Bind Orders
            model.Orders.Daily = Convert.ToString(orders.Where(j => j.UserPostCard.AddedOn.Value.Year == DateTime.UtcNow.Year
                       && j.UserPostCard.AddedOn.Value.Month == DateTime.UtcNow.Month
                       && j.UserPostCard.AddedOn.Value.Day == DateTime.UtcNow.Day && j.UserPostCard.IsApproved != true).Count());
            model.Orders.Weekly = Convert.ToString(orders.Where(x => x.UserPostCard.AddedOn.Value > currentWeek.Start).Count());
            model.Orders.Monthly = Convert.ToString(orders.Where(x => x.UserPostCard.AddedOn.Value > currentMonth.Start).Count());
            model.Orders.Yearly = Convert.ToString(orders.Where(x => x.UserPostCard.AddedOn.Value > currentYear.Start).Count());
            model.Orders.Total = Convert.ToString(orders.Count());

            //Bind Income
            model.TotalIncome.Daily = Convert.ToString(Income.Where(j => j.TransactionDate.Value.Year == DateTime.UtcNow.Year
                       && j.TransactionDate.Value.Month == DateTime.UtcNow.Month
                       && j.TransactionDate.Value.Day == DateTime.UtcNow.Day).Sum(x => x.TransactionAmount));
            model.TotalIncome.Weekly = Convert.ToString(Income.Where(x => x.TransactionDate.Value > currentWeek.Start).Sum(x => x.TransactionAmount));
            model.TotalIncome.Monthly = Convert.ToString(Income.Where(x => x.TransactionDate.Value > currentMonth.Start).Sum(x => x.TransactionAmount));
            model.TotalIncome.Yearly = Convert.ToString(Income.Where(x => x.TransactionDate.Value > currentYear.Start).Sum(x => x.TransactionAmount));
            model.TotalIncome.Total = Convert.ToString(Income.Sum(x => x.TransactionAmount));

            //Bind Messages count
            model.Messages.Daily = Convert.ToString(messages.Where(j => j.AddedOn.Value.Year == DateTime.UtcNow.Year
                       && j.AddedOn.Value.Month == DateTime.UtcNow.Month
                       && j.AddedOn.Value.Day == DateTime.UtcNow.Day).Count());
            model.Messages.Weekly = Convert.ToString(messages.Where(x => x.AddedOn.Value > currentWeek.Start).Count());
            model.Messages.Monthly = Convert.ToString(messages.Where(x => x.AddedOn.Value > currentMonth.Start).Count());
            model.Messages.Yearly = Convert.ToString(messages.Where(x => x.AddedOn.Value > currentYear.Start).Count());
            model.Messages.Total = Convert.ToString(messages.Count());
            model.Messages.Unresolved = Convert.ToString(messages.Where(c => c.IsResolved != true).Count());

            return model;
        }
        EditCMSPageModel ICMSManager.GetPageContentByPageType(int pageType)
        {
            var existingPage = Context.CMSPages.FirstOrDefault(z => z.PageType == pageType);
            if (existingPage != null)
                return new EditCMSPageModel(existingPage);
            else
                return null;
        }
    }
}
