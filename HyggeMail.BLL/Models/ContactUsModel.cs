using HyggeMail.BLL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using HyggeMail.DAL;

namespace HyggeMail.BLL.Models
{
    public partial class WebContactUsModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Email { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime ResolvedOn { get; set; }
        public DateTime DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsResolved { get; set; }
        [Required(ErrorMessage = "*Required")]
        public ContactRequestType RequestType { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public bool IsSubscriber { get; set; }
        public List<SelectListItem> RequestTypeList { get; set; }
        public WebContactUsModel()
        {
            this.RequestTypeList = new List<SelectListItem>();
            this.RequestTypeList = Utilities.EnumToList(typeof(ContactRequestType));
        }
    }

}
