using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Interfaces
{
    public interface IMembershipManager
    {
        PagingResult<MembershipModel> GetPlansPagedList(PagingModel model);

        ActionOutput CreatePlan(MembershipModel planModel);

        ActionOutput UpdatePlanDetails(MembershipModel planModel);

        MembershipModel GetPlanDetailsByID(int planID);

        ActionOutput DeletePlane(int planID);
    }
}
