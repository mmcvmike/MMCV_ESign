//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MMCV_Model.DB68
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public Nullable<int> DocumentTypeID { get; set; }
        public string Issuer { get; set; }
        public string IssuerEmpId { get; set; }
        public string Title { get; set; }
        public Nullable<int> Status { get; set; }
        public string Link { get; set; }
        public string ReferenceCode { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> Active { get; set; }
        public string EmailCC { get; set; }
    }
}
