using MMCV_Model.DocumentSign;
using System;
using System.Collections.Generic;

namespace MMCV_Model.Document
{
    public class DocumentBO
    {
        // database properties
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeID { get; set; }
        public string Issuer { get; set; }
        public string IssuerEmpId { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Link { get; set; }
        public string ReferenceCode { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Active { get; set; }
        public string EmailCC { get; set; }
        // custom properties
        public List<DocumentSignBO> DocumentSigns { get; set; }
        public string Base64File { get; set; }
        public int SignerStatus { get; set; }
    }

    public class DashboardCountBO
    {
        public int MeSign { get; set; }
        public int Sent { get; set; }
        public int Completed { get; set; }
    }
}
