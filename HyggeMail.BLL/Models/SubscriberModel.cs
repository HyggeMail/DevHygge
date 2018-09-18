using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public partial class SubscriberModel
    {
        public int ID { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-??]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Invalid Email")]
        public string EmailID { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> ActivatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public SubscriberModel() { }
        public SubscriberModel(Subscriber model)
        {
            this.ID = model.ID;
            this.EmailID = model.EmailID;
            this.AddedOn = model.AddedOn;
            this.IsDeleted = model.IsDeleted;
            this.DeletedOn = model.DeletedOn;
        }
    }
}
