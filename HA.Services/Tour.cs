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
    
    public partial class Tour
    {
        public Tour()
        {
            this.Attendances = new HashSet<Attendance>();
            this.Workers = new HashSet<Worker>();
        }
    
        public int TourID { get; set; }
        public System.TimeSpan TourStartTime { get; set; }
        public System.TimeSpan TourEndTime { get; set; }
        public string TourDescription { get; set; }
        public int TourDetachmentID { get; set; }
    
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual Detachment Detachment { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
    }
}