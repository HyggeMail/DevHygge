using HyggeMail.BLL.Models;
using HyggeMail.BLL.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HyggeMail.BLL.Interfaces
{
    public interface IBlogManager
    {
        PagingResult<BlogModel> GetBlogPageList(PagingModel model);
        ActionOutput AddUpdateBlog(AddBlogModel pageContent);
        AddBlogModel GetBlogById(int Id);
        ActionOutput DeleteBlog(int Id);
        ActionOutput<BlogModel> GetAllBlogs();
        ActionOutput UpdateBlogFeaturedArticleStatus(int Id);
        bool IsFeaturedArticleExist();
        BlogModel GetFeaturedArticleBlog();
        BlogModel GetBlogByEncodeId(string encodeId);
    }
}
