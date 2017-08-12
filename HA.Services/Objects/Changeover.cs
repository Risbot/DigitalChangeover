using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky ChangeoverTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Changeover
    {
        /// <summary>
        /// Určuje identifikátor objektu.
        /// </summary>
        [DataMember]
        public Int32 ID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataMember]
        public Int32 DetachmentID
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
        /// Určuje popis objektu.
        /// </summary>
        [Required(AllowEmptyStrings=false)]
        [DataMember]
        public string Description
        {
            get;
            set;
        }
    }
}
