using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;

namespace HyggeMail.BLL.Managers
{
    public class PaymentManager : BaseManager, IPaymentManager
    {
        public Payment MakePaymetWithPaypal(PayPalModel model)
        {
            var planDetails = Context.MembershipPlans.Find(model.PlanID);
            if (planDetails != null)
            {
                var discount = planDetails.Discount ?? 0;
                var rate = Math.Round(planDetails.Rate - ((planDetails.Rate * discount) / 100), 2);
                var itemList = new ItemList() { items = new List<Item>() };
                itemList.items.Add(new Item()
                {
                    name = planDetails.Name + " membership plan",
                    currency = model.Currency,
                    price = rate.ToString(),
                    quantity = "1"
                });

                var payer = new Payer() { payment_method = "paypal" };
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = model.CancelUrl,
                    return_url = model.ReturenUrl
                };
                var details = new Details()
               {
                   subtotal = rate.ToString(),
                   fee = "0",
                   gift_wrap = "0",
                   handling_fee = "0",
                   insurance = "0",
                   shipping = "0",
                   shipping_discount = "0",
                   tax = "0"
               };
                var amount = new Amount()
                {
                    currency = model.Currency,
                    total = rate.ToString(),
                    details = details
                };

                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = planDetails.Name + " membership subscription",
                    invoice_number = Convert.ToString(model.Id),
                    amount = amount,
                    item_list = itemList,


                });

                Payment payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls

                };
                return payment.Create(model.ApiContext);
            }
            return null;
        }

        public ActionOutput SavePaypalTransaction(Payment payment, int PlanID, string Gateway, decimal Amount, int UserID)
        {
            var planDetails = Context.MembershipPlans.Find(PlanID);
            var user = Context.Users.Find(UserID);
            if (planDetails != null)
            {
                var transaction = Context.UserTransactions.Create();
                transaction.Gateway = Gateway;
                transaction.Status = payment.state;
                transaction.TransactionAmount = Amount;
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionID = payment.id;
                transaction.UserID = UserID;
                transaction.PlanID = PlanID;
                var cardsCount = user.CardsCount;
                var planCards = planDetails.CardsAllocated;
                var newAllocatedCards = planCards + cardsCount;
                user.CardsCount = newAllocatedCards;
                user.UserHistories.Add(new UserHistory() { Type = "Transaction", Status = "Token Added", TokenChange = planCards.ToString(), TokenAvailable = user.CardsCount, AddedOn = DateTime.UtcNow });
                Context.UserTransactions.Add(transaction);
                Context.SaveChanges();
                return new ActionOutput { Message = planDetails.Name, Status = ActionStatus.Successfull, AvailableTokens = (int)user.CardsCount };
            }
            else
            {
                return new ActionOutput { Message = "Transaction Not Saved", Status = ActionStatus.Error };
            }
        }


        public PagingResult<PayPalTransaction> GetTransactionPagedList(PagingModel model)
        {
            var result = new PagingResult<PayPalTransaction>();
            var transactions = Context.UserTransactions.OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                var search = model.Search.ToLower();
                transactions = transactions.Where(z => z.TransactionID.ToLower().Contains(search) || z.User.FirstName.ToLower().Contains(search) || z.User.LastName.ToLower().Contains(search) || z.MembershipPlan.Name.ToLower().Contains(search));
            }
            if (model.UserID > 0)
            {
                transactions = transactions.Where(x => x.UserID == model.UserID);
            }
            var list = transactions
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new PayPalTransaction(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Transaction List";
            result.TotalCount = transactions.Count();
            return result;
        }

        public PayPalTransaction GetTransactionDetailsByUserID(int userID)
        {
            var transaction = Context.UserTransactions.OrderByDescending(m => m.TransactionDate).FirstOrDefault(m => m.UserID == userID);
            if (transaction != null)
                return new PayPalTransaction(transaction);
            else
            {
                //Check Basic membership
                var result = new PayPalTransaction();
                var basicmemeber = Context.Users.FirstOrDefault(x => x.UserID == userID && x.MemberShipType == 1);
                if (basicmemeber != null)
                {
                    if (basicmemeber.MemberShipType == 1)
                    {
                        result.IsBasic = true;
                        result.PendingCardCount = basicmemeber.CardsCount;
                    }
                }
                return result;
            }

        }

        public PayPalTransaction GetTransactionDetailsByTransID(int transID)
        {
            var transaction = Context.UserTransactions.OrderByDescending(m => m.TransactionDate).FirstOrDefault(m => m.ID == transID);
            if (transaction != null)
                return new PayPalTransaction(transaction);
            else
            {
                return null;
            }

        }
    }
}
