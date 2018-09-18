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
    public interface IEditorManager
    {
        /// <summary>
        /// This will be used to get user listing model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PagingResult<PostCardListingModel> GetPostCardPagedList(OrderPagingModel model, int userID);
        PagingResult<RecipientPostCardListingModel> GetPostCardOrdersPaggedList(RecipientOrderPagingModel model, int userID);
        /// <summary>
        /// Update User Details from Admin Panel
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        ActionOutput<PostCardResultModel> AddUpdatePostCard(AddUpdateImageEditorModel model);

        ActionOutput<AddUpdateImageEditorModel> GetPostCardDetailsByID(int PostCardID = 0, int userID = 0);
        ActionOutput DeletePostCardByID(int PostCardID, int userID = 0);
        ActionOutput CancelPostCardByID(int PostCardID);
        ActionOutput RejectPostCardByID(int PostCardID);
        ActionOutput ApprovePostCardByID(int PostCardID);
        ActionOutput UpdateOrderStatus(int PostCardID, short Status);
        ActionOutput ApproveReceiptent(int receiptentID);
        ActionOutput SentToError(int receiptentID);
        ActionOutput DispproveReceiptent(int receiptentID);
        ActionOutput CompletePostCard(int ReceiptentID);
        ActionOutput CompleteRecipientPostCard(int ReceiptentID);
        ActionOutput RejectWithReason(RejectWithReasonModel model);
        ActionOutput GetPostCardBackSideJsonResult(int ReceiptentID);
        PagingResult<RecipientPostCardListingModel> GetMyPostCardOrdersPaggedList(RecipientOrderPagingModel model, int userID);
        ActionOutput<AddUpdateImageEditorModel> GetDemoPostCardListing();
    }
}
