using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky DetachmentTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Detachment
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
        /// Určuje jméno oddělení.
        /// </summary>
        [StringLength(50)]
        [Required(AllowEmptyStrings=false)]
        [DataMember]
        public string Name
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
