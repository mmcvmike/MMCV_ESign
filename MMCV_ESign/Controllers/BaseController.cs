using MMCV_BLL;
using MMCV_Common.Helper;
using MMCV_Common;
using MMCV_Model.Common;
using MMCV_Model.User;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class BaseController : Controller
    {
        protected UserBO currentUser = UserBO.Current.CurrentUser();
        // GET: Base
        public BaseController()
        {
            GetMenu();
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
    }
}