using MMCV_BLL.Document;
using MMCV_Model.Document;
using MMCV_Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class DashboardController : Controller
    {
        private UserBO currentUser = UserBO.Current.CurrentUser();
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            //  
            DocumentBLL docBLL = new DocumentBLL();
            var formSearch = new FormSearchDashboard()
            {
                Email = currentUser.Email
            };
            var dashboardObj = docBLL.GetDashboard(formSearch);
            return View(dashboardObj);
        }
    }
}