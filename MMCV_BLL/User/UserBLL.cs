using MMCV_Common;
using MMCV_DAL.DocumentSign;
using MMCV_DAL.DocumentType;
using MMCV_DAL.User;
using MMCV_Model.DocumentSign;
using MMCV_Model.DocumentType;
using MMCV_Model.User;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMCV_BLL.User
{
    public class UserBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public UserBLL()
        {
        }

        public UserBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public UserBO GetUser(UserBO user)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetUser(user);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<UserSignatureBO> GetUserSignatures(int userId)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetUserSignatures(userId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }


        public UserSignatureBO AddUserSignature(UserSignatureBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.AddUserSignature(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool SetDefaultSignature(UserSignatureBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.SetDefaultSignature(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool DeleteSignature(UserSignatureBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.DeleteSignature(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public List<UserBO> GetUsers()
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetUsers();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public UserBO AddUser(UserBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.AddUser(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool UpdateUser(UserBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.UpdateUser(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateUserStamp(UserBO us)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.UpdateUserStamp(us);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UnActiveUser(int userId)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.UnActiveUser(userId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public UserBO GetUserById(int userId)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetUserById(userId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        #endregion
    }
}
