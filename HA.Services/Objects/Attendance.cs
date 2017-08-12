using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services.Objects
{
    /// <summary>
    /// Třída reprezentuje minimalizovanou verzi tabulky AttendanceTable z databáze.
    /// Je určená pro přenos potřebných dat mezi klientem a serverem.
    /// </summary>
    [DataContract]
    public class Attendance
    {
        /// <summary>
        /// Určuje identifikátor objektu.
        /// </summary>
        [Required]
        [DataMember]
        public Int64 DateID
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje identifikátor zaměstnance který je svázán s touto docházkou.
        /// </summary>
        [Required]
        [DataMember]
        public Int32 WorkerID
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje křestní jméno přidruženého zaměstnance.
        /// </summary>
        [DataMember]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje příjmení přidruženého zaměstnance.
        /// </summary>
        [DataMember]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje sapovské číslo přidruženého zaměstnance.
        /// </summary>
        [DataMember]
        public string SapNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje stav zaměstnance v docházce.
        /// </summary>
        [DataMember]
        public string WorkerState
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje identifikátor stavu zaměstnance.
        /// </summary>
        [Required]
        [DataMember]
        public Int32 WorkerStateID
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje v jakém turnusu je zaměstnanec v této docházce.
        /// </summary>
        [DataMember]
        public string WorkerTour
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje identifikátor turnusu zaměstnance. 
        /// </summary>
        [Required]
        [DataMember]
        public Int32 WorkerTourID
        {
            get;
            set;
        }

        /// <summary>
        /// Určuje popis objektu.
        /// </summary>
        [DataMember]
        public String Description
        {
            get;
            set;
        }
    }
}
