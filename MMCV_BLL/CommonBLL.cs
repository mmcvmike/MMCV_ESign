using MMCV_Common;
using MMCV_DAL;
using MMCV_Model.Common;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MMCV_BLL
{
    public class CommonBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public CommonBLL()
        {
        }

        public CommonBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public List<DepartmentBO> GetDepartments()
        {
            try
            {
                CommonDAO cDAO = new CommonDAO();
                return cDAO.GetDepartments();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<PositionBO> GetPositions()
        {
            try
            {
                CommonDAO cDAO = new CommonDAO();
                return cDAO.GetPositions();
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
