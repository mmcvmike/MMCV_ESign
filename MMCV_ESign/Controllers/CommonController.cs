using MMCV_BLL.DocumentType;
using MMCV_Common.Helper;
using MMCV_Common;
using MMCV_Model.DocumentType;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using MMCV_BLL;
using MMCV_Model.Common;

namespace MMCV_ESign.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult GetDocumentTypes()
        {
            try
            {
                if (CacheHelper.Get(CacheKeyHelper.DocumentType_CacheKey) == null)
                {
                    DocumentTypeBLL dTBll = new DocumentTypeBLL();
                    var temp = dTBll.GetDocumentTypes();
                    CacheHelper.Set(CacheKeyHelper.DocumentType_CacheKey, temp);
                }
                List<DocumentTypeBO> listDocTypes = (List<DocumentTypeBO>)CacheHelper.Get(CacheKeyHelper.DocumentType_CacheKey);

                return Json(new { rs = true, msg = "", data = listDocTypes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Common");
                return Json(new { rs = false, msg = "Error get document types" });
            }
        }

        public ActionResult GetDepartments()
        {
            try
            {
                if (CacheHelper.Get(CacheKeyHelper.Department_CacheKey) == null)
                {
                    CommonBLL cBLL = new CommonBLL();
                    var temp = cBLL.GetDepartments();
                    CacheHelper.Set(CacheKeyHelper.Department_CacheKey, temp);
                }
                List<DepartmentBO> listDepts = (List<DepartmentBO>)CacheHelper.Get(CacheKeyHelper.Department_CacheKey);

                return Json(new { rs = true, msg = "", data = listDepts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Common");
                return Json(new { rs = false, msg = "Error get department" });
            }
        }

        public ActionResult GetPositions()
        {
            try
            {
                if (CacheHelper.Get(CacheKeyHelper.Position_CacheKey) == null)
                {
                    CommonBLL cBLL = new CommonBLL();
                    var temp = cBLL.GetPositions();
                    CacheHelper.Set(CacheKeyHelper.Position_CacheKey, temp);
                }
                List<PositionBO> listPositions = (List<PositionBO>)CacheHelper.Get(CacheKeyHelper.Position_CacheKey);

                return Json(new { rs = true, msg = "", data = listPositions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Common");
                return Json(new { rs = false, msg = "Error get position" });
            }
        }

        public ActionResult GetMenu()
        {
            try
            {
                if (CacheHelper.Get(CacheKeyHelper.Menu_CacheKey) == null)
                {
                    CommonBLL cBLL = new CommonBLL();
                    var temp = cBLL.GetMenu();
                    CacheHelper.Set(CacheKeyHelper.Menu_CacheKey, temp);
                }
                List<MenuBO> listMenu = (List<MenuBO>)CacheHelper.Get(CacheKeyHelper.Menu_CacheKey);

                return Json(new { rs = true, msg = "", data = listMenu }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Common");
                return Json(new { rs = false, msg = "Error get menu" });
            }
        }
    }
}