using MMCV_BLL.Document;
using MMCV_BLL.User;
using MMCV_Common;
using MMCV_Model.Document;
using MMCV_Model.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class UserController : Controller
    {
        // GET: Account
        private string sessionUser = ConfigurationManager.AppSettings["SessionUser"];
        private UserBO currentUser = UserBO.Current.CurrentUser();
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserBO user)
        {
            // Verify
            if (ModelState.IsValid)
            {
                UserBLL userBLL = new UserBLL();
                var checkUser = userBLL.GetUser(user);
                if (checkUser == null)
                {
                    return Json(new { rs = false, msg = "Username or password is not correct. Please try again!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Get current user signature
                    var signatures = userBLL.GetUserSignatures(checkUser.UserID);
                    var userSig = new UserSignatureBO();
                    if (signatures.Count > 0)
                    {
                        var defaultSig = signatures.Where(x => x.IsDefault == 1).FirstOrDefault();
                        if (defaultSig != null)
                        {
                            userSig = defaultSig;
                        }
                        else
                        {
                            userSig = signatures[0];
                        }
                    }
                    checkUser.DefaultSignature = userSig;

                    // Set current user for session
                    Session[sessionUser] = checkUser;

                    return Json(new { rs = true, msg = "Login successfully" }, JsonRequestBehavior.AllowGet);
                }
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

        public ActionResult Logout()
        {
            Session[sessionUser] = null;
            return View("Login");
        }

        public ActionResult UserInfo()
        {
            return View(currentUser);
        }

        public ActionResult GetUserInfo()
        {
            return Json(new { rs = false, msg = "", data = currentUser }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserSignatures()
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                var listSignatures = userBLL.GetUserSignatures(currentUser.UserID);

                return Json(new { rs = true, msg = "GetUserSignatures successfully", data = listSignatures }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error GetUserSignatures" });
            }
        }

        public ActionResult AddUserSignature(UserSignatureBO us)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserBLL userBLL = new UserBLL();
                    us.UserID = currentUser.UserID;
                    us.CreatedDate = DateTime.Now;
                    us.IsDefault = 0;
                    us.Active = 1;
                    var addResult = userBLL.AddUserSignature(us);

                    return Json(new { rs = true, msg = "Add signature succssfully", data = addResult }, JsonRequestBehavior.AllowGet);
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
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error add user signature" });
            }
        }

        public ActionResult SetDefaultSignature(UserSignatureBO us)
        {
            try
            {
                UserBLL userBLL = new UserBLL();

                us.UserID = currentUser.UserID;
                var result = userBLL.SetDefaultSignature(us);

                return Json(new { rs = true, msg = "Change default signature succssfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error set default user signature" });
            }
        }

        public ActionResult DeleteSignature(UserSignatureBO us)
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                var deleteResult = userBLL.DeleteSignature(us);

                return Json(new { rs = true, msg = "Delete signature succssfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error delete user signature" });
            }
        }

        public ActionResult UserManagement()
        {
            return View();
        }

        public ActionResult GetUsers()
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                var listUsers = userBLL.GetUsers();

                return Json(new { rs = true, msg = "Get users successfully", data = listUsers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error get users" });
            }
        }

        public ActionResult AddUser(UserBO user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserBLL userBLL = new UserBLL();
                    var addResult = userBLL.AddUser(user);

                    return Json(new { rs = true, msg = "Add user succssfully", data = addResult }, JsonRequestBehavior.AllowGet);
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
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error add user" });
            }
        }

        public ActionResult UpdateUser(UserBO user)
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                var isUpdateSuccess = userBLL.UpdateUser(user);
                if (!isUpdateSuccess)
                {
                    return Json(new { rs = false, msg = "Error update user" });
                }
                return Json(new { rs = true, msg = "Update user successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error update user" });
            }
        }

        public ActionResult GetUserById(int userId)
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                var user = userBLL.GetUserById(userId);

                return Json(new { rs = true, msg = "Get user by id successfully", data = user }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "UserController");
                return Json(new { rs = false, msg = "Error get user by id" });
            }
        }
    }
}