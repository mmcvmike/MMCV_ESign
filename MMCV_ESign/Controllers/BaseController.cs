using MMCV_BLL;
using MMCV_Common.Helper;
using MMCV_Common;
using MMCV_Model.Common;
using MMCV_Model.User;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Web.Mvc;
using MMCV_Model.DB68;
using System.Linq;

namespace MMCV_ESign.Controllers
{
    public class BaseController : Controller
    {
        protected UserBO currentUser = UserBO.Current.CurrentUser();
        private const string USER_TAG = "USER_TAG";
        // GET: Base
        public BaseController()
        {
            //GetMenu();
            GetTags();
        }

        public ActionResult CheckSession()
        {
            return Json(new { rs = true, msg = "session alive" }, JsonRequestBehavior.AllowGet );
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
        }

        private void GetMenu()
        {
            try
            {
                if (CacheHelper.Get(CacheKeyHelper.Menu_CacheKey) == null)
                {
                    CommonBLL cBLL = new CommonBLL();
                    var temp = cBLL.GetMenu();
                    CacheHelper.Set(CacheKeyHelper.Menu_CacheKey, temp);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "BaseController");
            }
        }

        private void GetTags()
        {
            try
            {
                if (CacheHelper.Get(USER_TAG) == null)
                {
                    using (MMCV_ESignEntities entity = new MMCV_ESignEntities())
                    {
                        var temp = ViewBag.UserTags = entity.Tags
                            .Where(x => x.EmployeeID == currentUser.EmployeeID && x.Active == 1)
                            .ToList();

                        CacheHelper.Set(USER_TAG, temp, 2);
                    }
                }
                ViewBag.UserTags = CacheHelper.Get(USER_TAG);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "BaseController");
            }
        }
    }
}