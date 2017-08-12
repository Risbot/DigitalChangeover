using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky DateTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Date
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
        /// Určuje datum směny.
        /// </summary>
        [Required]
        [DataMember]
        public DateTime DateContent
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje zda se jedna o noční či denní směnu.
        /// </summary>
        [Required]
        [DataMember]
        public Boolean IsNight
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje zda je směna uzavřena nebo odemčena.
        /// </summary>
        [Required]
        [DataMember]
        public Boolean IsClosed
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
