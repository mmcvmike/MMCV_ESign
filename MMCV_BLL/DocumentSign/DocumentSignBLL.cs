using MMCV_Common;
using MMCV_DAL.Document;
using MMCV_DAL.DocumentSign;
using MMCV_Model.Document;
using MMCV_Model.DocumentSign;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMCV_BLL.DocumentSign
{
    public class DocumentSignBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public DocumentSignBLL()
        {
        }

        public DocumentSignBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public List<DocumentSignBO> GetDocumentSignByDocumentID(int docId)
        {
            try
            {
                DocumentSignDAO docSignDAO = new DocumentSignDAO();
                return docSignDAO.GetDocumentSignByDocumentID(docId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool AddDocumentSign(DocumentSignBO docSign)
        {
            try
            {
                DocumentSignDAO docSignDAO = new DocumentSignDAO();
                return docSignDAO.AddDocumentSign(docSign);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateStatusDocumentSign(DocumentSignBO docSign)
        {
            try
            {
                DocumentSignDAO docSignDAO = new DocumentSignDAO();
                return docSignDAO.UpdateStatusDocumentSign(docSign);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        #endregion
    }
}
