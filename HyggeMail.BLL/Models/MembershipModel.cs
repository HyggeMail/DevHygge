using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HyggeMail.BLL.Models
{
    public class MembershipModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        [Required]
        public decimal Rate { get; set; }
        public decimal? Discount { get; set; }
        [Required]
        public int CardsAllocated { get; set; }
        public bool Isactive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DeletedOn { get; set; }

        public MembershipModel() { }

        public MembershipModel(MembershipPlan obj)
        {
            this.CardsAllocated = obj.CardsAllocated;
            this.CreatedOn = Convert.ToDateTime(obj.CreatedOn);
            this.DeletedOn = Convert.ToDateTime(obj.DeletedOn);
            this.Description = obj.Description;
            this.Discount = obj.Discount;
            this.ID = obj.ID;
            this.Isactive = obj.Isactive ?? false;
            this.IsDeleted = obj.IsDeleted ?? false;
            this.Name = obj.Name;
            this.Rate = obj.Rate;
        }
    }
}
