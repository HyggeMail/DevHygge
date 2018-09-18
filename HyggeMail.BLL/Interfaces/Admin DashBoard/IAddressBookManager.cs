using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Interfaces.Admin_DashBoard
{
    public interface IAddressBookManager
    {
        ActionOutput<DataTable> SaveAddressesImportExcel(DataSet ds, int userId);
        PagingResult<RecipientListingModel> GetAddessBookPagedList(PagingModel model, int userID);
        ActionOutput<RecipientDetails> AddUpdateAddressBook(AddUpdateRecipientModel model);
        ActionOutput<AddUpdateRecipientModel> GetBookAddressByID(int userID, int addressId);
        ActionOutput DeleteBookAddressById(int userId, int addressID);
    }
}
