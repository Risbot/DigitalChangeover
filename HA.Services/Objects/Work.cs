using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky WorkTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Work
    {
        /// <summary>
        /// Určuje identifikátor objektu.
        /// </summary>
        [DataMember]
        public Int64 ID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataMember]
        public Int32 VehicleID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string VehicleNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataMember]
        public Int32 WorkTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string WorkTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataMember]
        public Int64 DateID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime DateContent
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Boolean IsNight
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required(AllowEmptyStrings=false)]
        [DataMember]
        public string FaultDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CauseDescription
        {
            get;
            set;
        }
    }
}
