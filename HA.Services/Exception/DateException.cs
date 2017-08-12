using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.Services
{
    /// <summary>
    /// Třída reprezentující výjimku směny.
    /// </summary>
    public class DateException : Exception
    {
        public DateException(string message) : base(message)
        {

        }
    }
}
