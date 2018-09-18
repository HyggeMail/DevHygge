using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Interfaces
{
    public interface IFAQManager
    {
        PagingResult<FAQModel> GetFAQPagedList(PagingModel model, int category);

        ActionOutput CreateFAQ(FAQModel FAQModel);

        ActionOutput UpdateFAQDetails(FAQModel FAQModel);

        FAQModel GetFAQDetailsByID(int FAQID);

        ActionOutput DeleteFAQ(int FAQID);
    }
}
