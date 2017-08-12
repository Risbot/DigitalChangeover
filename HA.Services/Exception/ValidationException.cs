using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HA.Services
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }

        [DataMember]
        public List<string> MemberName
        {
            get;
            set;
        }
    
    }

    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationResult> result)
            : base(GetFirstErrorMessage(result))
        {
            var r = new List<Result>();
            foreach (var item in result)
            {
                r.Add(new Result()
                {
                    ErrorMessage = item.ErrorMessage,
                    MemberName = item.MemberNames.ToList()
                });
            }
            Results = r;
        }

        public List<Result> Results
        {
            private set;
            get;
        }

        private static string GetFirstErrorMessage(IEnumerable<ValidationResult> result)
        {
            return result.First().ErrorMessage;
        }
    }  
}
