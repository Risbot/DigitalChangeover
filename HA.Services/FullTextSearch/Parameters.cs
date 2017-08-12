using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HA.Services
{
    [DataContract]
    public class Parameters
    {
        [Required]
        [DataMember]
        public DateTime From
        {
            get;
            set;
        }

        [Required]
        [DataMember]
        public DateTime To
        {
            get;
            set;
        }

        [DataMember]
        public Int32 VehicleID
        {
            get;
            set;
        }

        [DataMember]
        public Int32 TypeID
        {
            get;
            set;
        }

        [DataMember]
        public Int32 DetachmentID
        {
            get;
            set;
        }

        [DataMember]
        public string SearchKey
        {
            get;
            set;
        }
    }
}
