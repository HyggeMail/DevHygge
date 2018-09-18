//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HyggeMail.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MembershipPlan
    {
        public MembershipPlan()
        {
            this.Users = new HashSet<User>();
            this.UserTransactions = new HashSet<UserTransaction>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public int CardsAllocated { get; set; }
        public Nullable<bool> Isactive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserTransaction> UserTransactions { get; set; }
    }
}