using MMCV_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMCV_Model.User
{
    public class UserBO
    {
        private static UserBO _instance;

        public static UserBO Current
        {
            get { return _instance ?? (_instance = new UserBO()); }
        }

        public UserBO CurrentUser()
        {
            try
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null)
                //{
                //    HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);

                //    return session["SessionUser"] as UserBO;
                //}

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    var session = HttpContext.Current.Session[HttpContext.Current.Request.Cookies["SessionId"].Value];

                    return session as UserBO;
                }
            }
            catch (Exception objEx)
            {
                LogHelper.Instance.WriteLog("Không lấy được thông tin user", objEx, "CurrentUser", "UserBO");
            }
            return null;
        }

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int DepartmentID { get; set; }
        public int PositionID { get; set; }
        public int RoleID { get; set; }
        public string EmployeeID { get; set; }
        public string Password { get; set; }
        public int Active { get; set; }

        // customize properties
        public UserSignatureBO DefaultSignature { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string RoleName { get; set; }
        public string StampBase64 { get; set; }
        public bool IsAdmin
        {
            get
            {
                return RoleName == "Admin" ? true : false;
            }
        }
    }

    public class UserSignatureBO
    {
        public int UserSignatureID { get; set; }
        public int UserID { get; set; }
        public string Base64Signature { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Active { get; set; }
        public int IsDefault { get; set; }
    }
}
