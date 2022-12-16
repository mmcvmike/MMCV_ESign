using MMCV_Common;
using MMCV_DAL.Document;
using MMCV_DAL.DocumentType;
using MMCV_Model.Document;
using MMCV_Model.DocumentType;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMCV_BLL.DocumentType
{
    public class DocumentTypeBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public DocumentTypeBLL()
        {
        }

        public DocumentTypeBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public List<DocumentTypeBO> GetDocumentTypes()
        {
            try
            {
                DocumentTypeDAO docDAO = new DocumentTypeDAO();
                return docDAO.GetDocumentTypes();
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
