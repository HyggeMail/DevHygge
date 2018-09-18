using HyggeMail.BLL.Interfaces.Admin_DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Configuration;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using AutoMapper;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using HyggeMail.BLL.Models.Admin;
using System.Data;

namespace HyggeMail.BLL.Managers.Admin_DashBoard
{
    public class AddressBookManager : BaseManager, IAddressBookManager
    {
        //private string[] addressBookColumnNames = new string[] { "Category", "Sub- Category", "Author", "Keyword Famous For (Book, Song, Award, Distinct)",
        //    "Agency","Address Line 1", "City", "State", "Zip", "Country", "Address Verified (Yes / no / Maybe)","ImageLink" };

        ActionOutput<DataTable> IAddressBookManager.SaveAddressesImportExcel(DataSet ds, int userId)
        {
            DataTable dt = new DataTable("Grid");

            foreach (var item in ds.Tables[0].Columns)
            {
                if (!item.ToString().Contains("Column"))
                {
                    dt.Columns.Add(new DataColumn(item.ToString()));
                }
            }

            try
            {
                //var checkInvalidColumnName = ContainColumnThenGetData(ref ds);

                //if (!string.IsNullOrEmpty(checkInvalidColumnName))
                //    return new ActionOutput<DataTable>() { Status = ActionStatus.Error, Message = "Error Invalid column name occured " + checkInvalidColumnName + "." };

                List<UserAddressBook> obj = new List<UserAddressBook>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var name = ds.Tables[0].Rows[i][2].ToString();
                    var address = ds.Tables[0].Rows[i][5].ToString();

                    var existngUser = Context.UserAddressBooks.Where(z => z.Name.Trim().ToLower() == name.Trim().ToLower() && z.Address == address && z.UserIDFK == userId && z.IsDeleted != true && z.IsPermanent == true).FirstOrDefault();
                    if (existngUser != null)
                    {
                        continue;
                    }

                    var model = new UserAddressBook();

                    model.UserIDFK = userId;
                    model.Category = ds.Tables[0].Rows[i][0].ToString();
                    model.SubCategory = ds.Tables[0].Rows[i][1].ToString();
                    model.Name = ds.Tables[0].Rows[i][2].ToString();
                    model.KeywordFamousFor = ds.Tables[0].Rows[i][3].ToString();
                    model.Agency = ds.Tables[0].Rows[i][4].ToString();
                    model.Address = ds.Tables[0].Rows[i][5].ToString();
                    model.City = ds.Tables[0].Rows[i][6].ToString();
                    model.State = ds.Tables[0].Rows[i][7].ToString();
                    model.Zip = ds.Tables[0].Rows[i][8].ToString();
                    model.Country = ds.Tables[0].Rows[i][9].ToString();
                    model.ImageLink = Convert.ToString(ds.Tables[0].Rows[i][11]);
                    var addressVerified = ds.Tables[0].Rows[i][10].ToString();

                    if (addressVerified.Trim().ToLower() == "Yes".Trim().ToLower())
                        model.AddressVerified = true;
                    else
                        model.AddressVerified = false;

                    model.IsActive = true;
                    model.IsDeleted = false;
                    model.IsPermanent = true;
                    model.ActivatedOn = DateTime.UtcNow;
                    model.AddedOn = DateTime.UtcNow;
                    model.AddedByAdmin = true;

                    if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Address) && !string.IsNullOrEmpty(model.Country) &&
                        !string.IsNullOrEmpty(model.State) && !string.IsNullOrEmpty(model.City))
                        obj.Add(model);
                    else
                        dt.Rows.Add(model.Category, model.SubCategory, model.Name, model.KeywordFamousFor, model.Agency, model.Address,
                            model.City, model.State, model.Zip, model.Country, model.AddressVerified);
                }

                if (obj.Count > 0)
                {
                    Context.UserAddressBooks.AddRange(obj);
                    SaveChanges();
                }

