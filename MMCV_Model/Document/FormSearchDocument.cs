namespace MMCV_Model.Document
{
    public class FormSearchDocument
    {
        public int DocumentID { get; set; }
        public int Status { get; set; }
        public int DocumentType { get; set; }
        public string Issuer { get; set; }
        public string Signer { get; set; }
        public string Title { get; set; }
        public string ReferenceCode { get; set; }
    }

    public class FormSearchDashboard
    {
        public string Email { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
}
