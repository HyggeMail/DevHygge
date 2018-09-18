using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public partial class UserHistoryModel
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
    }
}
