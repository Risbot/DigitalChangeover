using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky UserTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class User
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
        [DataMember]
        public Int32? WorkerID
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
        [StringLength(30)]
        [Required(AllowEmptyStrings = false)]
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Password
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

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IEnumerable<Objects.Role> Roles
        {
            get;
            set;
        }
    }
}
