using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using NLog;

namespace HA.Services
{
    public class AuthenticationValidator : UserNamePasswordValidator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void Validate(string userName, string password)
        {          

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                logger.Log(LogLevel.Warn, "Not authentication");
                throw new SecurityTokenException("Nesprávně zadané heslo nebo jméno!");
            }
            try
            {
                var user = repository.Single(c => c.UserName == userName && c.UserPassword == password);
                if (user == null)
                {
                    logger.Log(LogLevel.Warn, "Not authentication");
                    throw new SecurityTokenException("Nesprávně zadané heslo nebo jméno!");
                }
                    
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }   
        }
    }
}
