using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Interfaces
{
   public interface IContactUsManager
    {
        /// <summary>
        /// This will be used to get user listing model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PagingResult<WebContactUsModel> GetContactUsPagedList(PagingModel model, int userID);
  
        ActionOutput<WebContactUsModel> GetContactUsByID(int ContactUsID = 0);
        ActionOutput DeleteContactUsByID(int ContactUsID);
        ActionOutput ResolveContactUsByID(int ContactUsID);
        ActionOutput SetSeenAllRecord();
        ActionOutput GetUnseenContactUsCount();
    }
}
