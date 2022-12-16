using MMCV_Model.Common;
using SAB.Library.Data;
using System;
using System.Collections.Generic;

namespace MMCV_DAL
{
    public class CommonDAO : BaseDAO
    {
        #region Constructor

        public CommonDAO() : base()
        {
        }

        public CommonDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Method

        public List<DepartmentBO> GetDepartments()
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = "SELECT * FROM Department";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<DepartmentBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<PositionBO> GetPositions()
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = "SELECT * FROM Position";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<PositionBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        #endregion
    }
}
