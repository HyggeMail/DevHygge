#region Default Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
#endregion

#region Custom Namespaces
using HyggeMail.Attributes;
using HyggeMail.BLL.Models;
using HyggeMail.BLL.Interfaces;
using System.IO;
using Excel;
using System.Data;
using HyggeMail.BLL.Interfaces.Admin_DashBoard;
using HyggeMail.ImportExport;

#endregion

namespace HyggeMail.Areas.Admin.Controllers
{
    public class AddressBookController : AdminBaseController
    {
        #region Variable Declaration
        private readonly IUserManager _userManager;
        private readonly IAddressBookManager _addressBookManager;
        #endregion

        public AddressBookController(IUserManager userManager, IErrorLogManager errorLogManager, IAddressBookManager addressBookManager)
            : base(errorLogManager)
        {
            _userManager = userManager;
            _addressBookManager = addressBookManager;

        }

        [HttpGet]
        public ActionResult ImportAddresses()
        {
            ViewBag.SelectedTab = SelectedAdminTab.AddressBook;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportAddresses(HttpPostedFileBase uploadfile)
        {
            if (ModelState.IsValid)
            {
                if (uploadfile != null && uploadfile.ContentLength > 0)
                {
                    var obj = new ImportExcel();
                    var result = obj.ImportExcelSheet(uploadfile);

                    if (result == null)
                    {
                        //Shows error if uploaded file is not Excel file
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    var response = _addressBookManager.SaveAddressesImportExcel(result, LOGGEDIN_USER.UserID);

                    if (response.Status == ActionStatus.Successfull)
                    {
                        if (response.Object.Rows.Count > 0)
                        {
                            ViewBag.AddressBookResponse = "Matched Records Saved Successfully.";
                            ViewBag.MissingTitle = "Mismatched execel file result";

                            TempData["DataTable"] = response.Object;

                            return View(response.Object);
                        }
                        else
                        {
                            return RedirectToAction("ManageAddressBook");
                        }
                    }
                    else
                        ModelState.AddModelError("File", response.Message);
                }
                else
                    ModelState.AddModelError("File", "Please upload your file");
            }
            else
            {
                ModelState.AddModelError("File", "Please upload your file");
            }
            return View();
        }

        public ActionResult ExportExcel()
        {
            var obj = new ExportDataTable();

            var dt = (DataTable)TempData["DataTable"];

            var stream = obj.ExportExcel(dt);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MismatchedResult.xlsx");
        }

        [HttpGet]
        public virtual FileResult DownloadExcelFormat()
        {
            string fullPath = Path.Combine(Server.MapPath("~/Uploads/ExcelFormat/"), "ExcelFormat.xls");
            return File(fullPath, "application/vnd.ms-excel", "ExcelFormat.xls");
        }

        [HttpGet]
        public ActionResult ManageAddressBook()
        {
            ViewBag.SelectedTab = SelectedAdminTab.AddressBook;
            var users = _addressBookManager.GetAddessBookPagedList(PagingModel.DefaultModel("Name"), LOGGEDIN_USER.UserID);
            return View(users);
        }

        [AjaxOnly, HttpPost]
        public JsonResult GetAddressBookPagingList(PagingModel model)
        {
            ViewBag.SelectedTab = SelectedAdminTab.AddressBook;
            var modal = _addressBookManager.GetAddessBookPagedList(model, LOGGEDIN_USER.UserID);
            List<string> resultString = new List<string>();
            resultString.Add(RenderRazorViewToString("Partials/_addressBookListing", modal));
            resultString.Add(modal.TotalCount.ToString());
            return JsonResult(resultString);
        }

        public ActionResult EditAddress(int addressId)
        {
            var model = _addressBookManager.GetBookAddressByID(LOGGEDIN_USER.UserID, addressId);

            if (model.Status == ActionStatus.Successfull)
                return View(model.Object);
            else
                return View(new AddUpdateRecipientModel());
        }

        [AjaxOnly, HttpPost]
        public JsonResult DeleteBookAddress(int id)
        {
            var result = _addressBookManager.DeleteBookAddressById(LOGGEDIN_USER.UserID, id);
            return JsonResult(result);
        }

        [HttpPost, AjaxOnly]
        [ValidateInput(false)]
        public JsonResult UpdateBookAddress(AddUpdateRecipientModel model)
        {
            model.UserID = LOGGEDIN_USER.UserID;
            var result = _addressBookManager.AddUpdateAddressBook(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}