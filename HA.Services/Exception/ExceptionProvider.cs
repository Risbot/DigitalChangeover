using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security;
using System.Text;

namespace HA.Services
{
    /// <summary>
    /// Třída tvořící z výjimek fault contract.
    /// </summary>
    public static class ExceptionProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Metoda která vytvoří fault contract.
        /// </summary>
        /// <param name="e">Výjimka dle které se tvoří fault contract.</param>
        /// <returns></returns>
        public static WcfException CreateFaultContract(Exception e)
        {
            if (e is ValidationException)
            {

                logger.Log(LogLevel.Error, "Validation Exception : ", e);
                ValidationException ve = e as ValidationException;
                return new WcfException()
                {
                    Status = ErrorStatus.ValidationError,
                    Message = "Validační chyba, viz. Result!",
                    Result = ve.Results
                };
            }
            if (e is DbUpdateException)
            {
                logger.Log(LogLevel.Error, "Db Exception : ", e);
                var ex = new WcfException()                
                {
                    Status = ErrorStatus.DatabaseError,
                    Message = "Nastál neznámí problém s databázi!",
                    Result = null
                };
                if (e.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    ex.Status = ErrorStatus.DatabaseInfo;
                    ex.Message = "Tento objekt jíž existuje!";     
                }
                if (e.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ex.Status = ErrorStatus.DatabaseInfo;
                    ex.Message = "Tento objekt nelze smazat jelikož se na něj odkazujete v jiném záznamu!";
                }
                if (e.InnerException.InnerException.Message.Contains("INSERT"))
                {
                    ex.Status = ErrorStatus.DatabaseInfo;
                    ex.Message = "Objekt neexistuje!";
                }
                return ex;
            }
            if (e is EntityException)
            {
                logger.Log(LogLevel.Error, "Entity Exception : ", e);
                return new WcfException()
                {
                    Status = ErrorStatus.DatabaseError,
                    Message = "Nelze se přípojit k databázi, zkuste požadavek zopakovat později!",
                    Result = null
                };
            }
            if (e is InvalidOperationException)
            {
                logger.Log(LogLevel.Error, "Invalid Operation Exception : ", e);
                return new WcfException()
                {
                    Status = ErrorStatus.DatabaseInfo,
                    Message = "Objekt nenalezen!",
                    Result = null
                };
            }
            if (e is FormatException)
            {
                logger.Log(LogLevel.Error, "Format Exception : ", e);
                return new WcfException()
                {
                    Status = ErrorStatus.ValidationError,
                    Message = (e as FormatException).Message,
                    Result = null
                };
            }
            if (e is DateException)
            {
                logger.Log(LogLevel.Error, "Date Exception : ", e);
                return new WcfException()
                {
                    Status = ErrorStatus.DateError,
                    Message = (e as DateException).Message,
                    Result = null
                };  
            }

            logger.Log(LogLevel.Fatal, "Unknowen Exceptionn : ", e);
            return new WcfException()
            {
                Status = ErrorStatus.UnknowenError,
                Message = "Neočekávaná chyba!",
                Result = null
            }; 
        }
    }
}
