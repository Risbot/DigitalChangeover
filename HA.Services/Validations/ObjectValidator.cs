using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HA.Services
{
    internal class ObjectValidator
    {
        public static void IsValid(object obj)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(obj, null, null);
            if (!Validator.TryValidateObject(obj, context, result, true))
            {
                throw new ValidationException(result);
            }
        }
    }
}

