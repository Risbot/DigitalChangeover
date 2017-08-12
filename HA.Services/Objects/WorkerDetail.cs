using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje rozšířenou verzi tabulky WorkerTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class WorkerDetail
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
        [StringLength(20)]
        [Required(AllowEmptyStrings = false)]
        [DataMember]
        public string FirstName
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
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(20)]
        [Required(AllowEmptyStrings = false)]
        [DataMember]
        public string ServiceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(20)]
        [Required(AllowEmptyStrings = false)]
        [DataMember]
        public string SapNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        [DataMember]
        public string ServiceEmail
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        [DataMember]
        public string PersonalEmail
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(20)]
        [DataMember]
        public string ServicePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(20)]
        [DataMember]
        public string PersonalPhone
        {
            get;
            set;
        }

        /// <summary>
        /// 
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
        public byte[] Photo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [EnumCount(1)]
        [DataMember]   
        public IEnumerable<Objects.Tour> Tours
        {
            get;
            set;
        }
    }
}
