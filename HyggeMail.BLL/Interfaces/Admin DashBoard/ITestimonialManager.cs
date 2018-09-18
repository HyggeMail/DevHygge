using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HyggeMail.BLL.Interfaces
{
    public interface ITestimonialManager
    {
        /// <summary>
        /// This will be used to get all page list.
        /// </summary>
        /// <returns></returns>
        PagingResult<TestimonialModel> GetTestimonialPageList(PagingModel model, int userID, UserTypes userType);

        /// <summary>
        /// This will be used to add testimonial
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        ActionOutput AddUpdateTestimonial(AddTestimonialModel pageContent);

        /// <summary>
        /// Will be used to get selected Contact by Id
        /// </summary>
        /// <param name="ContactID"></param>
        /// <returns></returns>
        AddTestimonialModel GetTestimonialById(int Id);
        ActionOutput DeleteTestimonial(int Id);

        ActionOutput<TestimonialModel> GetAllTestimonials();
    }
}
