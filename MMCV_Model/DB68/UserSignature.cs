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
    
    public partial class UserSignature
    {
        public int UserSignatureID { get; set; }
        public int UserID { get; set; }
        public string Base64Signature { get; set; }
        public Nullable<int> IsDefault { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> Active { get; set; }
    }
}
