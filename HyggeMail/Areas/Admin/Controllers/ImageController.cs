using HyggeMail.Attributes;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HyggeMail.Areas.Admin.Controllers
{
    public class ImageController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IImageManager _imageManager;
        #endregion

        public ImageController(IErrorLogManager errorLogManager, IImageManager imageManager)
            : base(errorLogManager)
        {
            _imageManager = imageManager;
        }
        public ActionResult AddImageCategory(int? id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Images;
            var model = new AddUpdateAdminImageCategoryModel();
            if (id != null)
            {
                model = _imageManager.GetImageCategoryById(Convert.ToInt32(id));
            }
            return View(model);
        }

        public ActionResult ManageImageCategory()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Images;
            var templates = _imageManager.GetImageCategoryList(new PagingModel() { SortBy = "AddedOn", SortOrder = "Desc" });
            return View(templates);
        }
        [AjaxOnly, HttpPost]
        public JsonResult GetImageCategoryPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Templates;
            PagingResult<AdminImageCategoryListingModel> modal = _imageManager.GetImageCategoryList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_imageCategoryListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);

        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddUpdateImageCategory(AddUpdateAdminImageCategoryModel model)
        {
            var result = _imageManager.AddUpdateImageCategory(model);
            return JsonResult(result);
        }

        [HttpPost, AjaxOnly]
        public JsonResult DeleteImageCategory(int id)
        {
            var result = _imageManager.DeleteImageCategory(id);
            return JsonResult(result);
        }



        public ActionResult AddAdminImage(int? id)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Images;
            var model = new AddUpdateAdminImageModel();
            if (id != null)
                model = _imageManager.GetAdminImageById(Convert.ToInt32(id));
            model.Categories = _imageManager.GetImageCategoryDDLList();
            return View(model);
        }

        public ActionResult ManageAdminImage()
        {
            ViewBag.SelectedTab = SelectedAdminTab.Images;
            var templates = _imageManager.GetImageList(PagingModel.DefaultModel("AddedOn"));
            return View(templates);
        }
        [AjaxOnly, HttpPost]
        public JsonResult GetAdminImagePagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.Templates;
            PagingResult<AdminImageListingModel> modal = _imageManager.GetImageList(model);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_AdminImageListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);

        }
        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult AddUpdateAdminImage(AddUpdateAdminImageModel model)
        {
            model.ByAdmin = true;
            model.AddedBy = LOGGEDIN_USER.UserID;
            var result = _imageManager.AddUpdateAdminImage(model);
            return JsonResult(result);
        }

        [HttpPost, AjaxOnly]
        public JsonResult DeleteAdminImage(int id)
        {
            var result = _imageManager.DeleteAdminImage(id);
            return JsonResult(result);
        }


    }
}