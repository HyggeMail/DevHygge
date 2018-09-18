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
using System.Web.Mvc;
using System.Data.Entity.Validation;
using AutoMapper;
using System.Drawing.Imaging;
using System.Web;
using System.IO;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;


namespace HyggeMail.BLL.Managers
{
    public class EditorManager : BaseManager, IEditorManager
    {
        PagingResult<PostCardListingModel> IEditorManager.GetPostCardPagedList(OrderPagingModel model, int userID)
        {
            var result = new PagingResult<PostCardListingModel>();
            if (string.IsNullOrEmpty(model.SortBy))
            {
                model.SortBy = "AddedOn";
                model.SortOrder = "Desc";
            }
            var query = Context.UserPostCards.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (userID > 0)
                query = query.Where(c => c.UserID == userID);

            if (model.Status != null)
            {
                if (model.Status == eAdminPostOrderStatus.Approved)
                    query = query.Where(c => c.IsApproved == true);
                else if (model.Status == eAdminPostOrderStatus.Completed)
                    query = query.Where(c => c.IsCompleted == true);

            }
            if (model.IsOrderPlaced > 0)
                query = query.Where(c => c.IsOrderPlaced == true);

            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.User.FirstName.Contains(model.Search) || z.User.Email.Contains(model.Search));
            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<UserPostCard>, List<PostCardListingModel>>(list.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Post Card List";
            result.TotalCount = query.Count();
            return result;
        }

        PagingResult<RecipientPostCardListingModel> IEditorManager.GetPostCardOrdersPaggedList(RecipientOrderPagingModel model, int userID)
        {
            var currentTime = DateTime.UtcNow;
            var result = new PagingResult<RecipientPostCardListingModel>();
            if (string.IsNullOrEmpty(model.SortBy))
            {
                model.SortBy = "AddedOn";
                model.SortOrder = "Desc";
            }
            var query = Context.UserPostCardRecipients.Where(x => (x.UserPostCard.IsOrderPlaced == true &&
                DbFunctions.TruncateTime(DbFunctions.AddMinutes(x.UserPostCard.OrderPlacedOn, 15)) <= currentTime)).OrderBy(model.SortBy + " " + model.SortOrder);

            //var query = from o in Context.UserPostCardRecipients
            //            where DbFunctions.TruncateTime(o.UserPostCard.OrderPlacedOn) != null
            //           && DbFunctions.AddMinutes(o.UserPostCard.OrderPlacedOn, 15) <= currentTime                      
            //            select o;

            if (userID > 0)
                query = query.Where(c => c.UserPostCard.UserID == userID).OrderBy(model.SortBy + " " + model.SortOrder);

            if (userID == 0)
            {
                if (model.shownByStatus == eRecipientOrderStatus.Approved)
                {
                    if (model.shownsBydays == eShownRecipientOrdersByDays.Past)
                        query = query.OrderBy(model.SortBy + " " + model.SortOrder).Where(j => j.UserPostCard.ShipmentDate < DbFunctions.TruncateTime(DateTime.UtcNow));
                    else if (model.shownsBydays == eShownRecipientOrdersByDays.Today)
                        query = query.Where(j => j.UserPostCard.ShipmentDate.Value.Year == DateTime.UtcNow.Year
                               && j.UserPostCard.ShipmentDate.Value.Month == DateTime.UtcNow.Month
                               && j.UserPostCard.ShipmentDate.Value.Day == DateTime.UtcNow.Day);
                    else if (model.shownsBydays == eShownRecipientOrdersByDays.Future)
                        query = query.Where(c => c.UserPostCard.ShipmentDate > DateTime.UtcNow);
                }

                //if (model.shownByStatus == eRecipientOrderStatus.New)
                //    query = query.Where(c => c.IsApproved == false && c.IsCompleted == false && c.IsError == null);
                //else if (model.shownByStatus == eRecipientOrderStatus.Approved)
                //    query = query.Where(c => c.IsApproved == true);
                //else if (model.shownByStatus == eRecipientOrderStatus.Completed)
                //    query = query.Where(c => c.IsCompleted == true && c.IsRejected != true);
                //else if (model.shownByStatus == eRecipientOrderStatus.Rejected)
                //    query = query.Where(c => c.IsRejected == true);
                //else if (model.shownByStatus == eRecipientOrderStatus.Errors)
                //    query = query.Where(c => c.IsError == true);


                if (model.shownByStatus == eRecipientOrderStatus.New)
                    query = query.Where(c => c.CardStatus == (int)CardStatusTypes.InProgress);
                else if (model.shownByStatus == eRecipientOrderStatus.Approved)
                    query = query.Where(c => c.CardStatus == (int)CardStatusTypes.Approved);
                else if (model.shownByStatus == eRecipientOrderStatus.Completed)
                    query = query.Where(c => c.CardStatus == (int)CardStatusTypes.Completed);
                else if (model.shownByStatus == eRecipientOrderStatus.Rejected)
                    query = query.Where(c => c.CardStatus == (int)CardStatusTypes.Rejected);
                else if (model.shownByStatus == eRecipientOrderStatus.Errors)
                    query = query.Where(c => c.CardStatus == (int)CardStatusTypes.Error);

            }
            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.UserPostCard.User.FirstName.Contains(model.Search) || z.UserPostCard.User.Email.Contains(model.Search) || z.ID.ToString().Contains(model.Search));

            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<UserPostCardRecipient>, List<RecipientPostCardListingModel>>(list.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Post Card Order List";
            result.TotalCount = query.Count();
            return result;
        }

