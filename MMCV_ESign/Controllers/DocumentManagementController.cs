using MMCV_BLL.Document;
using MMCV_BLL.DocumentSign;
using MMCV_BLL.Email;
using MMCV_Common;
using MMCV_Common.Helper;
using MMCV_Model.Common;
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

        public ActionResult GetMeSignDocuments()
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                FormSearchDocument frmSearch = new FormSearchDocument()
                {
                    Signer = currentUser.Email,
                    Status = (int)EnumDocumentSign.Initital
                };

                var listDocs = docBLL.GetMeSignDocuments(frmSearch).OrderByDescending(x => x.DocumentID);

                // Filter document which the previous signer has not signed yet

                return Json(new { rs = true, msg = "Get me sign documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get me sign documents" });
            }
        }

        public ActionResult GetSentDocuments()
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                FormSearchDocument frmSearch = new FormSearchDocument()
                {
                    Issuer = currentUser.Email,
                };

                var listDocs = docBLL.GetSentDocuments(frmSearch).OrderByDescending(x => x.DocumentID);

                return Json(new { rs = true, msg = "Get me sign documents successfully", data = listDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error get me sign documents" });
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
                if (ModelState.IsValid){
                    DocumentBLL docBLL = new DocumentBLL();
                    doc.CreatedDate = DateTime.Now;
                    doc.IssuerEmpId = currentUser.EmployeeID;
                    doc.ReferenceCode = Checksum.GenerateReferenceCode();

                    var isAddSuccess = docBLL.AddDocument(doc);
                    if (!isAddSuccess)
                    {
                        return Json(new { rs = false, msg = "Error add document to database" }, JsonRequestBehavior.AllowGet);
                    }

                    if (doc.Status == (int)EnumDocumentStatus.Initital)
                    {
                        var ele = doc.DocumentSigns.OrderBy(x => x.SignIndex).FirstOrDefault();
                        ele.Sender = doc.Issuer;
                        ele.Title = doc.Title;

                        var templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Template/Email/SignDocument.html");
                        var body = EmailSender.ReadEmailTemplate(templatePath);
                        Task.Run(() =>
                        {
                            //var body = $"Dear {ele.Email}," +
                            //        $"<br >You have a document need to sign. Reference code: {doc.ReferenceCode}" +
                            //        $"<br ><a style='height: 60px; background: green;' href='{baseUrl}Home/PdfPage?docId={ele.DocumentID}&email={ele.Email}&signIndex={ele.SignIndex}'>Start sign</a>";
                            //var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", "tuyen.nguyenvan@mmcv.mektec.com", "", body);
                            ////var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", ele.Email, "", body);

                            EmailBO emailBO = new EmailBO()
                            {
                                MailTo = ele.Email,
                                Subject = "Document Sign",
                                Content = body,
                                CC = ""
                            };
                            EmailSender.DocumentSendMail(ele, emailBO);
                        });
                    }

                    return Json(new { rs = true, msg = "Add document succssfully" }, JsonRequestBehavior.AllowGet);
                }
                else {
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
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = CDN_Source_File;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);
                        }
                    }
                }
                return Json(new { rs = true, msg = "Save file document succssfully" }, JsonRequestBehavior.AllowGet);
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
                    returnFile = System.IO.File.ReadAllBytes(CDN_Source_File + doc.Link);
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

                            fileName = fileContent.FileName;
                            filePath = path + fileContent.FileName;
                            fileSize = fileContent.ContentLength / 1024;
                        }
                    }
                }
                var data = new { fileName = fileName, fileLink = filePath, fileSize = fileSize };

                return Json(new { rs = true, msg = "Upload file successfully", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error upload document" });
            }
        }

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
                //var body = $"Dear {firstEmail.Email}," +
                //            "<br >" +
                //            $"<br >You have a document need to sign. Reference code: {firstEmail.DocumentReferenceCode}" +
                //            $"<br >Please access <a href='{baseUrl}/Home/PdfPage?docId={firstEmail.DocumentID}&email={firstEmail.Email}&signIndex={firstEmail.SignIndex}'>this link</a> to sign document";
                //Task.Run(() =>
                //{
                //    var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", firstEmail.Email, "", body);
                //});

                var templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Template/Email/SignDocument.html");
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
                var filePath = CDN_Source_File + doc.Link;
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = doc.Link;

                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/"); // when use jquery.fileDonwload in UI need to set header to handle loading spinner
                return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error download document in detail view" });
            }
        }
    }
}