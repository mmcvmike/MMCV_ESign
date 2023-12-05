using MMCV_BLL.Document;
using MMCV_BLL.DocumentSign;
using MMCV_BLL.Email;
using MMCV_Common;
using MMCV_Common.Helper;
using MMCV_Model.Common;
using MMCV_Model.DB68;
using MMCV_Model.Document;
using MMCV_Model.DocumentSign;
using MMCV_Model.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class DocumentManagementController : BaseController
    {
        // GET: DocumentManagement
        private string CDN_Source_File = ConfigurationManager.AppSettings["CDN_Source_File"];
        private string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public ActionResult DocumentManagement()
        {
            //using (MMCV_ESignEntities entity = new MMCV_ESignEntities())
            //{
            //    ViewBag.Tags = entity.Tags.Where(x => x.EmployeeID == currentUser.EmployeeID && x.Active == 1).ToList();
            //    return View();
            //}
            return View();
        }

        public ActionResult GetDocuments()
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                FormSearchDocument frmSearch = new FormSearchDocument()
                {
                    Signer = currentUser.Email,
                    Issuer = currentUser.Email,
                };

                var listDocs = docBLL.GetDocuments(frmSearch);

                return Json(new { rs = true, msg = "Get documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get documents" });
            }
        }

        public ActionResult GetMeSignDocuments(FormSearchDocument frmSearch)
        {
            try
            {
                //DocumentBLL docBLL = new DocumentBLL();
                //FormSearchDocument frmSearch = new FormSearchDocument()
                //{
                //    Signer = currentUser.Email,
                //    Status = (int)EnumDocumentSign.Initital
                //};
                //var listDocs = docBLL.GetMeSignDocuments(frmSearch).OrderByDescending(x => x.DocumentID);
                using (MMCV_ESignEntities entity = new MMCV_ESignEntities())
                {
                    DateTime? fromDate = null;
                    if (!string.IsNullOrEmpty(frmSearch.FromDate))
                    {
                        fromDate = DateTime.Parse(frmSearch.FromDate);
                    }

                    DateTime? toDate = null;
                    if (!string.IsNullOrEmpty(frmSearch.ToDate))
                    {
                        toDate = DateTime.Parse(frmSearch.ToDate);
                    }

                    var listDocs = entity.Documents
                                    .Join(entity.DocumentSigns,
                                          t1 => t1.DocumentID,
                                          t2 => t2.DocumentID,
                                          (t1, t2) => new { t1, t2 })
                                    .Where(x => (x.t2.Email == currentUser.Email && x.t1.Status != 3 && x.t1.Status != 4))
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.Title), x => x.t1.Title.Contains(frmSearch.Title))
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.Status), x => x.t2.Status.ToString() == frmSearch.Status)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.ReferenceCode), x => x.t1.ReferenceCode == frmSearch.ReferenceCode)
                                    //.WhereIf(!string.IsNullOrEmpty(frmSearch.DocumentType), x => x.t1.DocumentTypeID.ToString() == frmSearch.DocumentType)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.FromDate), x => x.t1.CreatedDate >= fromDate)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.ToDate), x => x.t1.CreatedDate <= toDate)
                                    .Select(x => new
                                    {
                                        DocumentID = x.t1.DocumentID,
                                        DocumentName = x.t1.DocumentName,
                                        DocumentTypeID = x.t1.DocumentTypeID,
                                        Issuer = x.t1.Issuer,
                                        IssuerEmpId = x.t1.IssuerEmpId,
                                        Title = x.t1.Title,
                                        Status = x.t1.Status,
                                        Link = x.t1.Link,
                                        ReferenceCode = x.t1.ReferenceCode,
                                        Note = x.t1.Note,
                                        CreatedBy = x.t1.CreatedBy,
                                        CreatedDate = x.t1.CreatedDate,
                                        Active = x.t1.Active,
                                        SignerStatus = x.t2.Status
                                    })
                                    .OrderByDescending(x => x.DocumentID)
									.DistinctBy(x => x.DocumentID)
									.ToList();

                    // Filter document which the previous signer has not signed yet

                    // Filter by tags
                    var returnData = listDocs;
                    if (frmSearch.TagId != 0)
                    {
                        var tagDocList = entity.TagDocs
                                         .Where(x => x.EmpID == currentUser.EmployeeID && x.TagID == frmSearch.TagId)
                                         .ToList();

                        returnData = listDocs
                                     .Where(x => tagDocList.Any(y => y.DocID == x.DocumentID)).ToList();
                    }
                    
                    return Json(new { rs = true, msg = "Get me sign documents successfully", data = returnData }, JsonRequestBehavior.AllowGet);
                    //return Json(new { rs = true, msg = "Get me sign documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get me sign documents" });
            }
        }

        public ActionResult GetSentDocuments(FormSearchDocument frmSearch)
        {
            try
            {
                //DocumentBLL docBLL = new DocumentBLL();
                //FormSearchDocument frmSearch = new FormSearchDocument()
                //{
                //    Issuer = currentUser.Email,
                //};

                //var listDocs = docBLL.GetSentDocuments(frmSearch).OrderByDescending(x => x.DocumentID);

                //return Json(new { rs = true, msg = "Get me sign documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);

                using (MMCV_ESignEntities entity = new MMCV_ESignEntities())
                {

                    DateTime? fromDate = null;
                    if (!string.IsNullOrEmpty(frmSearch.FromDate))
                    {
                        fromDate = DateTime.Parse(frmSearch.FromDate);
                    }

                    DateTime? toDate = null;
                    if (!string.IsNullOrEmpty(frmSearch.ToDate))
                    {
                        toDate = DateTime.Parse(frmSearch.ToDate);
                    }

                    var listDocs = entity.Documents
                                    .Join(entity.DocumentSigns,
                                          t1 => t1.DocumentID,
                                          t2 => t2.DocumentID,
                                          (t1, t2) => new { t1, t2 })
                                    .Where(x => (x.t1.Issuer == currentUser.Email && x.t1.Status != 3 && x.t1.Status != 4))
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.Title), x => x.t1.Title.Contains(frmSearch.Title))
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.Status), x => x.t1.Status.ToString() == frmSearch.Status)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.SignStatus), x => x.t2.Status.ToString() == frmSearch.SignStatus)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.ReferenceCode), x => x.t1.ReferenceCode == frmSearch.ReferenceCode)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.DocumentType), x => x.t1.DocumentTypeID.ToString() == frmSearch.DocumentType)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.FromDate), x => x.t1.CreatedDate >= fromDate)
                                    .WhereIf(!string.IsNullOrEmpty(frmSearch.ToDate), x => x.t1.CreatedDate <= toDate)
                                    .Select(x => new
                                    {
                                        DocumentID = x.t1.DocumentID,
                                        DocumentName = x.t1.DocumentName,
                                        DocumentTypeID = x.t1.DocumentTypeID,
                                        Issuer = x.t1.Issuer,
                                        IssuerEmpId = x.t1.IssuerEmpId,
                                        Title = x.t1.Title,
                                        Status = x.t1.Status,
                                        Link = x.t1.Link,
                                        ReferenceCode = x.t1.ReferenceCode,
                                        Note = x.t1.Note,
                                        CreatedBy = x.t1.CreatedBy,
                                        CreatedDate = x.t1.CreatedDate,
                                        Active = x.t1.Active,
                                        SignerStatus = x.t2.Status
                                    })
                                    .OrderByDescending(x => x.DocumentID)
                                    .DistinctBy(x => x.DocumentID)
                                    .ToList();

                    // Filter document which the previous signer has not signed yet

                    return Json(new { rs = true, msg = "Get sent documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get sent documents" });
            }
        }

        public ActionResult GetDraftDocuments()
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                FormSearchDocument frmSearch = new FormSearchDocument()
                {
                    Issuer = currentUser.Email,
                };

                var listDocs = docBLL.GetDraftDocuments(frmSearch).OrderByDescending(x => x.DocumentID);

                return Json(new { rs = true, msg = "Get draft documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get draft documents" });
            }
        }

        public ActionResult GetCancelDocuments()
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                FormSearchDocument frmSearch = new FormSearchDocument()
                {
                    Issuer = currentUser.Email,
                };

                var listDocs = docBLL.GetCancelDocuments(frmSearch).OrderByDescending(x => x.DocumentID);

                return Json(new { rs = true, msg = "Get cancel documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get cancel documents" });
            }
        }

        public ActionResult AddDocument(DocumentBO doc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DocumentBLL docBLL = new DocumentBLL();
                    doc.CreatedDate = DateTime.Now;
                    doc.IssuerEmpId = currentUser.EmployeeID;
                    doc.ReferenceCode = Checksum.GenerateReferenceCode();
                    doc.Link = doc.Link.ToLower();

                    if (doc.DocumentSigns == null || doc.DocumentSigns.Count == 0)
                    {
                        return Json(new { rs = false, msg = "You need to add at least one signature for one person" }, JsonRequestBehavior.AllowGet);
                    }

                    var docId = docBLL.AddDocument(doc);
                    if (docId <= 0)
                    {
                        return Json(new { rs = false, msg = "Error add document to database" }, JsonRequestBehavior.AllowGet);
                    }

                    // save file to folder of user
                    var tempPath = CDN_Source_File + currentUser.EmployeeID + "/" + doc.Link;
                    if (System.IO.File.Exists(tempPath))
                    {
                        var correctFilePath = CDN_Source_File + currentUser.EmployeeID + "/" + docId + "/";
                        if (!Directory.Exists(correctFilePath))
                        {
                            Directory.CreateDirectory(correctFilePath);
                            System.IO.File.Copy(tempPath, correctFilePath + doc.Link);

                            //if (doc.Link.EndsWith("docx") || doc.Link.EndsWith("doc"))
                            //{
                            //    System.IO.File.Copy(tempPath.Replace(".docx", ".pdf").Replace(".doc", ".pdf"), correctFilePath + doc.Link.Replace(".docx", ".pdf").Replace(".doc", ".pdf"));
                            //}
                            //else if (doc.Link.EndsWith("jpg") || doc.Link.EndsWith("jpeg") || doc.Link.EndsWith("png"))
                            //{
                            //    System.IO.File.Copy(tempPath.Replace(".jpg", ".pdf").Replace(".jpeg", ".pdf").Replace(".png", ".pdf"), correctFilePath + doc.Link.Replace(".jpg", ".pdf").Replace(".jpeg", ".pdf").Replace(".png", ".pdf"));
                            //}
                        }
                    }

                    // send mail for signer
                    if (doc.DocumentSigns != null && doc.DocumentSigns.Count > 0)
                    {
                        if (doc.Status == (int)EnumDocumentStatus.Initital)
                        {
                            var ele = doc.DocumentSigns.OrderBy(x => x.SignIndex).FirstOrDefault();
                            ele.Sender = doc.Issuer;
                            ele.Title = doc.Title;
                            //var templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Template/Email/SignDocument.html");
                            var templatePath = Server.MapPath("~/Template/Email/SignDocument.html");
                            var body = EmailSender.ReadEmailTemplate(templatePath);
                            Task.Run(() =>
                            {
                                EmailBO emailBO = new EmailBO()
                                {
                                    MailTo = ele.Email,
                                    Subject = "Document Sign",
                                    Content = body,
                                    CC = doc.EmailCC,
                                };
                                EmailSender.DocumentSendMail(ele, emailBO);
                            });
                        }
                    }

                    return Json(new { rs = true, msg = "Add document succssfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var errors = ModelState.Select(x => (x.Value.Errors))
                                 .Where(y => y.Count > 0)
                                 .ToList();
                    var returnData = errors.Where(x => x.Count > 0).Select(y => y.FirstOrDefault()).Select(x => x.ErrorMessage);

                    return Json(new { rs = false, msg = string.Join("<br/>", returnData) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error add document" });
            }
        }

        public ActionResult SaveFile()
        {
            try
            {
				var userFolder = currentUser.EmployeeID;
				string path = CDN_Source_File + userFolder + "/";

				foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
						if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var fileName = fileContent.FileName.ToLower();
                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileName);

                            //// if file is not pdf then save one more file under pdf format
                            //var pdfFileName = fileName;
                            //if (fileName.EndsWith("docx") || fileName.EndsWith("doc"))
                            //{
                            //    pdfFileName = fileName.Replace(".docx", ".pdf").Replace(".doc", ".pdf");
                            //}
                            //else if (fileName.EndsWith("jpg") || fileName.EndsWith("jpeg") || fileName.EndsWith("png"))
                            //{
                            //    pdfFileName = fileName.Replace(".jpg", ".pdf").Replace(".jpeg", ".pdf").Replace(".png", ".pdf");
                            //}
                            //postedFile.SaveAs(path + pdfFileName);
                        }
                    }
                }
                return Json(new { rs = true, msg = "Save file document successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error save file document" });
            }
        }

        public ActionResult GetDocumentBase64Data(int docId = 0)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                var doc = docBLL.GetDocumentById(docId);
                dynamic returnFile;
                if (doc.Link.Contains(".doc") || doc.Link.Contains(".docx"))
                {
                    var tempFile = docBLL.ConvertFileToPdf(doc);
                    returnFile = System.IO.File.ReadAllBytes(tempFile);
                }
                else
                {
                    var path = CDN_Source_File + doc.IssuerEmpId + "/" + docId + "/";
					returnFile = System.IO.File.ReadAllBytes(path + doc.Link);
                }

                // Convert file stored in CDN to base64
                string base64String = Convert.ToBase64String(returnFile, 0, returnFile.Length);
                doc.Base64File = base64String;

                return Json(new { rs = true, msg = "", data = doc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DetailView(int docId = 0)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();

                // Get document info
                var doc = docBLL.GetDocumentById(docId);

                // Get document sign info
                var docSign = docSignBLL.GetDocumentSignByDocumentID(docId);
                doc.DocumentSigns = docSign;

                //var returnData = new Tuple<DocumentBO, List<DocumentSignBO>>(doc, docSign);

                return View(doc);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public ActionResult UploadDocumentFile()
        {
            try
            {
                string base64String = "";
                var newTestPath = "";
                var newTestFileName = "";
                var fileName = "";
                var filePath = "";
                var fileSize = 0;
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Data/");

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);

                            fileName = fileContent.FileName.ToLower();
                            filePath = path + fileContent.FileName;
                            fileSize = fileContent.ContentLength / 1024;

                            newTestPath = filePath;
                            newTestFileName = fileName;

                            if (fileName.EndsWith("docx") || fileName.EndsWith("doc"))
                            {
                                object oMissing = System.Reflection.Missing.Value;
                                Type wordType = Type.GetTypeFromProgID("Word.Application");
                                dynamic app = Activator.CreateInstance(wordType);
                                app.Visible = false;
                                if (app.Documents != null)
                                {
                                    var document = app.Documents.Open(filePath, true);
                                    document.Activate();
                                    if (document != null)
                                    {
                                        //document.ExportAsFixedFormat(newDocumentPath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
                                        newTestFileName = fileName.Replace(".docx", ".pdf").Replace(".doc", ".pdf");
                                        newTestPath = path + newTestFileName;
                                        document.ExportAsFixedFormat(newTestPath, 17);
                                        document.Close();
                                    }
                                    app.Quit();
                                }
                            }
                            else if (fileName.EndsWith("xlsx") || fileName.EndsWith("xls"))
                            {
                                object oMissing = System.Reflection.Missing.Value;
                                Type excelType = Type.GetTypeFromProgID("Excel.Application");
                                dynamic excelApp = Activator.CreateInstance(excelType);
                                excelApp.Visible = false;

                                if (excelApp.Workbooks != null)
                                {
                                    var workbook = excelApp.Workbooks.Open(filePath);
                                    if (workbook != null)
                                    {
                                        // Save the Excel workbook as PDF
                                        //workbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, "D:\\test.pdf");
                                        newTestFileName = fileName.Replace(".xlsx", ".pdf").Replace(".xls", ".pdf");
                                        newTestPath = path + newTestFileName;
                                        workbook.ExportAsFixedFormat(0, newTestPath);
                                        workbook.Close();
                                    }

                                    excelApp.Quit();
                                }
                            }
                            else if (fileName.EndsWith("jpg") || fileName.EndsWith("jpeg") || fileName.EndsWith("png"))
                            {
                                newTestFileName = fileName.Replace(".jpg", ".pdf").Replace(".jpeg", ".pdf").Replace(".png", ".pdf");
                                newTestPath = path + newTestFileName;
                                DocumentBLL docBLL = new DocumentBLL();
                                docBLL.ConvertImageToPdf(filePath, newTestPath);
                            }

                            // Convert file stored in CDN to byte array
                            dynamic returnFile;
                            returnFile = System.IO.File.ReadAllBytes(newTestPath);
                            base64String = Convert.ToBase64String(returnFile, 0, returnFile.Length);
                        }
                    }
                }
                //var data = new { fileName = fileName, fileLink = filePath, fileSize = fileSize };
                var data = new { fileName = newTestFileName, fileLink = newTestPath, fileSize = fileSize, base64Data = base64String };

                return Json(new { rs = true, msg = "Upload file successfully", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error upload document" });
            }
        }

        //public ActionResult UploadDocumentFile()
        //{
        //    try
        //    {
        //        var fileName = "";
        //        var filePath = "";
        //        var fileSize = 0;
        //        foreach (string file in Request.Files)
        //        {
        //            var fileContent = Request.Files[file];
        //            if (fileContent != null && fileContent.ContentLength > 0)
        //            {
        //                string path = Server.MapPath("~/Data/");

        //                if (!Directory.Exists(path))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }

        //                foreach (string key in Request.Files)
        //                {
        //                    HttpPostedFileBase postedFile = Request.Files[key];
        //                    postedFile.SaveAs(path + fileContent.FileName);

        //                    fileName = fileContent.FileName;
        //                    filePath = path + fileContent.FileName;
        //                    fileSize = fileContent.ContentLength / 1024;
        //                }
        //            }
        //        }
        //        var data = new { fileName = fileName, fileLink = filePath, fileSize = fileSize };

        //        return Json(new { rs = true, msg = "Upload file successfully", data = data }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
        //        return Json(new { rs = false, msg = "Error upload document" });
        //    }
        //}

        public ActionResult IssueAndSendMail(DocumentBO doc)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();

                doc.Status = (int)EnumDocumentStatus.Initital;
                var isUpdateStatusSuccess = docBLL.UpdateStatusDocument(doc);
                if (!isUpdateStatusSuccess)
                {
                    return Json(new { rs = false, msg = "Update document status from draft to initial failed" });
                }

                var documentSignList = docSignBLL.GetDocumentSignByDocumentID(doc.DocumentID);
                if (doc.Status == (int)EnumDocumentStatus.Initital)
                {
                    Task.Run(() =>
                    {
                        SendMailToSigner(documentSignList);
                    });
                }

                return Json(new { rs = true, msg = "Issue document and send mail succssfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error issue doc and send mail to signer" });
            }
        }

        [HttpPost]
        public ActionResult SendMail(DocumentBO doc)
        {
            try
            {
                DocumentSignBLL docSignBLL = new DocumentSignBLL();
                var documentSignList = docSignBLL.GetDocumentSignByDocumentID(doc.DocumentID);

                if (doc.Status == (int)EnumDocumentStatus.Initital)
                {
                    Task.Run(() =>
                    {
                        SendMailToSigner(documentSignList);
                    });
                }

                return Json(new { rs = true, msg = "Send mail to signer succssfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error send mail to signer" });
            }
        }

        [NonAction]
        private void SendMailToSigner(List<DocumentSignBO> listEmail)
        {
            var firstEmail = listEmail.Where(x => x.Status == (int)EnumDocumentSign.Initital).OrderBy(x => x.SignIndex).FirstOrDefault();
            if (firstEmail != null)
            {
                var templatePath = HttpContext.Server.MapPath("~/Template/Email/SignDocument.html");
                var body = EmailSender.ReadEmailTemplate(templatePath);
                Task.Run(() =>
                {
                    EmailBO emailBO = new EmailBO()
                    {
                        MailTo = firstEmail.Email,
                        Subject = "Document Sign",
                        Content = body,
                        CC = ""
                    };
                    EmailSender.DocumentSendMail(firstEmail, emailBO);
                });
            }
        }

        public ActionResult Download(DocumentBO doc)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();

                doc = docBLL.GetDocumentById(doc.DocumentID);
                if (docBLL.ResultMessageBO.IsError)
                {
                    LogHelper.Instance.WriteLog(docBLL.ResultMessageBO.Message, docBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Download Document");
                }

				string path = CDN_Source_File + doc.IssuerEmpId + "/" + doc.DocumentID + "/";
				var filePath = path + doc.Link;

                var returnFile = docBLL.ConvertFileToPdf(doc);

                //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                byte[] fileBytes = System.IO.File.ReadAllBytes(returnFile);
                //string fileName = doc.Link;
                string fileName = doc.Link;
                if (fileName.EndsWith("docx") || fileName.EndsWith("doc"))
                {
                    fileName = fileName.Replace(".docx", ".pdf").Replace(".doc", ".pdf");
                }
                else if (fileName.EndsWith("jpg") || fileName.EndsWith("jpeg") || fileName.EndsWith("png"))
                {
                    fileName = fileName.Replace(".jpg", ".pdf").Replace(".jpeg", ".pdf").Replace(".png", ".pdf");
                }

                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/"); // when use jquery.fileDonwload in UI need to set header to handle loading spinner
                //return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
                return File(fileBytes, MediaTypeNames.Application.Zip, fileName);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error download document in detail view" });
            }
        }

        public ActionResult AssignDocToTag(int docId, int tagId)
        {
            try
            {
                using (MMCV_ESignEntities entity = new MMCV_ESignEntities())
                {
                    var objExist = entity.TagDocs
                        .Where(x => x.EmpID == currentUser.EmployeeID && x.DocID == docId)
                        .FirstOrDefault();

                    if (objExist == null)
                    {
                        entity.TagDocs.Add(new TagDoc()
                        {
                            DocID = docId,
                            TagID = tagId,
                            EmpID = currentUser.EmployeeID
                        });
                    }
                    else
                    {
                        if (objExist.TagID != tagId) // update
                        {
                            objExist.TagID = tagId;
                            entity.Entry(objExist).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    entity.SaveChanges();
                }

                return Json(new { rs = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "" });
            }
        }
    }
}