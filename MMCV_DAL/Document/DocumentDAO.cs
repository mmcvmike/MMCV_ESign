using SAB.Library.Data;
using System.Collections.Generic;
using System;
using MMCV_Model.Document;
using System.Collections;
using MMCV_Common.Helper;
using System.Linq;

namespace MMCV_DAL.Document
{
    public class DocumentDAO : BaseDAO
    {
        #region Constructor

        public DocumentDAO() : base()
        {
        }

        public DocumentDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Method

        public DashboardCountBO GetDashboard(FormSearchDashboard formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("USP_Dashboard_Count");
                objIData.AddParameter("@Email", formSearch.Email);
                //objIData.AddParameter("@Signer", formSearch.Signer);
                //objIData.AddParameter("@Status", formSearch.Status);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DashboardCountBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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

        public List<DocumentBO> GetDocuments(FormSearchDocument formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("USP_Documents_Get");
                objIData.AddParameter("@Issuer", formSearch.Issuer);
                objIData.AddParameter("@Signer", formSearch.Signer);
                objIData.AddParameter("@Status", formSearch.Status);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DocumentBO>(reader);
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

        public List<DocumentBO> GetMeSignDocuments(FormSearchDocument formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("[USP_Documents_Get_MeSign]");
                objIData.AddParameter("@Signer", formSearch.Signer);
                objIData.AddParameter("@Status", string.IsNullOrEmpty(formSearch.Status) ? "" : formSearch.Status);
                objIData.AddParameter("@Title", string.IsNullOrEmpty(formSearch.Title) ? "" : formSearch.Title);
                objIData.AddParameter("@ReferenceCode", string.IsNullOrEmpty(formSearch.ReferenceCode) ? "" : formSearch.ReferenceCode);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DocumentBO>(reader);
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

        public List<DocumentBO> GetSentDocuments(FormSearchDocument formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("USP_Documents_Get_Sent");
                objIData.AddParameter("@Issuer", formSearch.Issuer);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DocumentBO>(reader);
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

        public List<DocumentBO> GetDraftDocuments(FormSearchDocument formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("USP_Documents_Get_Draft");
                objIData.AddParameter("@Issuer", formSearch.Issuer);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DocumentBO>(reader);
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

        public List<DocumentBO> GetCancelDocuments(FormSearchDocument formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("USP_Documents_Get_Cancel");
                objIData.AddParameter("@Issuer", formSearch.Issuer);

                var reader = objIData.ExecStoreToDataReader();
                var list = ConvertToListObject<DocumentBO>(reader);
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

        public DocumentBO GetDocumentById(int id)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"SELECT * FROM Document WHERE DocumentID = {id}";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<DocumentBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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

        public int AddDocument(DocumentBO doc)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"INSERT INTO Document(DocumentName, Issuer, IssuerEmpId, ReferenceCode, Title, DocumentTypeID, Link, CreatedBy, CreatedDate, Status,EmailCC) " +
                    $"VALUES (N'{doc.DocumentName}', N'{doc.Issuer}', '{doc.IssuerEmpId}', '{doc.ReferenceCode}', N'{doc.Title}', {doc.DocumentTypeID}, N'{doc.Link}', N'{doc.CreatedBy}', '{doc.CreatedDate.ToString("yyyy/MM/dd hh:mm:ss")}', {doc.Status},N'{doc.EmailCC}'); " +
                    $"SELECT SCOPE_IDENTITY()";
                BeginTransactionIfAny(objIData);
                objIData.CreateNewSqlText(query);
                var returnId = objIData.ExecStoreToString(query);
                CommitTransactionIfAny(objIData);

                return int.Parse(returnId);
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

        public bool UpdateStatusDocument(DocumentBO doc)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE Document" +
                    $" SET Status = {doc.Status}, Note = N'{doc.Note}'" +
                    $" WHERE DocumentID = {doc.DocumentID}";
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
