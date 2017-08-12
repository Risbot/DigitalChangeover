using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.ServiceModel;
using NLog;

namespace HA.Services
{
    public class AuthorizationPolicy : IAuthorizationPolicy
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string id = Guid.NewGuid().ToString();
        private string[] roles;
        private IList<IIdentity> identities;

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {         
            object obj;
            if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
            {
                logger.Log(LogLevel.Warn, "Not authorizated");
                return false;
            }
            identities = obj as IList<IIdentity>;
            if (obj == null || identities.Count <= 0)
            {
                logger.Log(LogLevel.Warn, "Not authorizated");
                return false;
            }
            EnsureRoles();
            evaluationContext.Properties["Principal"] = new GenericPrincipal(identities[0],roles);    
            return true;
        }


        protected virtual void EnsureRoles()
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {   
                roles = repository.Single(c => c.UserName == identities[0].Name).Roles.Select(c=> c.RoleName).ToArray();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }


        public System.IdentityModel.Claims.ClaimSet Issuer
        {
            get 
            {
                return null;
            }
        }

        public string Id
        {
            get 
            {
                return id;
            }
        }
    }
}
