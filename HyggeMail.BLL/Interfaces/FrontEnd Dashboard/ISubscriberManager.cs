using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HyggeMail.BLL.Interfaces
{
    public interface ISubscriberManager
    {
        PagingResult<SubscriberModel> GetSubscriberPagedList(PagingModel model, int category);
        ActionOutput SubmitSubscriberEmail(SubscriberModel SubscriberModel);
        SubscriberModel GetSubscriberDetailsByID(int SubscriberID);
        ActionOutput DeleteSubscriber(int SubscriberID);
    }
}
