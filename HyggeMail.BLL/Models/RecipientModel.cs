using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    public class AddUpdateRecipientModel : DbContext
    {
        public int ID { get; set; }

        public int? UserID { get; set; }
        [Required, StringLength(25, ErrorMessage = "The Name cannot be more than 25 characters")]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        public string Zip { get; set; }
        public bool IsPermanent { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }

        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string KeywordFamousFor { get; set; }
        public string Agency { get; set; }
        public bool AddressVerified { get; set; }

        public AddUpdateRecipientModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
            this.IsPermanent = true;
            this.CountryList = Context.countries.ToList().Select(c =>
                new SelectListItem { Text = c.name, Value = Convert.ToString(c.id) }).ToList();
        }

    }

    public class RecipientListingModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string KeywordFamousFor { get; set; }
        public string Agency { get; set; }
        public Nullable<bool> AddressVerified { get; set; }
        public DateTime AddedOn { get; set; }
        public RecipientListingModel()
        {

        }
    }

    public class RecipientGroupModel
    {
        public string Alphabet { get; set; }
        public int count { get; set; }
        public List<RecipientDetails> List { get; set; }
    }
    public class RecipientDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public bool IsPermanent { get; set; }
        public string MarkChecked { get; set; }
        public bool AddedByAdmin { get; set; }
        public string ImageLink { get; set; }
        public string Agency { get; set; }
        public string KeywordFamousFor { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
    }

}