        PagingResult<RecipientPostCardListingModel> IEditorManager.GetMyPostCardOrdersPaggedList(RecipientOrderPagingModel model, int userID)
        {
            var result = new PagingResult<RecipientPostCardListingModel>();
            if (string.IsNullOrEmpty(model.SortBy))
            {
                model.SortBy = "AddedOn";
                model.SortOrder = "Desc";
            }
            var query = Context.UserPostCardRecipients.Where(c => c.UserPostCard.UserID == userID && c.UserPostCard.IsOrderPlaced == true).OrderBy(model.SortBy + " " + model.SortOrder);

            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.UserPostCard.User.FirstName.Contains(model.Search) || z.UserPostCard.User.Email.Contains(model.Search) || z.ID.ToString().Contains(model.Search));

            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<UserPostCardRecipient>, List<RecipientPostCardListingModel>>(list.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Post Card Order List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput<PostCardResultModel> IEditorManager.AddUpdatePostCard(AddUpdateImageEditorModel model)
        {
            IUserManager _um = new UserManager();

            var user = Context.Users.FirstOrDefault(x => x.UserID == model.UserID);
            var userCardsLeft = user.CardsCount;
            var id = 0;
            var resultObject = new PostCardResultModel();
            var message = "";
            if (userCardsLeft > 0)
            {
                try
                {
                    if (model.IsCopyCard)
                        model.ID = 0;

                    if (model.ID > 0)
                    {
                        var PostCard = Context.UserPostCards.Where(z => z.ID == model.ID && z.IsDeleted != true).FirstOrDefault();

                        var rootPath = AttacmentsPath.UserProfileImages + user.FirstName.Replace(" ", "") + "-" + user.UserID + "/PostCards/";

                        //delete the existed files
                        if (!string.IsNullOrEmpty(PostCard.CardFrontPath))
                        {
                            var file = PostCard.CardFrontPath.Split('/')[5];
                            var path = HttpContext.Current.Server.MapPath(rootPath + file);
                            if (File.Exists(path))
                                File.Delete(path);
                        }

                        if (!string.IsNullOrEmpty(PostCard.CardBackPath))
                        {
                            var file = PostCard.CardBackPath.Split('/')[5];
                            var path = HttpContext.Current.Server.MapPath(rootPath + file);
                            if (File.Exists(path))
                                File.Delete(path);
                        }

                        if (!string.IsNullOrEmpty(PostCard.CardBackPathWithIFrame))
                        {
                            var file = PostCard.CardBackPathWithIFrame.Split('/')[5];
                            var path = HttpContext.Current.Server.MapPath(rootPath + file);
                            if (File.Exists(path))
                                File.Delete(path);
                        }

                        PostCard = Mapper.Map<AddUpdateImageEditorModel, UserPostCard>(model, PostCard);
                        if (PostCard.IsOrderPlaced == true)
                        {
                            PostCard.IsOrderPlaced = true;
                            PostCard.OrderPlacedOn = DateTime.UtcNow;
                            IEmailManager email = new EmailManager();
                            email.SendOrderStatusToAdmin("Placed", user);
                        }
                        if (!string.IsNullOrEmpty(model.CardFrontJson))
                        {
                            var frontUID = Guid.NewGuid().ToString();
                            var imageFront = Utilities.Base64ToImage(model.CardFront);

                            PostCard.ShipmentDate = model.ShipmentDate;
                            var frontImageName = Guid.NewGuid().ToString() + "-front" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageFront.Save(HttpContext.Current.Server.MapPath(rootPath) + frontImageName);

                            PostCard.CardFrontPath = rootPath + frontImageName;
                            PostCard.CardFront = null;
                        }

                        if (!string.IsNullOrEmpty(model.CardBackJson))
                        {
                            var backUID = Guid.NewGuid().ToString();
                            var imageBack = Utilities.Base64ToImage(model.CardBack);

                            var backImageName = Guid.NewGuid().ToString() + "-back" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageBack.Save(HttpContext.Current.Server.MapPath(rootPath) + backImageName);

                            PostCard.CardBackPath = rootPath + backImageName;
                            PostCard.CardBack = null;
                        }


                        if (!string.IsNullOrEmpty(model.CardBackJsonWithIFrame))
                        {
                            var backframeUID = Guid.NewGuid().ToString();
                            var imageBackWithFrame = Utilities.Base64ToImage(model.CardBackWithFrame);

                            var backImageName = Guid.NewGuid().ToString() + "-backWithFrame" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageBackWithFrame.Save(HttpContext.Current.Server.MapPath(rootPath) + backImageName);

                            PostCard.CardBackPathWithIFrame = rootPath + backImageName;
                        }

                        if (PostCard.UserPostCardRecipients != null && PostCard.UserPostCardRecipients.Count > 0)
                        {
                            foreach (var item in PostCard.UserPostCardRecipients)
                                Context.UserHistories.RemoveRange(item.UserHistories);
                        }
                        Context.UserPostCardRecipients.RemoveRange(PostCard.UserPostCardRecipients);
                        if (model.Recipients.Count > 0)
                        {
                            foreach (var item in model.Recipients)
                            {
                                if (user.CardsCount > 0)
                                {
                                    var recipient = new UserPostCardRecipient();
                                    recipient = Mapper.Map<UserRecipientModel, UserPostCardRecipient>(item, recipient);
                                    recipient.FKUserAddressBookId = item.ID;
                                    recipient.AddedOn = DateTime.UtcNow;
                                    recipient.IsCompleted = false;
                                    recipient.IsApproved = false;
                                    recipient.CardStatus = (int)CardStatusTypes.InProgress;
                                    PostCard.UserPostCardRecipients.Add(recipient);
                                    recipient.UserHistories.Add(new UserHistory() { UserFK = model.UserID, Type = "Order Placed", TokenChange = "-1", AddedOn = DateTime.UtcNow });

                                    if (model.IsOrderPlaced)
                                        user.CardsCount--; //27-feb-2018
                                }
                                else
                                {
                                    return new ActionOutput<PostCardResultModel>
                                    {
                                        Status = ActionStatus.Error,
                                        Message = "Your pending tokens are less than the recipient you added. Please add tokens for new orders."
                                    };
                                }
                            }
                        }
                        PostCard.AddedOn = DateTime.UtcNow;
                        // Context.UserPostCards.Add(PostCard);
                        //       Context.UserPostCardRecipients.AddRange(PostCard.UserPostCardRecipients);
                        message = "Postcard details updated successfully.";
                        id = PostCard.ID;
                        Context.SaveChanges();
                        resultObject = new PostCardResultModel(PostCard);
                    }
                    else
                    {

                        var rootPath = AttacmentsPath.UserProfileImages + user.FirstName.Replace(" ", "") + "-" + user.UserID + "/PostCards/";
                        var postCard = new UserPostCard();
                        UserPostCard book = Mapper.Map<AddUpdateImageEditorModel, UserPostCard>(model);
                        book.UpdatedOn = DateTime.UtcNow;
                        book.AddedOn = DateTime.UtcNow;
                        if (book.IsOrderPlaced == true)
                        {
                            book.IsOrderPlaced = true;
                            book.OrderPlacedOn = DateTime.UtcNow;
                            IEmailManager email = new EmailManager();
                            email.SendOrderStatusToAdmin("Placed", user);
                        }
                        book.ShipmentDate = model.ShipmentDate;
                        book.IsDeleted = false;
                        if (!string.IsNullOrEmpty(model.CardFrontJson))
                        {
                            var frontUID = Guid.NewGuid().ToString();
                            var imageFront = Utilities.Base64ToImage(model.CardFront);

                            var frontImageName = Guid.NewGuid().ToString() + "-front" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageFront.Save(HttpContext.Current.Server.MapPath(rootPath) + frontImageName);

                            book.CardFrontPath = rootPath + frontImageName;
                            book.CardFront = null;
                        }

                        if (!string.IsNullOrEmpty(model.CardBackJson))
                        {
                            var backUID = Guid.NewGuid().ToString();
                            var imageBack = Utilities.Base64ToImage(model.CardBack);

                            var backImageName = Guid.NewGuid().ToString() + "-back" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageBack.Save(HttpContext.Current.Server.MapPath(rootPath) + backImageName);

                            book.CardBackPath = rootPath + backImageName;
                            book.CardBack = null;
                        }


                        if (!string.IsNullOrEmpty(model.CardBackJsonWithIFrame))
                        {
                            var backframeUID = Guid.NewGuid().ToString();
                            var imageBackWithFrame = Utilities.Base64ToImage(model.CardBackWithFrame);

                            var backImageName = Guid.NewGuid().ToString() + "-backWithFrame" + ".png";

                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                            imageBackWithFrame.Save(HttpContext.Current.Server.MapPath(rootPath) + backImageName);

                            book.CardBackPathWithIFrame = rootPath + backImageName;
                        }

                        if (model.Recipients.Count > 0)
                        {
                            foreach (var item in model.Recipients)
                            {
                                if (user.CardsCount > 0)
                                {
                                    var recipient = new UserPostCardRecipient();
                                    recipient = Mapper.Map<UserRecipientModel, UserPostCardRecipient>(item, recipient);
                                    recipient.FKUserAddressBookId = item.ID;
                                    recipient.AddedOn = DateTime.UtcNow;
                                    recipient.IsCompleted = false;
                                    recipient.IsApproved = false;
                                    recipient.CardStatus = (int)CardStatusTypes.InProgress;
                                    book.UserPostCardRecipients.Add(recipient);
                                    recipient.UserHistories.Add(new UserHistory() { UserFK = model.UserID, Type = "Order Placed", TokenChange = "-1", AddedOn = DateTime.UtcNow });

                                    if (model.IsOrderPlaced)
                                        user.CardsCount--; //27-feb-2018
                                }
                                else
                                {
                                    return new ActionOutput<PostCardResultModel>
                      {
                          Status = ActionStatus.Error,
                          Message = "Your pending tokens are less than the recipient you added. Please add tokens for new orders."
                      };
                                }
                            }
                        }
                        if (model.SelectedImages != null)
                        {
                            if (model.SelectedImages.Count > 0)
                            {
                                foreach (var item in model.SelectedImages)
                                {
                                    var postCardImage = new UserPostCardImage();
                                    postCardImage = Mapper.Map<PostCardSelectedImages, UserPostCardImage>(item, postCardImage);
                                    postCardImage.UsedOn = DateTime.UtcNow;
                                    postCardImage.UsedBy = model.UserID;
                                    book.UserPostCardImages.Add(postCardImage);
                                }
                            }
                        }
                        book.UserOrder = new UserOrder();
                        book.UserOrder.OrderStatus = (short)eOrderStatus.OrderPlaced;
                        //  user.CardsCount--;
                        Context.UserPostCards.Add(book);
                        Context.SaveChanges();

                        message = "Postcard details added successfully.";
                        id = book.ID;
                        resultObject = new PostCardResultModel(book);
                    }

                    if (user.OrderPlacedNotification == true && user.OrderPlacedNotification != null)
                    {
                        if (model.IsOrderPlaced == true)
                        {

                            IEmailManager _emailManager = new EmailManager();
                            _emailManager.SendOrderPlacedForUser(user);
                        }
                    }
                    return new ActionOutput<PostCardResultModel>
                    {
                        Object = resultObject,
                        Status = ActionStatus.Successfull,
                        Message = message
                    };
                }
                catch (Exception ex)
                {
                    IErrorLogManager er = new ErrorLogManager();
                    er.LogExceptionToDatabase(ex);
                    return new ActionOutput<PostCardResultModel>
                    {
                        Status = ActionStatus.Error,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ActionOutput<PostCardResultModel>
                {
                    Status = ActionStatus.Error,
                    Message = "You have used all your cards. Please purchase a new plan for more cards."
                };
            }

        }

        ActionOutput<AddUpdateImageEditorModel> IEditorManager.GetPostCardDetailsByID(int PostCardID = 0, int userID = 0)
        {
            var result = new ActionOutput<AddUpdateImageEditorModel>();
            if (PostCardID > 0)
            {
                var postcard = Context.UserPostCards.FirstOrDefault(x => x.ID == PostCardID);
                if (postcard != null)
                {
                    if (userID > 0 && postcard.UserID != userID)
                        return new ActionOutput<AddUpdateImageEditorModel>() { Message = "No acess for this record.", Status = ActionStatus.Error };
                    var model = new AddUpdateImageEditorModel();
                    model = Mapper.Map<UserPostCard, AddUpdateImageEditorModel>(postcard, model);
                    model.Recipients = Mapper.Map<List<UserPostCardRecipient>, List<UserRecipientModel>>(postcard.UserPostCardRecipients.ToList(), model.Recipients.ToList());
                    result.Object = model;
                    result.Message = "Post card details";
                    result.Status = ActionStatus.Successfull;
                }
                else
                {
                    result.Message = "No Record found.";
                    result.Status = ActionStatus.Error;
                }
            }
            return result;
        }

        ActionOutput<AddUpdateImageEditorModel> IEditorManager.GetDemoPostCardListing()
        {
            var emailId = Config.DemoPostCardEmail;

            var result = new ActionOutput<AddUpdateImageEditorModel>();
            if (!string.IsNullOrEmpty(emailId))
            {
                var postcard = Context.UserPostCards.Where(x => x.IsDeleted != true && x.User.Email == emailId).OrderByDescending(x => Guid.NewGuid()).Take(8).ToList();
                if (postcard.Count > 0)
                {
                    var model = new List<AddUpdateImageEditorModel>();
                    model = Mapper.Map<List<UserPostCard>, List<AddUpdateImageEditorModel>>(postcard.ToList(), model);

                    result.List = model.ToList();
                }

                result.Message = string.Empty;
                result.Status = ActionStatus.Successfull;
            }
            return result;
        }

        ActionOutput IEditorManager.DeletePostCardByID(int PostCardID, int userID = 0)
        {
            var postcard = Context.UserPostCards.FirstOrDefault(c => c.ID == PostCardID);
            if (userID > 0 && postcard.UserID != userID)
                return new ActionOutput() { Message = "No access for this postcard.", Status = ActionStatus.Error };
            postcard.IsDeleted = true;
            postcard.DeletedOn = DateTime.UtcNow;
            Context.SaveChanges();
            return new ActionOutput() { Message = "Post card deleted successfully", Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.RejectWithReason(RejectWithReasonModel model)
        {
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == model.RecipientCardID);
            var msg = "";
            if (postcard.IsRejected == true)
            {
                postcard.CardStatus = PreviousCardStatus(ref postcard);
                postcard.IsRejected = false;
            }

            else
            {
                postcard.IsRejected = true;
                postcard.IsApproved = false;
                postcard.IsCompleted = false;
                postcard.IsError = false;
                postcard.RejectedOn = DateTime.UtcNow;
                postcard.RejectedReason = model.Reason;
                postcard.CardStatus = (int)CardStatusTypes.Rejected;
                msg = "Rejected successfully";
                IEmailManager _em = new EmailManager();
                if (postcard.UserPostCard.User.OrderStatusNotification != null && postcard.UserPostCard.User.OrderStatusNotification == true)
                {
                    _em.SendRejectionEmailToUser(postcard.ID, model.Reason);
                }
                postcard.UserHistories.Add(new UserHistory() { UserFK = postcard.UserPostCard.UserID, Type = "Order Rejected", TokenChange = "+1", AddedOn = DateTime.UtcNow, TokenAvailable = postcard.UserPostCard.User.CardsCount });

                if (postcard.IsError == false || postcard.IsError == null)
                    postcard.UserPostCard.User.CardsCount += 1;
            }
            Context.SaveChanges();
            return new ActionOutput() { Message = msg, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.ApprovePostCardByID(int PostCardID)
        {
            IEmailManager _emailManager = new EmailManager();
            var status = "";

            var message = "";
            var postcard = Context.UserPostCards.FirstOrDefault(c => c.ID == PostCardID);
            if (postcard.IsApproved == true)
            {
                postcard.IsApproved = false;
                postcard.ApprovedOn = null;

                status = "disapproved";
                message = "Post card disapproved successfully";
            }
            else
            {
                postcard.IsApproved = true;
                postcard.ApprovedOn = DateTime.UtcNow;

                status = "approved";
                message = "Post card approved successfully";
            }
            Context.SaveChanges();
            if (postcard.User.OrderPlacedNotification == true && postcard.User.OrderPlacedNotification != null)
            {
                _emailManager.SendOrderStatusChangeMailForUser(postcard.User, status);
            }
            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.RejectPostCardByID(int PostCardID)
        {
            IEmailManager _emailManager = new EmailManager();
            var status = "";
            var message = "";
            var postcard = Context.UserPostCards.FirstOrDefault(c => c.ID == PostCardID);
            if (postcard.IsRejected == true)
            {
                postcard.IsRejected = false;
                postcard.RejectedOn = null;
                message = "Rejection removed successfully";
            }
            else
            {
                postcard.IsRejected = true;
                postcard.RejectedOn = DateTime.UtcNow;
                message = "Post card rejected successfully";
                status = "Rejected";

            }
            if (postcard.User.OrderPlacedNotification == true && postcard.User.OrderPlacedNotification != null)
                _emailManager.SendOrderStatusChangeMailForUser(postcard.User, status);
            Context.SaveChanges();
            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.UpdateOrderStatus(int orderID, short Status)
        {
            IEmailManager _emailManager = new EmailManager();
            eOrderStatus status = (eOrderStatus)Status;

            var result = new ActionOutput();
            var order = Context.UserOrders.FirstOrDefault(c => c.ID == orderID);
            if (order != null)
            {
                order.OrderStatus = Status;
                Context.SaveChanges();
                result.Message = "Order status updated successfully";
                result.Status = ActionStatus.Successfull;
            }
            else
            {
                result.Message = "No record found";
                result.Status = ActionStatus.Error;
            }
            if (order.UserPostCard.User.OrderPlacedNotification == true && order.UserPostCard.User.OrderPlacedNotification != null)
                _emailManager.SendOrderStatusChangeMailForUser(order.UserPostCard.User, status.ToString());

            return result;
        }

        ActionOutput IEditorManager.CancelPostCardByID(int PostCardID)
        {
            IEmailManager _emailManager = new EmailManager();

            ActionOutput result = new ActionOutput();
            result.Message = "Post card request cancelled successfully";
            var postcard = Context.UserPostCards.FirstOrDefault(c => c.ID == PostCardID);
            if (postcard != null)
            {
                if (postcard.IsApproved == true)
                {
                    result.Message = "Card already approved by the admin.";
                    result.Status = ActionStatus.Failed;
                }
                if (postcard.IsCancel == true)
                {
                    result.Message = "Card already cancelled by you.";
                    result.Status = ActionStatus.Failed;
                }
                if (postcard.IsCompleted == true)
                {
                    result.Message = "Card already completed by the admin.";
                    result.Status = ActionStatus.Failed;
                }
                if (postcard.IsDeleted == true)
                {
                    result.Message = "You can not cancel the deleted card.";
                    result.Status = ActionStatus.Failed;
                }
                if (postcard.IsRejected == true)
                {
                    result.Message = "Card already rejected by the admin.";
                    result.Status = ActionStatus.Failed;
                }

                if (postcard.AddedOn.Value.AddMinutes(15) >= DateTime.UtcNow)
                {
                    postcard.IsCancel = true;
                    postcard.CancelledOn = DateTime.UtcNow;
                    foreach (var item in postcard.UserPostCardRecipients)
                    {
                        item.IsCancelled = true;
                        item.CancelledOn = DateTime.UtcNow;
                        item.CardStatus = (int)CardStatusTypes.Cancelled;
                        postcard.User.CardsCount += 1;
                    }
                    Context.SaveChanges();
                    //if (postcard.User.OrderPlacedNotification == true && postcard.User.OrderPlacedNotification != null)
                    //    _emailManager.SendOrderStatusChangeMailForUser(postcard.User, "Cancelled");

                    _emailManager.SendOrderStatusToAdmin("Cancelled", postcard.User);

                    result.Status = ActionStatus.Successfull;
                }
                else
                {
                    result.Message = "Time expired and now your order is placed.";
                    result.Status = ActionStatus.Failed;
                }

            }
            else
            {
                result.Message = "No record found.";
                result.Status = ActionStatus.Failed;
            }
            return result;
        }

        ActionOutput IEditorManager.ApproveReceiptent(int ReceiptentID)
        {
            IEmailManager _emailManager = new EmailManager();

            var message = "";
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == ReceiptentID);
            if (postcard.IsApproved == false || postcard.IsApproved == null)
            {
                postcard.IsApproved = true;
                postcard.IsCompleted = false;
                postcard.IsError = false;
                postcard.IsRejected = false;
                postcard.CardStatus = (int)CardStatusTypes.Approved;
                message = "Post card recipient approved successfully";
                //if (postcard.UserPostCard.User.OrderPlacedNotification == true && postcard.UserPostCard.User.OrderPlacedNotification != null)
                //    _emailManager.SendOrderStatusChangeMailForUser(postcard.UserPostCard.User, "recipient approved");

                postcard.UserHistories.Add(new UserHistory() { UserFK = postcard.UserPostCard.UserID, Type = "Order", Status = "Order Approved", TokenChange = "", AddedOn = DateTime.UtcNow, TokenAvailable = postcard.UserPostCard.User.CardsCount });
            }
            else
            {
                if (postcard.IsApproved == true)
                {
                    postcard.CardStatus = PreviousCardStatus(ref postcard);
                    postcard.IsApproved = false;
                    postcard.IsError = false;
                    postcard.IsRejected = false;
                    message = "Post card recipients disapproved successfully";
                }
                //if (postcard.UserPostCard.User.OrderPlacedNotification == true && postcard.UserPostCard.User.OrderPlacedNotification != null)
                //    _emailManager.SendOrderStatusChangeMailForUser(postcard.UserPostCard.User, "recipient disapproved");

            }
            Context.SaveChanges();
            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.CompletePostCard(int postcardID)
        {
            IEmailManager _emailManager = new EmailManager();

            var message = "";
            ActionStatus status;
            var postcard = Context.UserPostCards.FirstOrDefault(c => c.ID == postcardID);
            if (postcard != null)
            {
                postcard.IsCompleted = true;
                postcard.CompletedOn = DateTime.UtcNow;
                message = "Post card completed successfully";
                status = ActionStatus.Successfull;
                Context.SaveChanges();
                if (postcard.User.OrderStatusNotification == true && postcard.User.OrderStatusNotification != null)
                {

                    _emailManager.SendOrderStatusChangeMailForUser(postcard.User, "completed");

                }

            }
            else
            {
                message = "No record found."; status = ActionStatus.Error;
            }
            return new ActionOutput() { Message = message, Status = status };
        }

        ActionOutput IEditorManager.CompleteRecipientPostCard(int ReceiptentID)
        {
            IEmailManager _emailManager = new EmailManager();
            var message = "";
            ActionStatus status;
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == ReceiptentID);
            if (postcard != null)
            {
                postcard.IsCompleted = true;
                postcard.IsApproved = true;
                postcard.IsRejected = false;
                postcard.IsError = false;
                postcard.ApprovedOn = DateTime.UtcNow;
                postcard.CompletedOn = DateTime.UtcNow;
                postcard.CardStatus = (int)CardStatusTypes.Completed;
                message = "Post card completed successfully";
                status = ActionStatus.Successfull;
                Context.SaveChanges();
                if (postcard.UserPostCard.User.OrderStatusNotification == true && postcard.UserPostCard.User.OrderStatusNotification != null)
                {
                    _emailManager.SendCardWasMailedEmail(postcard.UserPostCard.User);
                    _emailManager.SendOrderStatusChangeMailForUser(postcard.UserPostCard.User, "completed");
                }

                postcard.UserHistories.Add(new UserHistory() { UserFK = postcard.UserPostCard.UserID, Type = "Order", Status = "Order Completed", TokenChange = "", AddedOn = DateTime.UtcNow, TokenAvailable = postcard.UserPostCard.User.CardsCount });
            }
            else
            {
                message = "No record found."; status = ActionStatus.Error;
            }
            return new ActionOutput() { Message = message, Status = status };
        }

        ActionOutput IEditorManager.DispproveReceiptent(int ReceiptentID)
        {
            var message = "";
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == ReceiptentID);
            if (postcard.IsApproved == true)
            {
                postcard.CardStatus = PreviousCardStatus(ref postcard);
                postcard.IsApproved = false;
                postcard.IsRejected = false;
                postcard.IsError = false;

                message = "Post card recipient disapproved successfully";
                Context.SaveChanges();
            }
            else
            {
                message = "Post card recipients is already in disapproved state";
            }

            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.SentToError(int ReceiptentID)
        {
            var message = "";
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == ReceiptentID);
            if (postcard.IsError == true)
            {
                postcard.CardStatus = PreviousCardStatus(ref postcard);
                postcard.IsError = false;
                postcard.IsRejected = false;

                message = "remove from errors successfully";

                if (postcard.IsRejected == false || postcard.IsRejected == null)
                    postcard.UserPostCard.User.CardsCount -= 1;
            }
            else
            {
                postcard.IsError = true;
                postcard.IsRejected = false;
                postcard.CardStatus = (int)CardStatusTypes.Error;
                message = "Sent to errors successfully";

                if (postcard.IsRejected == false || postcard.IsRejected == null)
                    postcard.UserPostCard.User.CardsCount += 1;
            }
            Context.SaveChanges();
            return new ActionOutput() { Message = message, Status = ActionStatus.Successfull };
        }

        ActionOutput IEditorManager.GetPostCardBackSideJsonResult(int ReceiptentID)
        {
            List<string> result = new List<string>();
            var message = "";
            var postcard = Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == ReceiptentID);
            if (postcard != null)
            {
                if (postcard.UserPostCard.CardBackJsonWithIFrame != null)
                    result.Add(postcard.UserPostCard.CardBackJsonWithIFrame);
                else
                    result.Add(postcard.UserPostCard.CardBackJson);
                message = "remove from errors successfully";
            }
            else
            {
                postcard.IsError = true;
                message = "Sent to errors successfully";
            }
            return new ActionOutput() { Message = message, Results = result, Status = ActionStatus.Successfull };
        }



        #region Helping Methods

        public int PreviousCardStatus(ref UserPostCardRecipient model)
        {
            int status = (int)CardStatusTypes.InProgress;

            if (model.IsApproved == true)
            {
                status = (int)CardStatusTypes.InProgress;
            }
            if (model.IsRejected == true)
            {
                status = (int)CardStatusTypes.InProgress;
            }
            if (model.IsError == true)
            {
                status = (int)CardStatusTypes.InProgress;
            }
            if (model.IsCompleted == true && model.IsRejected == true)
            {
                status = (int)CardStatusTypes.Completed;
            }
            if (model.IsCompleted == true && model.IsError == true)
            {
                status = (int)CardStatusTypes.Completed;
            }
            if (model.IsApproved == true && model.IsError == true && model.IsCompleted == false)
            {
                status = (int)CardStatusTypes.Approved;
            }

            return status;
        }

        #endregion
    }
}
