using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using MMCV_Common;
using MMCV_DAL.Document;
using MMCV_DAL.DocumentSign;
using MMCV_Model.Document;
using MMCV_Model.DocumentSign;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MMCV_BLL.Document
{
    public class DocumentBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;
        private string CDN_Source_File = ConfigurationManager.AppSettings["CDN_Source_File"];

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public DocumentBLL()
        {
        }

        public DocumentBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        public DashboardCountBO GetDashboard(FormSearchDashboard formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetDashboard(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<DocumentBO> GetDocuments(FormSearchDocument formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetDocuments(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<DocumentBO> GetSentDocuments(FormSearchDocument formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetSentDocuments(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<DocumentBO> GetMeSignDocuments(FormSearchDocument formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetMeSignDocuments(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<DocumentBO> GetDraftDocuments(FormSearchDocument formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetDraftDocuments(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<DocumentBO> GetCancelDocuments(FormSearchDocument formSearch)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetCancelDocuments(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public DocumentBO GetDocumentById(int id)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.GetDocumentById(id);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool AddDocument(DocumentBO doc)
        {
            IData objIData;
            if (objDataAccess == null)
            {
                objIData = Data.CreateData(LogHelper.Instance.GetConnectionStringDS(), false);
                objDataAccess = objIData;
            }
            else
                objIData = objDataAccess;
            try
            {
                objIData.BeginTransaction();
                DocumentDAO docDAO = new DocumentDAO(objIData);
                DocumentSignDAO docSignDAO = new DocumentSignDAO(objIData);
                var docId = docDAO.AddDocument(doc);

                doc.DocumentSigns.ForEach((ele) =>
                {
                    ele.DocumentReferenceCode = doc.ReferenceCode;
                    ele.DocumentID = docId;
                    docSignDAO.AddDocumentSign(ele);
                });

                objIData.CommitTransaction();
                return true;
            }
            catch (Exception objEx)
            {
                objIData.RollBackTransaction();
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
            finally
            {
                objIData.Disconnect();
            }
        }

        public bool UpdateStatusDocument(DocumentBO doc)
        {
            try
            {
                DocumentDAO docDAO = new DocumentDAO();
                return docDAO.UpdateStatusDocument(doc);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public string ConvertFileToPdf(DocumentBO doc)
        {
            string documentPath = CDN_Source_File + doc.Link;
            string newDocumentPath = CDN_Source_File + doc.Link.Replace(".docx", ".pdf").Replace(".doc", ".pdf");
            try
            {
                if (File.Exists(documentPath))
                {
                    // If doc is file PDF, copy to other file
                    if (documentPath.ToLower().EndsWith("pdf"))
                    {
                        File.Copy(documentPath, newDocumentPath, true);
                    }
                    // If doc is word document convert it into pdf file
                    else if (documentPath.ToLower().EndsWith(".doc") || documentPath.ToLower().EndsWith(".docx"))
                    {
                        try
                        {
                            object oMissing = System.Reflection.Missing.Value;
                            Type wordType = Type.GetTypeFromProgID("Word.Application");
                            dynamic app = Activator.CreateInstance(wordType);
                            app.Visible = false;
                            if (app.Documents != null)
                            {
                                var document = app.Documents.Open(documentPath, true);
                                document.Activate();
                                if (document != null)
                                {
                                    //document.ExportAsFixedFormat(newDocumentPath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
                                    document.ExportAsFixedFormat(newDocumentPath, 17);
                                    document.Close();
                                }
                                app.Quit();
                            }

                            //// Create a new Microsoft Word application object
                            //Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                            //// C# doesn't have optional arguments so we'll need a dummy value
                            //object oMissing = System.Reflection.Missing.Value;
                            //word.Visible = false;
                            //word.ScreenUpdating = false;

                            //// Cast as Object for word Open method
                            //object filename = documentPath;

                            //// Use the dummy value as a placeholder for optional arguments
                            //var doc1 = word.Documents.Open(ref filename, true, true, false);
                            //doc1.Activate();

                            //object outputFileName = newDocumentPath;
                            //object fileFormat = WdSaveFormat.wdFormatPDF;

                            //// Save document into PDF Format
                            //doc1.SaveAs(ref outputFileName,
                            //    ref fileFormat, ref oMissing, ref oMissing,
                            //    ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            //    ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            //    ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                            //// Close the Word document, but leave the Word application open.
                            //// doc has to be cast to type _Document so that it will find the
                            //// correct Close method.                
                            //object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                            //((_Document)doc1).Close(ref saveChanges, ref oMissing, ref oMissing);
                            //doc1 = null;

                            //// word has to be cast to type _Application so that it will find
                            //// the correct Quit method.
                            //((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                            //word = null;
                        }
                        catch (Exception ex)
                        {
                            this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "");
                            objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                        }
                    }
                    else if (documentPath.ToLower().EndsWith("jpg") || documentPath.ToLower().EndsWith("jpeg") || documentPath.ToLower().EndsWith("png") || documentPath.ToLower().EndsWith("webp"))
                    {
                        if (documentPath.ToLower().EndsWith("webp"))
                        {
                            documentPath += ".png";
                        }
                        try
                        {
                            bool result = ConvertImageToPdf(documentPath, newDocumentPath);
                        }
                        catch (Exception ex)
                        {
                            this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "");
                            objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return newDocumentPath;
        }

        public bool ConvertImageToPdf(string fullPath, string pathDestination)
        {
            try
            {
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(fullPath);
                using (FileStream fs = new FileStream(pathDestination, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(new iTextSharp.text.Rectangle(0, 0, image.Width, image.Height, 0));
                    doc.NewPage();
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                    {
                        doc.Open();
                        image.SetAbsolutePosition(0, 0);
                        writer.DirectContent.AddImage(image);
                        doc.Close();
                        fs.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "");
                objResultMessageBO = LogHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        #endregion
    }
}
