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
    
    public partial class Detachment
    {
        public Detachment()
        {
            this.Dates = new HashSet<Date>();
            this.Changeovers = new HashSet<Changeover>();
            this.Tours = new HashSet<Tour>();
            this.Users = new HashSet<User>();
            this.Vehicles = new HashSet<Vehicle>();
            this.Workers = new HashSet<Worker>();
            this.WorkerStates = new HashSet<WorkerState>();
            this.WorkTypes = new HashSet<WorkType>();
        }
    
        public int DetachmentID { get; set; }
        public string DetachmentName { get; set; }
        public string DetachmentDescription { get; set; }
    
        public virtual ICollection<Date> Dates { get; set; }
        public virtual ICollection<Changeover> Changeovers { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<WorkerState> WorkerStates { get; set; }
        public virtual ICollection<WorkType> WorkTypes { get; set; }
    }
}
