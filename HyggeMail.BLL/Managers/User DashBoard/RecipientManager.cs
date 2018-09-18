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

namespace HyggeMail.BLL.Managers
{
    public class RecipientManager : BaseManager, IRecipientManager
    {
        PagingResult<RecipientListingModel> IRecipientManager.GetRecipientPagedList(PagingModel model, int userID)
        {
            var result = new PagingResult<RecipientListingModel>();
            var query = Context.UserAddressBooks.OrderBy(model.SortBy + " " + model.SortOrder);
            if (userID > 0)
                query = query.Where(c => c.UserIDFK == userID);
            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.Name.Contains(model.Search) || z.Address.Contains(model.Search));

            result.List = Mapper.Map<List<UserAddressBook>, List<RecipientListingModel>>(query.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "User List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput<RecipientDetails> IRecipientManager.AddUpdateRecipient(AddUpdateRecipientModel model)
        {
            var message = "";
            RecipientDetails recipientDetails = new RecipientDetails();
            if (model.ID > 0)
            {
                var recipient = Context.UserAddressBooks.Where(z => z.ID == model.ID && z.IsDeleted != true).FirstOrDefault();
                recipient = Mapper.Map<AddUpdateRecipientModel, UserAddressBook>(model, recipient);
                message = "Recipient details updated successfully.";
                Context.SaveChanges();
                recipientDetails = Mapper.Map<UserAddressBook, RecipientDetails>(recipient, recipientDetails);

            }
            else
            {
                var existngUser = Context.UserAddressBooks.Where(z => z.Name.Trim().ToLower() == model.Name.Trim().ToLower() && z.Address == model.Address && z.UserIDFK == model.UserID && z.IsDeleted != true && z.IsPermanent == true).FirstOrDefault();
                if (existngUser != null)
                {
                    return new ActionOutput<RecipientDetails>
                    {
                        Status = ActionStatus.Error,
                        Message = "This recipient with same name and address is already added into your address book."
                    };
                }
                else
                {
                    UserAddressBook book = Mapper.Map<AddUpdateRecipientModel, UserAddressBook>(model);
                    book.IsActive = true;
                    book.ActivatedOn = DateTime.UtcNow;
                    book.AddedOn = DateTime.UtcNow;
                    book.IsDeleted = false;
                    Context.UserAddressBooks.Add(book);
                    Context.SaveChanges();
                    recipientDetails = Mapper.Map<UserAddressBook, RecipientDetails>(book, recipientDetails);
                    //  recipientDetails.State = Context.states.FirstOrDefault(x => x.id == book.State).name;
                    //                    recipientDetails.City = Context.cities.FirstOrDefault(x => x.cityID == book.City).cityName;
                    message = "Recipient details added successfully.";
                }
            }
            return new ActionOutput<RecipientDetails>
            {
                Status = ActionStatus.Successfull,
                Message = message,
                Object = recipientDetails
            };

        }

        ActionOutput<RecipientGroupModel> IRecipientManager.GetPostCardRecipientByAlphabetic(int cid, int userID = 0, string keyword = "", bool IsAddedByadmin = false)
        {
            var userRecipientIds = Context.UserPostCardRecipients.Where(x => x.UserPostCardID == cid).Select(x => x.FKUserAddressBookId).ToList();

            var list = new List<RecipientGroupModel>();
            IQueryable<UserAddressBook> dataList;
            if (!IsAddedByadmin)
                dataList = Context.UserAddressBooks.Where(c => c.IsActive == true && (c.IsDeleted != true || c.IsDeleted == null) && c.UserIDFK == userID && c.IsPermanent == true && (c.AddedByAdmin == false || c.AddedByAdmin == null));
            else
                dataList = Context.UserAddressBooks.Where(c => c.IsActive == true && (c.IsDeleted != true || c.IsDeleted == null) && c.AddedByAdmin == true);

            if (!string.IsNullOrEmpty(keyword))
                // dataList = dataList.Where(c => c.Name.Contains(keyword) || c.Address.Contains(keyword));
                dataList = dataList.Where(c => c.Name.ToLower().Contains(keyword.ToLower()));


            if (!IsAddedByadmin)
            {
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string currentAlphabet = c.ToString();
                    var item = new RecipientGroupModel();
                    item.Alphabet = currentAlphabet;
                    item.List = new List<RecipientDetails>();
                    var currentList = dataList.Where(x => x.Name.ToLower().StartsWith(currentAlphabet.ToLower()));
                    item.count = currentList.Count();
                    item.List = Mapper.Map<List<UserAddressBook>, List<RecipientDetails>>(currentList.ToList(), item.List);

                    //To show the first five records in case of famous people addresses
                    //if (string.IsNullOrEmpty(keyword) && IsAddedByadmin)
                    //    item.List = item.List.Take(5).ToList();

                    item.List.Where(x => userRecipientIds.Contains(x.ID)).ToList().ForEach(x => x.MarkChecked = "checked");

                    list.Add(item);
                }
            }
            else
            {
                var item = new RecipientGroupModel();
                item.List = new List<RecipientDetails>();
                item.count = dataList.Count();
                item.List = Mapper.Map<List<UserAddressBook>, List<RecipientDetails>>(dataList.ToList(), item.List);

                item.List.Where(x => userRecipientIds.Contains(x.ID)).ToList().ForEach(x => x.MarkChecked = "checked");

                list.Add(item);
            }
            return new ActionOutput<RecipientGroupModel>() { Message = "Images List", Status = ActionStatus.Successfull, List = list };
        }

