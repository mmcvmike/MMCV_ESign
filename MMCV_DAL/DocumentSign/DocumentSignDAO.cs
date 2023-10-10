using MMCV_Model.Document;
using MMCV_Model.DocumentSign;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMCV_DAL.DocumentSign
{
    public class DocumentSignDAO : BaseDAO
    {
        #region Constructor

        public DocumentSignDAO() : base()
        {
        }

        public DocumentSignDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Method

        public List<DocumentSignBO> GetDocumentSignByDocumentID(int docId)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"SELECT * FROM DocumentSign WHERE DocumentID = {docId}";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<DocumentSignBO>(reader);
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

        public bool AddDocumentSign(DocumentSignBO docSign)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"INSERT INTO DocumentSign(DocumentID, DocumentReferenceCode, Fullname, Email, SignIndex, X, Y, Width, Height, Type, Page) " +
                    $"VALUES ({docSign.DocumentID}, '{docSign.DocumentReferenceCode}', '{docSign.Fullname}', '{docSign.Email}', {docSign.SignIndex}, " +
                    $"{docSign.X}, {docSign.Y}, {docSign.Width}, {docSign.Height}, {docSign.Type}, {docSign.Page})";
                BeginTransactionIfAny(objIData);
                objIData.CreateNewSqlText(query);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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

        public bool UpdateStatusDocumentSign(DocumentSignBO docSign)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE DocumentSign" +
                    $" SET Status = {docSign.Status}, SignDate = '{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}'," +
                    $" UserSignatureID = {docSign.UserSignatureID}, UserEmpID = '{docSign.UserEmpID}', Note = '{docSign.Note}'" +
                    $" WHERE DocumentSignID = {docSign.DocumentSignID}";
                BeginTransactionIfAny(objIData);
                objIData.CreateNewSqlText(query);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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
