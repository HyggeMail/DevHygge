using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyggeMail.Areas.Admin.Controllers
{
    public class ManageContentController : Controller
    {
        //
        // GET: /Admin/ManageContent/
        public ActionResult Index()
        {
            return View();
        }

        #region Common Fields

        public void UploadImage(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string ImageName = upload.FileName;

                var UploadPath = Server.MapPath("~/Uploads/CkUploads/" + Session["PostGuid"] + "/");
                bool exists = System.IO.Directory.Exists(UploadPath);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(UploadPath);
                }
                string path = System.IO.Path.Combine(UploadPath, ImageName);
                upload.SaveAs(path);
            }
        }

        public ActionResult UploadPartial()
        {
            var appData = Server.MapPath("~/Uploads/CkUploads/" + Session["PostGuid"] + "/");
            bool exists = System.IO.Directory.Exists(appData);
            if (exists)
            {
                var images = Directory.GetFiles(appData).Select(x => new ImageViewModel
                {
                    Url = Url.Content("~/Uploads/CkUploads/" + Session["PostGuid"] + "/" + Path.GetFileName(x))
                });
                return View(images);
            }
            return View();
        }

        #endregion
	}
}