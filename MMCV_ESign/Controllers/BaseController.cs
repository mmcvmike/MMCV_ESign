using MMCV_Model.User;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public ActionResult Index()
        {
            return View();
        }
    }
}