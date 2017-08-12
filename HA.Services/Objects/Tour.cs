using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky TourTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Tour
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
        /// Určuje počáteční čas turnusu.
        /// </summary>
        [Required]
        [DataMember]
        public TimeSpan StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje konečný čas turnusu.
        /// </summary>
        [Required]
        [DataMember]
        public TimeSpan EndTime
        {
            get;
            set;
        }


        [Required]
        [DataMember]
        public Int32 DetachmentID
        {
            get;
            set;
        }


        /// <summary>
        /// Určuje popis objektu.
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
    }
}
