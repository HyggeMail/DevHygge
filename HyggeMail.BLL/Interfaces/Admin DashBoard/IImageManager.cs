using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.DAL;
using System.Web.Mvc;

namespace HyggeMail.BLL.Interfaces
{
    public interface IImageManager
    {
        PagingResult<AdminImageListingModel> GetImageList(PagingModel model);
        PagingResult<AdminImageCategoryListingModel> GetImageCategoryList(PagingModel model);
        ActionOutput AddUpdateImageCategory(AddUpdateAdminImageCategoryModel categoryModel);
        AddUpdateAdminImageCategoryModel GetImageCategoryById(int id);
        ActionOutput DeleteImageCategory(int id);

        ActionOutput AddUpdateAdminImage(AddUpdateAdminImageModel model);
        ActionOutput DeleteAdminImage(int id);
        AddUpdateAdminImageModel GetAdminImageById(int id);
        List<SelectListItem> GetImageCategoryDDLList();
        ActionOutput<ImagesByCategoryViewModel> GetImagesByCategoryWise(int userID = 0, string keyword = "");
        ActionOutput<UserPostCardViewModel> GetLatestUserPostCard();
        ActionOutput<ImagesByCategoryViewModel> GetEmojiFromLocalFolder();
        ActionOutput<UnSplashImagesListModel> GetUnSplashImages(int pageNo = 1, string keyword = "");
    }
}
