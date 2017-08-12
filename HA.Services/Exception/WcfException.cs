using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HA.Services
{
    /// <summary>
    /// Výčet chybových kódu.
    /// </summary>
    public enum ErrorStatus 
    {
        /// <summary>
        /// Chybový kód validace.
        /// </summary>
        ValidationError = 100,
        /// <summary>
        /// Chybový kód určující problém s databázi.
        /// </summary>
        DatabaseError = 300,
        /// <summary>
        /// Chybový kód určující problém konzistenci dat v databázi.
        /// </summary>
        DatabaseInfo = 500,
        /// <summary>
        /// Chybový kód neznámé chyby.
        /// </summary>
        UnknowenError = 200,
        /// <summary>
        /// Chybový kód určující problém se směnou.
        /// </summary>
        DateError = 600,
        /// <summary>
        /// Chybový kód určující bezpečnostní chybu.
        /// </summary>
        SecurityError = 400
    }

    /// <summary>
    /// Třída reprezentuje fault contract.
    /// </summary>
    [DataContract]
    public class WcfException
    {
        /// <summary>
        /// Parametr určuje chybový kód.
        /// </summary>
        [DataMember]
        public ErrorStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Parametr určuje zprávu chyby.
        /// </summary>
        [DataMember]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Parametr v případě validačni chyby obsahuje výsledek validace.
        /// </summary>
        [DataMember]
        public List<Result> Result
        {
            get;
            set;
        }
    }
}
