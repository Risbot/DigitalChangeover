using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;


namespace HA.Services
{
    public class EnumCountAttribute : ValidationAttribute
    {
        public int MinCount 
        {
            get;
            set; 
        }

        public EnumCountAttribute(int minCount)
        {
            MinCount = minCount;
        }

        public override bool IsValid(object value)
        {
            bool isValid = true;
            int count = (value as ICollection).Count;
            if (count < MinCount)
                isValid = false;
            return isValid;
        }

    }
}
