using MMCV_Model.DocumentType;
using SAB.Library.Data;
using System;
using System.Collections.Generic;

namespace MMCV_DAL.DocumentType
{
    public class DocumentTypeDAO : BaseDAO
    {
        #region Constructor

        public DocumentTypeDAO() : base()
        {
        }

        public DocumentTypeDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Method

        public List<DocumentTypeBO> GetDocumentTypes()
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = "SELECT * FROM DocumentType";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<DocumentTypeBO>(reader);
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
