using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Configuration;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using AutoMapper;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using HyggeMail.BLL.Models.Admin;

namespace HyggeMail.BLL.Managers
{
    public class BlogManager : BaseManager, IBlogManager
    {

        PagingResult<BlogModel> IBlogManager.GetBlogPageList(PagingModel model)
        {
            var result = new PagingResult<BlogModel>();
            var query = Context.BlogDetails.Where(x => x.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.Title.Contains(model.Search) || z.Name.Contains(model.Search) || z.Description.Contains(model.Search));
            }

            if (model.Checked) 
            {
                query = query.Where(x => x.IsFeaturedArticle == true);
            }

            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new BlogModel(x)).ToList();
            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "Blog List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput IBlogManager.AddUpdateBlog(AddBlogModel model)
        {
            try
            {
                if (model.ID > 0)
                {
                    var blog = Context.BlogDetails.Where(z => z.ID == model.ID && z.IsDeleted != true).FirstOrDefault();
                    if (blog != null)
                    {
                        blog.Title = model.Title;
                        if (model.Image == null)
                            blog.Image = model.ImageName;
                        else
                            blog.Image = Utilities.SaveImage(model.Image, AppDefaults.BlogPath, AppDefaults.BlogThumbPath);
                        blog.Description = model.Description;
                        blog.UpdatedOn = DateTime.UtcNow;
                        Context.SaveChanges();
                        return new ActionOutput
                        {
                            Status = ActionStatus.Successfull,
                            Message = "Sucessfully Updated."
                        };
                    }
                    else
                    {
                        return new ActionOutput
                        {
                            Status = ActionStatus.Error,
                            Message = "No blog found."
                        };
                    }
                }
                else
                {
                    if (model.Image == null)
                        return new ActionOutput
                        {
                            Status = ActionStatus.Error,
                            Message = "Please add the Blog image."
                        };

                    var newBlog = new BlogDetail();

                    newBlog.Title = model.Title;
                    newBlog.Description = model.Description;
                    newBlog.IsActive = model.IsActive;
                    newBlog.AddedOn = DateTime.UtcNow;
                    newBlog.UpdatedOn = DateTime.UtcNow;
                    newBlog.Image = Utilities.SaveImage(model.Image, AppDefaults.BlogPath, AppDefaults.BlogThumbPath);
                    newBlog.IsDeleted = model.IsDeleted;
                    newBlog.IsFeaturedArticle = model.IsFeaturedArticle;
                    Context.BlogDetails.Add(newBlog);
                    Context.SaveChanges();
                    return new ActionOutput
                    {
                        Status = ActionStatus.Successfull,
                        Message = "Sucessfully Added."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Internal Server error."
                };
            }
        }

        ActionOutput<BlogModel> IBlogManager.GetAllBlogs()
        {
            var result = new ActionOutput<BlogModel>();
            var blog = Context.BlogDetails.Where(x => x.IsDeleted != true).OrderByDescending(x => x.AddedOn).AsEnumerable();
            var list = blog.ToList().Select(x => new BlogModel(x)).ToList();
            if (list != null && list.Count > 0)
            {
                return new ActionOutput<BlogModel>
                {
                    Status = ActionStatus.Successfull,
                    Message = "Blog List",
                    List = list
                };
            }
            else
                return null;
        }

        AddBlogModel IBlogManager.GetBlogById(int Id)
        {
            var blog = Context.BlogDetails.FirstOrDefault(x => x.ID == Id);
            if (blog == null)
                return null;
            else
                return new AddBlogModel(blog);
        }

        ActionOutput IBlogManager.DeleteBlog(int Id)
        {
            var blog = Context.BlogDetails.Where(z => z.ID == Id).FirstOrDefault();
            if (blog == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Blog Not Exist."
                };
            }
            else
            {
                Context.BlogDetails.Remove(blog);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Blog Deleted Successfully."
                };
            }
        }

        ActionOutput IBlogManager.UpdateBlogFeaturedArticleStatus(int Id)
        {
            string message = "Blog Not Exist.";
            var blog = Context.BlogDetails.Where(z => z.ID == Id).FirstOrDefault();
            if (blog == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = message
                };
            }
            else
            {
                if (blog.IsFeaturedArticle == true)
                {
                    blog.IsFeaturedArticle = false;
                    message = "Remove featured article on this blog";
                }
                else
                {

                    var checkBlogExist = (this as IBlogManager).IsFeaturedArticleExist();
                    if (checkBlogExist)
                    {
                        return new ActionOutput
                        {
                            Status = ActionStatus.Error,
                            Message = "You already have set Blog as Featured Article. So Please unchecked first then choose other one."
                        };
                    }

                    blog.IsFeaturedArticle = true;
                    message = "Blog successfully set as featured article";
                }
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = message
                };
            }
        }

        bool IBlogManager.IsFeaturedArticleExist()
        {
            var checkBlogExist = Context.BlogDetails.Where(x => x.IsFeaturedArticle == true).Any();

            return checkBlogExist;
        }

        BlogModel IBlogManager.GetFeaturedArticleBlog()
        {
            var blog = Context.BlogDetails.Where(x => x.IsFeaturedArticle == true).FirstOrDefault();
            if (blog == null)
                return null;
            else
                return new BlogModel(blog);
        }

        BlogModel IBlogManager.GetBlogByEncodeId(string encodeId)
        {
            var id = Convert.ToInt32(Utilities.DecodeString(encodeId));
            var blog = Context.BlogDetails.FirstOrDefault(x => x.ID == id);
            if (blog == null)
                return null;
            else
                return new BlogModel(blog);
        }
    }
}
