using MMCV_BLL.User;
using MMCV_Common;
using MMCV_Model.DB68;
using MMCV_Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class DocumentTypeController : BaseController
    {
        // GET: DocumentType
        public ActionResult DocumentType()
        {
            return View();
        }

        public ActionResult GetDocType()
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var listDocType = entity.DocumentTypes.Where(x => x.EmployeeID == currentUser.EmployeeID).ToList();
                    return Json(new { rs = true, msg = "", data = listDocType }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentTypeController");
                return Json(new { rs = false, msg = "Error get doc type" });
            }
        }

        public ActionResult GetDocTypeById(long id)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var docType = entity.DocumentTypes.Where(x => x.DocumentTypeID == id).FirstOrDefault();
                    return Json(new { rs = true, msg = "", data = docType }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentTypeController");
                return Json(new { rs = false, msg = "Error get doc type by id" });
            }
        }

        public ActionResult GetDocTypeByEmployeeId()
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var listDocType = entity.DocumentTypes.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.Active == 1)).ToList();
                    return Json(new { rs = true, msg = "", data = listDocType }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentTypeController");
                return Json(new { rs = false, msg = "Error get list doc type by emp id" });
            }
        }

        public async Task<ActionResult> AddDocType(DocumentType docType)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    if (ModelState.IsValid)
                    {
                        // check exist
                        var docTypeExist = entity.DocumentTypes.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.DocumentTypeName.Trim().ToLower() == docType.DocumentTypeName.Trim().ToLower())).ToList();
                        if (docTypeExist.Count > 0)
                        {
                            return Json(new { rs = false, msg = $"There is already exist document type name: {docType.DocumentTypeName}" });
                        }
                        docType.EmployeeID = currentUser.EmployeeID;
                        docType.Active = 1;
                        entity.DocumentTypes.Add(docType);
                        await entity.SaveChangesAsync();

                        return Json(new { rs = true, msg = "Add doc type succssfully", data = docType }, JsonRequestBehavior.AllowGet);
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
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentTypeController");
                return Json(new { rs = false, msg = "Error add doc type" });
            }
        }

        public async Task<ActionResult> UpdateDocType(DocumentType docType)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    // check exist
                    var docTypeExist = entity.DocumentTypes.Where(x => x.DocumentTypeID == docType.DocumentTypeID).ToList();
                    if (docTypeExist.Count <= 0)
                    {
                        return Json(new { rs = false, msg = $"There is no exist document type name: {docType.DocumentTypeName}" });
                    }
                    var updatedObj = docTypeExist.FirstOrDefault();

                    if (ModelState.IsValid)
                    {
                        // check exist
                        if (docType.DocumentTypeName != updatedObj.DocumentTypeName)
                        {
                            var docTypeExistName = entity.DocumentTypes.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.DocumentTypeName.Trim().ToLower() == docType.DocumentTypeName.Trim().ToLower())).ToList();
                            if (docTypeExistName.Count > 0)
                            {
                                return Json(new { rs = false, msg = $"There is already exist document type name: {docType.DocumentTypeName}" });
                            }
                        }

                        updatedObj.DocumentTypeName = docType.DocumentTypeName;
                        updatedObj.Description = docType.Description;
                        updatedObj.Active = docType.Active;

                        entity.Entry(updatedObj).State = System.Data.Entity.EntityState.Modified;
                        await entity.SaveChangesAsync();

                        return Json(new { rs = true, msg = "Update doc type succssfully", data = docType }, JsonRequestBehavior.AllowGet);
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
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentTypeController");
                return Json(new { rs = false, msg = "Error update doc type" });
            }
        }
    }
}