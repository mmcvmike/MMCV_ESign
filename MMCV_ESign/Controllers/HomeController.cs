﻿using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using MMCV_Common;
using MMCV_BLL.Document;
using MMCV_Model.Document;
using MMCV_Model.User;
using MMCV_BLL.DocumentSign;
using MMCV_BLL.User;
using System.Configuration;
using MMCV_Common.Helper;
using System.Threading.Tasks;
using MMCV_Model.DocumentSign;
using System.Collections.Generic;

namespace MMCV_ESign.Controllers
{
    public class HomeController : Controller
    {
        private string CDN_Source_File = ConfigurationManager.AppSettings["CDN_Source_File"];
        private string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private UserBO currentUser = UserBO.Current.CurrentUser();

        public ActionResult PdfPage(int docId = 0)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();
                UserBLL userBLL = new UserBLL();

                // Get document info
                var doc = docBLL.GetDocumentById(docId);

                // Get document sign info
                var docSign = docSignBLL.GetDocumentSignByDocumentID(docId).OrderBy(x => x.SignIndex).ToList();
                doc.DocumentSigns = docSign;

                // First sign index
                var firstSignIndex = docSign.FirstOrDefault().SignIndex;

                // Check authorize access and sign document
                var isAuthor = doc.Issuer == currentUser.Email ? true : false;
                //var isSignerIncludeCurrentUser = docSign.Where(x => x.Email == currentUser.Email).FirstOrDefault();
                string[] stringSeparators = new string[] { ",", ";" };
                var isSignerIncludeCurrentUser = docSign.Where(x => x.Email.Split(stringSeparators, StringSplitOptions.None).Any(y => y.Trim() == currentUser.Email)).FirstOrDefault();
                var isSigner = isSignerIncludeCurrentUser == null ? false : true;
                if (!isAuthor && !isSigner)
                {
                    return Json(new { rs = false, msg = "You do not have right to access this document" }, JsonRequestBehavior.AllowGet);
                }

                // Check signing flow
                if (!isAuthor)
                {
                    var currentUserIndexSign = isSignerIncludeCurrentUser.SignIndex;
                    if (currentUserIndexSign > firstSignIndex)
                    {
                        var preSigner = docSign.Where(x => x.SignIndex == isSignerIncludeCurrentUser.SignIndex - 1).Where(x => x.Status == (int)EnumDocumentSign.Signed);
                        if (preSigner.Count() <= 0)
                        {
                            return Json(new { rs = false, msg = "The previous signer has not signed or declined" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                // Get user signature
                var signatures = userBLL.GetUserSignatures(currentUser.UserID);
                var userSig = new UserSignatureBO();
                if (signatures.Count > 0)
                {
                    var defaultSig = signatures.Where(x => x.IsDefault == 1).FirstOrDefault();
                    if (defaultSig != null)
                    {
                        userSig = defaultSig;
                    }
                    else
                    {
                        userSig = signatures[0];
                    }
                }

                // Convert file stored in CDN to byte array
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
                string base64String = Convert.ToBase64String(returnFile, 0, returnFile.Length);
                doc.Base64File = base64String;

                var returnData = new Tuple<UserBO, DocumentBO, UserSignatureBO>(currentUser, doc, userSig);

                return View(returnData);
            }
            catch (Exception ex)
            {
                return View("Error", new { Message = ex.Message });
            }
        }

        public ActionResult UpdateDocumentAfterSigning(DocumentBO doc)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();

                var docSign = docSignBLL.GetDocumentSignByDocumentID(doc.DocumentID);
                doc.DocumentSigns = docSign;

                var currentDocSign = docSign.Where(x => (x.Email == currentUser.Email && x.DocumentID == doc.DocumentID)).FirstOrDefault();
                if (currentDocSign != null)
                {
                    // update status of document sign
                    currentDocSign.UserSignatureID = currentUser.DefaultSignature.UserSignatureID;
                    currentDocSign.UserEmpID = currentUser.EmployeeID;
                    currentDocSign.Status = (int)EnumDocumentSign.Signed;
                    docSignBLL.UpdateStatusDocumentSign(currentDocSign);

                    if (currentDocSign.SignIndex + 1 >= docSign.Count())
                    {
                        // update status of document to complete
                        doc.Status = (int)EnumDocumentStatus.Completed;
                        docBLL.UpdateStatusDocument(doc);

                        // send mail to the issuer that document has completed signing flow
                        var body = $"Dear {doc.Issuer}," +
                                   $"<br>" +
                                   $"Your document has been completed. Reference code: {currentDocSign.DocumentReferenceCode}";
                        var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", "tuyen.nguyenvan@mmcv.mektec.com", "", body);
                        //var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", doc.Issuer, "", body);
                    }
                    else
                    {
                        // send mail to the next signer
                        var nextSignerEmail = docSign.Where(x => x.SignIndex == (currentDocSign.SignIndex + 1)).FirstOrDefault();
                        var body = $"Dear {nextSignerEmail.Email}," +
                                    $"<br>" +
                                    $"You have a document need to sign. Reference code: {doc.ReferenceCode}" + 
                                    $"<br>Please access <a href='{baseUrl}/Home/PdfPage?docId={doc.DocumentID}&email={nextSignerEmail.Email}&signIndex={nextSignerEmail.SignIndex}'>this link</a> to sign document";
                        var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", "tuyen.nguyenvan@mmcv.mektec.com", "", body);
                        //var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", nextSignerEmail.Email, "", body);
                    }
                }

                return Json(new { rs = true, msg = "Update document successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Home");
                return Json(new { rs = false, msg = "Error UpdateDocumentAfterSigning" });
            }
        }

        public ActionResult SaveFileAndSignature()
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

                            if (fileContent.FileName.Contains(".pdf"))
                            {
                                //new Thread(() =>
                                //{
                                //    Thread.CurrentThread.IsBackground = true;
                                //    var inputFilePath = path + fileContent.FileName;
                                //    var outputFilePath = path + "_removed_" + fileContent.FileName;
                                //    PdfHelper.RemoveXObject(inputFilePath, outputFilePath);
                                //}).Start();

                                var task = Task.Run(() =>
                                {
                                    var inputFilePath = path + fileContent.FileName;
                                    var outputFilePath = path + "_removed_" + fileContent.FileName;
                                    PdfHelper.RemoveXObject(inputFilePath, outputFilePath);
                                });

                                //var inputFilePath = path + fileContent.FileName;
                                //var outputFilePath = path + "_removed_" + fileContent.FileName;
                                //PdfHelper.RemoveXObject(inputFilePath, outputFilePath);
                            }

                            if (fileContent.FileName.Contains(".docx") || fileContent.FileName.Contains(".doc"))
                            {

                            }
                        }
                    }
                }

