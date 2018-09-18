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
using System.Web;
using Newtonsoft.Json;
using HyggeMail.UnSplash;

namespace HyggeMail.BLL.Managers
{
    public class ImageManager : BaseManager, IImageManager
    {
        PagingResult<AdminImageListingModel> IImageManager.GetImageList(PagingModel model)
        {
            var result = new PagingResult<AdminImageListingModel>();
            var query = Context.AdminImages.Where(c => c.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.FileName.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList();
            result.List = Mapper.Map<List<AdminImage>, List<AdminImageListingModel>>(query.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Image List";
            result.TotalCount = query.Count();
            return result;
        }
        PagingResult<AdminImageCategoryListingModel> IImageManager.GetImageCategoryList(PagingModel model)
        {
            var result = new PagingResult<AdminImageCategoryListingModel>();
            var query = Context.ImageCategories.Where(c => c.IsDeleted != true).OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.CategoryName.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList();
            result.List = Mapper.Map<List<ImageCategory>, List<AdminImageCategoryListingModel>>(query.ToList(), result.List);
            result.Status = ActionStatus.Successfull;
            result.Message = "Image Category List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput IImageManager.AddUpdateImageCategory(AddUpdateAdminImageCategoryModel categoryModel)
        {
            var existingImageCategory = Context.ImageCategories.FirstOrDefault(z => z.ID == categoryModel.ID);
            if (existingImageCategory == null)
            {
                var newRecord = new ImageCategory();
                var checkNameExist = Context.ImageCategories.FirstOrDefault(z => z.CategoryName.ToLower() == categoryModel.CategoryName.ToLower() && z.IsDeleted != true && z.ID != categoryModel.ID);
                if (checkNameExist != null)
                {
                    return new ActionOutput
                    {
                        Status = ActionStatus.Error,
                        Message = "Image category name already exist."
                    };
                }
                newRecord = Mapper.Map<AddUpdateAdminImageCategoryModel, ImageCategory>(categoryModel);
                newRecord.ActivatedOn = DateTime.UtcNow;
                newRecord.AddedOn = DateTime.UtcNow;
                newRecord.IsDeleted = false;
                newRecord.IsActive = true;
                Context.ImageCategories.Add(newRecord);

                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Category Added Sucessfully."
                };
            }
            else
            {
                var checkNameExist = Context.ImageCategories.FirstOrDefault(z => z.CategoryName.ToLower() == categoryModel.CategoryName.ToLower() && z.IsDeleted != true && z.ID != categoryModel.ID);
                if (checkNameExist != null)
                {
                    return new ActionOutput
                    {
                        Status = ActionStatus.Error,
                        Message = "Image category name already exist."
                    };
                }
                existingImageCategory = Mapper.Map<AddUpdateAdminImageCategoryModel, ImageCategory>(categoryModel, existingImageCategory);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Category Updated Sucessfully."
                };
            }
        }

        AddUpdateAdminImageCategoryModel IImageManager.GetImageCategoryById(int id)
        {
            var model = new AddUpdateAdminImageCategoryModel();
            var existingCategory = Context.ImageCategories.FirstOrDefault(z => z.ID == id);
            if (existingCategory != null)
                return model = Mapper.Map<ImageCategory, AddUpdateAdminImageCategoryModel>(existingCategory);
            else
                return null;
        }

        ActionOutput IImageManager.DeleteImageCategory(int id)
        {
            var employee = Context.ImageCategories.Where(z => z.ID == id).FirstOrDefault();
            if (employee == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Image Category doesn't exit in system"
                };
            }
            else
            {
                employee.IsDeleted = true;
                employee.DeletedOn = DateTime.Now;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Category Deleted Successfully."
                };
            }
        }


