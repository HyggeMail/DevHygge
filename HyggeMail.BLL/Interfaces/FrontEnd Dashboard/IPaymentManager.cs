using HyggeMail.BLL.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Interfaces
{
    public interface IPaymentManager
    {
        Payment MakePaymetWithPaypal(PayPalModel model);

        ActionOutput SavePaypalTransaction(Payment payment, int PlanID, string Gateway, decimal Amount, int UserID);

        PagingResult<PayPalTransaction> GetTransactionPagedList(PagingModel model);

        PayPalTransaction GetTransactionDetailsByUserID(int userID);

        PayPalTransaction GetTransactionDetailsByTransID(int transID);
    }
}
