//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HA.Services
{
    using System;
    using System.Collections.Generic;
    
    public partial class Work
    {
        public long WorkID { get; set; }
        public int WorkVehicleID { get; set; }
        public int WorkWorkTypeID { get; set; }
        public long WorkDateID { get; set; }
        public string WorkFaultDescription { get; set; }
        public string WorkCauseDescription { get; set; }
    
        public virtual Date Date { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual WorkType WorkType { get; set; }
    }
}
