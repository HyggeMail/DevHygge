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
    
    public partial class UserHistory
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string TokenChange { get; set; }
        public Nullable<int> TokenAvailable { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<int> UserFK { get; set; }
        public Nullable<int> OrderIDFK { get; set; }
        public Nullable<int> TransactionIDFK { get; set; }
    
        public virtual UserPostCardRecipient UserPostCardRecipient { get; set; }
        public virtual User User { get; set; }
        public virtual UserTransaction UserTransaction { get; set; }
    }
}
