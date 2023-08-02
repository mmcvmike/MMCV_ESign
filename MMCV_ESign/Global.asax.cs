using MMCV_Common;
using MMCV_Model.User;
using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MMCV_ESign
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private string SessionUser = ConfigurationManager.AppSettings["SessionUser"];
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                string strUrl = HttpContext.Current.Request.Url.ToString();
                //string rawUrl = HttpContext.Current.Request.RawUrl;
                string strUserAgent = HttpContext.Current.Request.UserAgent;

                //if (!string.IsNullOrEmpty(strUserAgent))
                //{
                //    if (Session[SessionUser] != null)
                //    {
                //        UserBO objUser = (UserBO)Session[SessionUser];
                //    }
                //    else if (!strUrl.Contains("/User/Login"))
                //    {
                //        Session["PreURL"] = strUrl; // Lưu trữ thông tin đường dẫn trước khi đăng nhập để khi đăng nhập thành công thì redirect đến trang đó
                //        Response.Redirect("~/User/Login");
                //    }
                //}

                //if (Session[SessionUser] != null)
                //{
                //    UserBO objUser = (UserBO)Session[SessionUser];
                //}
                //else if (!strUrl.Contains("/User/Login"))
                //{
                //    Session["PreURL"] = strUrl; // Lưu trữ thông tin đường dẫn trước khi đăng nhập để khi đăng nhập thành công thì redirect đến trang đó
                //    Response.Redirect("~/User/Login");
                //}

                string sessionId = Request.Cookies["SessionId"]?.Value;
                if (Session[sessionId] != null)
                {
                    UserBO objUser = (UserBO)Session[sessionId];
                }
                else if (!strUrl.Contains("/User/Login"))
                {
                    Session["PreURL"] = strUrl; // Lưu trữ thông tin đường dẫn trước khi đăng nhập để khi đăng nhập thành công thì redirect đến trang đó
                    Response.Redirect("~/User/Login");
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog("Application_AcquireRequestState", ex, "Application_AcquireRequestState", "Global.asax");
            }
        }
    }
}