        ActionOutput IImageManager.AddUpdateAdminImage(AddUpdateAdminImageModel model)
        {
            var existingImage = Context.AdminImages.FirstOrDefault(z => z.ID == model.ID);
            if (existingImage == null)
            {
                var newRecord = new AdminImage();
                newRecord = Mapper.Map<AddUpdateAdminImageModel, AdminImage>(model);
                foreach (var item in model.Image)
                {
                    //  var fileUName = Utilities.SavePostedFile(AttacmentsPath.AdminImages + "/", item);
                    var saveFileWithThumb = Utilities.SaveImage(item, AttacmentsPath.AdminImages, AttacmentsPath.AdminImagesThumb);
                    newRecord.FilePath = AttacmentsPath.AdminImages + saveFileWithThumb;
                    newRecord.FileName = Path.GetFileNameWithoutExtension(saveFileWithThumb);
                    newRecord.Description = model.Description;
                    newRecord.ActivatedOn = DateTime.UtcNow;
                    newRecord.AddedOn = DateTime.UtcNow;
                    newRecord.IsDeleted = false;
                    newRecord.IsActive = true;
                    Context.AdminImages.Add(newRecord);
                }
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Added Sucessfully."
                };
            }
            else
            {
                // existingImage = Mapper.Map<AddUpdateAdminImageModel, AdminImage>(model);
                existingImage.CategoryID = model.CategoryID;
                existingImage.Description = model.Description;
                if (model.Image != null)
                {
                    foreach (var item in model.Image)
                    {
                        //  var fileUName = Utilities.SavePostedFile(AttacmentsPath.AdminImages + "/", item);
                        var saveFileWithThumb = Utilities.SaveImage(item, AttacmentsPath.AdminImages, AttacmentsPath.AdminImagesThumb);
                        existingImage.FileName = Path.GetFileNameWithoutExtension(saveFileWithThumb);
                        existingImage.FilePath = AttacmentsPath.AdminImages + saveFileWithThumb;
                    }
                }
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Updated Sucessfully."
                };
            }
        }

        AddUpdateAdminImageModel IImageManager.GetAdminImageById(int id)
        {
            var model = new AddUpdateAdminImageModel();
            var existing = Context.AdminImages.FirstOrDefault(z => z.ID == id);
            if (existing != null)
                return model = Mapper.Map<AdminImage, AddUpdateAdminImageModel>(existing);
            else
                return null;
        }

        ActionOutput IImageManager.DeleteAdminImage(int id)
        {
            var employee = Context.AdminImages.Where(z => z.ID == id).FirstOrDefault();
            if (employee == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Image doesn't exit in system"
                };
            }
            else
            {
                employee.IsDeleted = true;
                employee.DeletedOn = DateTime.Now;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Image Deleted Successfully."
                };
            }
        }

        List<SelectListItem> IImageManager.GetImageCategoryDDLList()
        {
            var list = new List<SelectListItem>();
            var result = Context.ImageCategories.Where(c => c.IsDeleted != true && c.IsActive == true).OrderBy(m => m.CategoryName).AsEnumerable();
            if (result != null)
            {
                foreach (var i in result)
                {
                    list.Add(new SelectListItem { Text = i.CategoryName, Value = i.ID.ToString() });
                }
            }
            return list;
        }

        ActionOutput<ImagesByCategoryViewModel> IImageManager.GetImagesByCategoryWise(int userID = 0, string keyword = "")
        {
            var dataList = Context.ImageCategories.Where(c => c.IsActive == true && c.IsDeleted != true);
            var model = new List<ImagesByCategoryViewModel>();

            //Bind Recently added images which were added by the admin of last one hour.
            //var lastOneHour = DateTime.UtcNow.AddHours(-1);
            //var recentlyAdded = Context.AdminImages.Where(x => x.AddedOn >= lastOneHour && x.ByAdmin == true && x.IsDeleted != true);
            //var recentList = new ImagesByCategoryViewModel();
            //recentList.CatName = "Recently Added";
            //recentList.ImagesCount = recentlyAdded.Count();
            //recentList.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(recentlyAdded.ToList(), recentList.Images);
            //model.Add(recentList);

            //Bind most used images.
            var mostUsed = Context.UserPostCardImages.Where(x => x.AdminImage.ByAdmin == true && x.AdminImage.IsDeleted != true).OrderBy(x => x.AdminImage.AddedOn).GroupBy(x => x.AdminImage).Take(20).ToList().Select(x => x.Key);
            var mostUsedList = new ImagesByCategoryViewModel();
            mostUsedList.CatName = "Mostly Used";
            mostUsedList.ImagesCount = mostUsed.Count();
            mostUsedList.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(mostUsed.ToList(), mostUsedList.Images);
            model.Add(mostUsedList);
            if (userID > 0)
            {
                //Bind user added images which were added by the admin of last one hour.
                var userImages = Context.AdminImages.Where(x => x.AddedBy == userID && x.IsDeleted != true);
                var userAddedImagesList = new ImagesByCategoryViewModel();
                userAddedImagesList.CatName = "Personal Directory";
                userAddedImagesList.HasLoggedInUser = true;
                userAddedImagesList.ImagesCount = userImages.Count();
                userAddedImagesList.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(userImages.ToList(), userAddedImagesList.Images);
                model.Add(userAddedImagesList);

                //Bind Recently used images by User.
                var recentlyUsed = Context.UserPostCardImages.Where(x => x.UsedBy == userID && x.AdminImage.IsDeleted != true).GroupBy(x => x.AdminImage).ToList().Select(x => x.Key);
                var recentlyUsedList = new ImagesByCategoryViewModel();
                recentlyUsedList.CatName = "Recently Used";
                recentlyUsedList.IsRecent = true;
                recentlyUsedList.ImagesCount = mostUsed.Count();
                recentlyUsedList.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(recentlyUsed.ToList(), recentlyUsedList.Images);
                model.Add(recentlyUsedList);
            }



            foreach (var item in dataList)
            {
                var record = new ImagesByCategoryViewModel(item);
                record.Images = new List<ImagesViewModel>();
                if (userID > 0)
                {
                    var imageList = item.AdminImages.Where(x => (x.ByAdmin == true || x.AddedBy == userID) && x.IsDeleted != true);
                    record.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(imageList.ToList(), record.Images);
                    record.ImagesCount = record.Images.Count;
                }
                else
                {
                    var imageList = item.AdminImages.Where(x => x.ByAdmin == true && x.IsDeleted != true);
                    record.Images = Mapper.Map<List<AdminImage>, List<ImagesViewModel>>(imageList.ToList(), record.Images);
                    record.ImagesCount = record.Images.Count;
                }
                model.Add(record);
                if (keyword != "")
                    model = model.Where(c => c.CatName.ToLower().Contains(keyword.ToLower())).ToList();

            }
            return new ActionOutput<ImagesByCategoryViewModel>() { Message = "Images List", Status = ActionStatus.Successfull, List = model };
        }

        ActionOutput<UnSplashImagesListModel> IImageManager.GetUnSplashImages(int pageNo = 1, string keyword = "")
        {
            var _unSplashServices = new UnSplashServices();

            var response = _unSplashServices.GetPics(pageNo, keyword);

            if (!string.IsNullOrEmpty(response))
            {
                var data = JsonConvert.DeserializeObject<UnSplashImagesListModel>(response);
                data.PageNumber = pageNo;
                return new ActionOutput<UnSplashImagesListModel>() { Message = "Images List", Status = ActionStatus.Successfull, Object = data };
            }

            return new ActionOutput<UnSplashImagesListModel>() { Message = "UnSplash Server Error", Status = ActionStatus.Error, Object = null };
        }

        ActionOutput<UserPostCardViewModel> IImageManager.GetLatestUserPostCard()
        {
            var result = new ActionOutput<UserPostCardViewModel>();
            var images = Context.UserPostCards.OrderByDescending(x => x.ID).Take(8);
            result.List = Mapper.Map<List<UserPostCard>, List<UserPostCardViewModel>>(images.ToList(), result.List);
            return result;
        }

        public ActionOutput<ImagesByCategoryViewModel> GetEmojiFromLocalFolder()
        {
            var model = new List<ImagesByCategoryViewModel>();
            var folderPath = HttpContext.Current.Server.MapPath("/Content/Images/Emoji");
            string[] FolderDirectory = Directory.GetDirectories(folderPath);
            if (FolderDirectory != null && FolderDirectory.Count() > 0)
            {
                foreach (string subdir in FolderDirectory)
                {
                    var recentList = new ImagesByCategoryViewModel();
                    string foldername = new DirectoryInfo(subdir).Name;
                    var imageList = new List<ImagesViewModel>();
                    int imageID = 1;
                    foreach (string img in Directory.GetFiles(subdir))
                    {
                        var imageName = Path.GetFileName(img);
                        var imagePath = "/Content/Images/Emoji/" + foldername + "/" + imageName;
                        imageList.Add(new ImagesViewModel
                        {
                            FileName = imageName,
                            Description = "Hygge Mail emoticons",
                            FilePath = imagePath,
                            ImageID = imageID
                        });
                        imageID++;
                    }
                    recentList.CatName = foldername;
                    recentList.Images = imageList;
                    recentList.ImagesCount = Directory.GetFiles(subdir).Length;
                    model.Add(recentList);
                }
            }
            return new ActionOutput<ImagesByCategoryViewModel>
            {
                List = model,
                Message = "Emoji List",
                Status = ActionStatus.Successfull
            };
        }

    }

}