                return Json(new { rs = true, msg = "Save file and signature succssfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "DocumentManagement");
                return Json(new { rs = false, msg = "Error save file and signature document" });
            }
        }

        public ActionResult Decline(DocumentBO doc, string note)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();

                var docSign = docSignBLL.GetDocumentSignByDocumentID(doc.DocumentID);
                doc.DocumentSigns = docSign;

                var currentDocSign = docSign.Where(x => (x.Email == currentUser.Email && x.DocumentID == doc.DocumentID)).FirstOrDefault();
                // update status of document sign
                currentDocSign.UserSignatureID = currentUser.DefaultSignature.UserSignatureID;
                currentDocSign.UserEmpID = currentUser.EmployeeID;
                currentDocSign.Status = (int)EnumDocumentSign.Declined;
                currentDocSign.Note = note;
                docSignBLL.UpdateStatusDocumentSign(currentDocSign);

                // send mail to issuer
                var body = $"Dear {doc.Issuer}," +
                    $"<br>" +
                    $"{currentDocSign.Email} decline to sign your document. Reference code: {doc.ReferenceCode}";
                var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", "tuyen.nguyenvan@mmcv.mektec.com", "", body);
                //var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", doc.Issuer, "", body);

                return Json(new { rs = true, msg = "Decline successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Home");
                return Json(new { rs = false, msg = "Error Decline" });
            }
        }

        public ActionResult Cancel(DocumentBO doc, string note)
        {
            try
            {
                DocumentBLL docBLL = new DocumentBLL();
                DocumentSignBLL docSignBLL = new DocumentSignBLL();

                // Update status of document
                doc.Status = (int)EnumDocumentStatus.Cancel;
                var isUpdateStatusSuccess = docBLL.UpdateStatusDocument(doc);
                if (!isUpdateStatusSuccess)
                {
                    return Json(new { rs = false, msg = "Update document status to Cancel failed" });
                }

                // Send mail to all signer which signed and the next signer
                var documentSignList = docSignBLL.GetDocumentSignByDocumentID(doc.DocumentID);
                SendCancelMail(documentSignList);

                return Json(new { rs = true, msg = "Cancel document successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "Home");
                return Json(new { rs = false, msg = "Error cancel" });
            }
        }

        private void SendCancelMail(List<DocumentSignBO> listEmail)
        {
            var listSignedEmail = listEmail.Where(x => x.Status == (int)EnumDocumentSign.Signed).ToList();
            var nextSignerEmail = listEmail.Where(x => x.Status == (int)EnumDocumentSign.Initital).OrderBy(x => x.SignIndex).FirstOrDefault();

            if (nextSignerEmail != null)
            {
                listSignedEmail.Add(nextSignerEmail);
            }

            if (listSignedEmail != null && listSignedEmail.Count > 0)
            {
                listSignedEmail.ForEach((ele) =>
                {
                    var body = $"Dear {ele.Email}," +
                    "<br >" +
                    $"This email to announce that the document has been canceled. Reference code: {ele.DocumentReferenceCode}";
                    var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", "tuyen.nguyenvan@mmcv.mektec.com", "", body);
                    //var isSendMailSuccess = MailHelper.SendEmail("Document Sign", "system@mmcv.mektec.com", ele.Email, "", body);
                });
            }
        }
    }
}