        ActionOutput<RecipientGroupModel> IRecipientManager.GetRecipientByAlphabetic(int userID = 0, string keyword = "")
        {
            var list = new List<RecipientGroupModel>();
            var dataList = Context.UserAddressBooks.Where(c => c.IsActive == true && c.IsDeleted != true && c.UserIDFK == userID && c.IsPermanent == true);
            if (keyword != "")
                dataList = dataList.Where(c => c.Name.Contains(keyword) || c.Address.Contains(keyword));

            for (char c = 'A'; c < 'Z'; c++)
            {
                string currentAlphabet = c.ToString();
                var item = new RecipientGroupModel();
                item.Alphabet = currentAlphabet;
                item.List = new List<RecipientDetails>();
                var currentList = dataList.Where(x => x.Name.ToLower().StartsWith(currentAlphabet.ToLower()));
                item.count = currentList.Count();
                item.List = Mapper.Map<List<UserAddressBook>, List<RecipientDetails>>(currentList.ToList(), item.List);
                list.Add(item);
            }

            return new ActionOutput<RecipientGroupModel>() { Message = "Images List", Status = ActionStatus.Successfull, List = list };
        }

        ActionOutput<RecipientDetails> IRecipientManager.GetUserRecipients(int userID = 0, string keyword = "")
        {
            var list = new List<RecipientDetails>();
            var dataList = Context.UserAddressBooks.Where(c => c.IsActive == true && c.IsDeleted != true && c.UserIDFK == userID && c.IsPermanent == true);
            if (keyword != "")
                dataList = dataList.Where(c => c.Name.Contains(keyword) || c.Address.Contains(keyword));

            list = Mapper.Map<List<UserAddressBook>, List<RecipientDetails>>(dataList.ToList(), list);

            return new ActionOutput<RecipientDetails>() { Message = "Recipient List", Status = ActionStatus.Successfull, List = list };
        }



        ActionOutput IRecipientManager.DeleteRecipientByID(int userId, int recipientID)
        {
            var recipient = Context.UserAddressBooks.Where(z => z.UserIDFK == userId && z.ID == recipientID).FirstOrDefault();
            if (recipient == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Recipient not exist."
                };
            }
            else
            {
                recipient.IsDeleted = true;
                recipient.DeletedOn = DateTime.Now;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Recipient deleted successfully."
                };
            }
        }

        ActionOutput<AddUpdateRecipientModel> IRecipientManager.GetRecipientByID(int userID = 0, int recipientID = 0)
        {
            IUserManager _um = new UserManager();
            var recipient = Context.UserAddressBooks.Where(z => z.UserIDFK == userID && z.ID == recipientID && z.IsPermanent == true).FirstOrDefault();
            if (recipient != null)
            {
                var model = new AddUpdateRecipientModel();
                model = Mapper.Map<UserAddressBook, AddUpdateRecipientModel>(recipient, model);
                return new ActionOutput<AddUpdateRecipientModel>() { Message = "Recipient Details", Status = ActionStatus.Successfull, Object = model };
            }
            else
            {
                return new ActionOutput<AddUpdateRecipientModel>()
                {
                    Status = ActionStatus.Error,
                    Message = "Recipient not exist."
                };
            }
        }


        public string GetRejectionReasonByCardId(int id)
        {
            return Context.UserPostCardRecipients.FirstOrDefault(c => c.ID == id).RejectedReason;
        }
    }
}
