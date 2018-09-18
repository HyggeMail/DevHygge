using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public class UserSession
    {
        public bool sessionStatus { get; set; }
        public Sessionstatus sessionStatusType { get; set; }
    }
}
