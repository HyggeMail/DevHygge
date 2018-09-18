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

    public interface IRecipientManager
    {
        /// <summary>
        /// This will be used to get user listing model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PagingResult<RecipientListingModel> GetRecipientPagedList(PagingModel model, int userID);

        /// <summary>
        /// Update User Details from Admin Panel
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        ActionOutput<RecipientDetails> AddUpdateRecipient(AddUpdateRecipientModel model);

        ActionOutput<RecipientGroupModel> GetRecipientByAlphabetic(int userID = 0, string keyword = "");
        ActionOutput<RecipientDetails> GetUserRecipients(int userID = 0, string keyword = "");
        ActionOutput DeleteRecipientByID(int userId, int recipientID);
        ActionOutput<AddUpdateRecipientModel> GetRecipientByID(int userID = 0, int recipientID = 0);
        ActionOutput<RecipientGroupModel> GetPostCardRecipientByAlphabetic(int cid, int userID = 0, string keyword = "", bool IsAddedByadmin = false);
      //  ActionOutput<RecipientGroupModel> GetAdminAddressBookByAlphabetic(int cid, int userID = 0, string keyword = "");
        string GetRejectionReasonByCardId(int id);
    }
}
