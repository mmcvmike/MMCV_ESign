using MMCV_BLL.Document;
using MMCV_Common;
using MMCV_Model.DB68;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers.Form
{
    public class FormManagementController : BaseController
    {
        private readonly string FORM_FOLDER = ConfigurationManager.AppSettings["CDN_FORM_FOLDER"];

        // GET: FormManagement
        public ActionResult FormManagement()
        {
            return View();
        }

        public ActionResult GetForms()
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var listForms = entity.Forms.ToList();
                    return Json(new { rs = true, msg = "", data = listForms }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "FormManagementController");
                return Json(new { rs = false, msg = "Error get form" });
            }
        }

        public ActionResult AddForm(MMCV_Model.DB68.Form form)
        {
            using (var entity = new MMCV_ESignEntities())
            {
                using (var dbcxtransaction = entity.Database.BeginTransaction())
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            // check exist
                            var formExist = entity.Forms
                                .Where(x => (x.FormName.Trim() == form.FormName && x.DeptName.Trim() == form.DeptName))
                                .ToList();

                            if (formExist.Count > 0)
                            {
                                return Json(new { rs = false, msg = $"There is already exist form name: {form.FormName} in Dept: {form.DeptName}" });
                            }

                            form.ModifiedDate = DateTime.Now;
                            form.ModifiedBy = currentUser.Username;

                            entity.Forms.Add(form);
                            entity.SaveChangesAsync();

                            // copy file from Temp folder to server folder if form link exist
                            if (!string.IsNullOrEmpty(form.FormLink))
                            {
                                if (!Directory.Exists(FORM_FOLDER + form.DeptName))
                                {
                                    Directory.CreateDirectory(FORM_FOLDER + form.DeptName);
                                }
                                string fileToCopy = Server.MapPath("~/Data/") + form.FormLink;
                                string destinationFile = FORM_FOLDER + form.DeptName + "\\" + form.FormLink;
                                System.IO.File.Copy(fileToCopy, destinationFile, true);
                            }

                            dbcxtransaction.Commit();

                            return Json(new { rs = true, msg = "Add succssfully" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var errors = ModelState.Select(x => (x.Value.Errors))
                                         .Where(y => y.Count > 0)
                                         .ToList();
                            var returnData = errors.Where(x => x.Count > 0).Select(y => y.FirstOrDefault()).Select(x => x.ErrorMessage);

                            return Json(new { rs = false, msg = string.Join("<br/>", returnData) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        dbcxtransaction.Rollback();
                        LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "FormController");
                        return Json(new { rs = false, msg = "Error add form" });
                    }
                }
            }
        }

        public ActionResult UploadForm()
        {
            try
            {
                var fileName = "";
                var filePath = "";
                var fileSize = 0;
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Data/");

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);

                            fileName = fileContent.FileName;
                            filePath = path + fileContent.FileName;
                            fileSize = fileContent.ContentLength / 1024;
                        }
                    }
                }
                var data = new { fileName = fileName, fileLink = filePath, fileSize = fileSize };

                return Json(new { rs = true, msg = "Upload file successfully", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = ex.Message });
            }
        }

        public ActionResult DownloadForm(int formId = 0)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var form = entity.Forms.Where(x => x.FormID== formId).FirstOrDefault();

                    var filePath = FORM_FOLDER + form.DeptName + "/" + form.FormLink;
                    dynamic returnFile = System.IO.File.ReadAllBytes(filePath);

                    // Convert file stored in CDN to base64
                    string base64String = Convert.ToBase64String(returnFile, 0, returnFile.Length);

                    return Json(new { rs = true, msg = "", data = new { base64Data = base64String, fileName = form.FormLink } }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}