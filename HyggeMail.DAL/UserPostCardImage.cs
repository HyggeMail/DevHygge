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
    
    public partial class UserPostCardImage
    {
        public int ID { get; set; }
        public Nullable<int> AdminImageID { get; set; }
        public Nullable<int> UsedBy { get; set; }
        public Nullable<System.DateTime> UsedOn { get; set; }
        public Nullable<int> UserPostCardID { get; set; }
    
        public virtual AdminImage AdminImage { get; set; }
        public virtual UserPostCard UserPostCard { get; set; }
        public virtual User User { get; set; }
    }
}