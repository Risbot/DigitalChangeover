using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace UserCreator
{
   
    public class User
    {
        public Int32 DetachmentID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

    }
}
