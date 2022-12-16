using System;

namespace MMCV_Model.DocumentSign
{
    public class DocumentSignBO
    {
        public int DocumentSignID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentReferenceCode { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int SignIndex { get; set; }
        public DateTime SignDate { get; set; }
        public int UserSignatureID { get; set; }
        public string UserEmpID { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }

        // custom properties
        public string Sender { get; set; }
        public string Title { get; set; }
    }
}