                return new ActionOutput<DataTable>() { Status = ActionStatus.Successfull, Object = dt };
            }
            catch (Exception e)
            {
                return new ActionOutput<DataTable>() { Status = ActionStatus.Error, Message = e.Message };
            }
        }

        PagingResult<RecipientListingModel> IAddressBookManager.GetAddessBookPagedList(PagingModel model, int userID)
        {
            var result = new PagingResult<RecipientListingModel>();
            var query = Context.UserAddressBooks.OrderBy(model.SortBy + " " + model.SortOrder);
            if (userID > 0)
                query = query.Where(c => c.UserIDFK == userID && c.IsDeleted == false);
            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.Name.Contains(model.Search) || z.Address.Contains(model.Search));

            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<UserAddressBook>, List<RecipientListingModel>>(list.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Address List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput<AddUpdateRecipientModel> IAddressBookManager.GetBookAddressByID(int userID, int addressId)
        {
            var address = Context.UserAddressBooks.Where(z => z.UserIDFK == userID && z.ID == addressId && z.IsDeleted == false).FirstOrDefault();
            if (address != null)
            {
                var model = new AddUpdateRecipientModel();
                model = Mapper.Map<UserAddressBook, AddUpdateRecipientModel>(address, model);
                return new ActionOutput<AddUpdateRecipientModel>() { Message = "Address Details", Status = ActionStatus.Successfull, Object = model };
            }
            else
            {
                return new ActionOutput<AddUpdateRecipientModel>()
                {
                    Status = ActionStatus.Error,
                    Message = "Address not exist."
                };
            }
        }

        ActionOutput<RecipientDetails> IAddressBookManager.AddUpdateAddressBook(AddUpdateRecipientModel model)
        {
            var message = "";
            RecipientDetails addressDetails = new RecipientDetails();
            if (model.ID > 0)
            {
                var existngUser = Context.UserAddressBooks.Where(z => z.Name.Trim().ToLower() == model.Name.Trim().ToLower() && z.Address == model.Address && z.UserIDFK == model.UserID && z.ID != model.ID
                    && z.IsDeleted == false).FirstOrDefault();
                if (existngUser != null)
                {
                    return new ActionOutput<RecipientDetails>
                    {
                        Status = ActionStatus.Error,
                        Message = "This author with same name and address is already added into your address book."
                    };
                }

                var address = Context.UserAddressBooks.Where(z => z.ID == model.ID && z.IsDeleted != true).FirstOrDefault();
                address = Mapper.Map<AddUpdateRecipientModel, UserAddressBook>(model, address);
                address.AddressVerified = model.AddressVerified;
                Context.SaveChanges();
                message = "Address details updated successfully.";
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
                    addressDetails = Mapper.Map<UserAddressBook, RecipientDetails>(book, addressDetails);
                    //  recipientDetails.State = Context.states.FirstOrDefault(x => x.id == book.State).name;
                    //                    recipientDetails.City = Context.cities.FirstOrDefault(x => x.cityID == book.City).cityName;
                    message = "Address details added successfully.";
                }
            }
            return new ActionOutput<RecipientDetails>
            {
                Status = ActionStatus.Successfull,
                Message = message,
                Object = addressDetails
            };

        }

        ActionOutput IAddressBookManager.DeleteBookAddressById(int userId, int addressID)
        {
            var address = Context.UserAddressBooks.Where(z => z.UserIDFK == userId && z.ID == addressID).FirstOrDefault();
            if (address == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Address not exist."
                };
            }
            else
            {
                address.IsDeleted = true;
                address.DeletedOn = DateTime.Now;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Address deleted successfully."
                };
            }
        }

        #region Helping Methods

        //private string ContainColumnThenGetData(ref DataSet ds)
        //{
        //    DataColumnCollection columns = ds.Tables[0].Columns;
        //    for (int i = 0; i < addressBookColumnNames.Length; i++)
        //    {
        //        var columnName = addressBookColumnNames[i];
        //        if (columns.Contains(columnName))
        //            continue;
        //        else
        //            return addressBookColumnNames[i];
        //    }

        //    return null;
        //}

        #endregion
    }
}
