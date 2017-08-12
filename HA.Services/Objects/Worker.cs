using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky WorkerTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Worker
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
        [DataMember]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string SapNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ServicePhone
        {
            get;
            set;
        }
    }
}
