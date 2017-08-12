using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;

namespace HA.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SecurityService : ISecurityService
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public Objects.User AddUser(Objects.User user)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            IRepository<Role> repositoryRole = new Repositor<Role>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(user); 
                PasswordGenerator generator = new PasswordGenerator(10, user.Name);
                user.Password = generator.Password; 
                User u = new User();
                u.UserName = String.IsNullOrWhiteSpace(user.Name) ? null : user.Name;    
                u.UserPassword = generator.Hash;
                u.UserWorkerID = user.WorkerID;
                u.UserDetachmentID = user.DetachmentID;
                u.UserDescription = String.IsNullOrWhiteSpace(user.Description) ? null : user.Description;
                if (user.Roles != null)
                    foreach (var item in user.Roles)
                    {
                        var r = repositoryRole.Single(c => c.RoleID == item.ID);
                        u.Roles.Add(r);
                    }
                unitOfWork.BeginTransaction();
                repository.Add(u);
                unitOfWork.CommitTransaction();
                user.ID = u.UserID;   
                return user;
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void DeleteUser(Int32 userID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {   
                unitOfWork.BeginTransaction();
                var u = repository.Single(c => c.UserID == userID);
                repository.Delete(u);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public string ResetPassword(Int32 userID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {           
                unitOfWork.BeginTransaction();
                var u = repository.Single(c => c.UserID == userID);
                PasswordGenerator generator = new PasswordGenerator(10,u.UserName);
                u.UserPassword = generator.Hash;
                repository.Update(u);
                unitOfWork.CommitTransaction();
                return generator.Password;
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void UpdateUser(Objects.User user)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            IRepository<Role> repositoryRole = new Repositor<Role>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(user);  
                unitOfWork.BeginTransaction();
                var u = repository.Single(c => c.UserID == user.ID);
                u.UserDetachmentID = user.DetachmentID;
                u.UserDescription = String.IsNullOrWhiteSpace(user.Description) ? null : user.Description;
                u.UserWorkerID = user.WorkerID;
                if (user.Roles != null)
                {
                    var oldIds = u.Roles.Select(c => c.RoleID);
                    var addRoles = user.Roles.Where(c => !oldIds.Contains(c.ID)).ToList();
                    var newIds = user.Roles.Select(c => c.ID);
                    var removeRoles = u.Roles.Where(c => !newIds.Contains(c.RoleID)).ToList();
                    foreach (var item in removeRoles)
                    {
                        var r = repositoryRole.Single(c => c.RoleID == item.RoleID);
                        u.Roles.Remove(r);
                    }
                    foreach (var item in addRoles)
                    {
                        var r = repositoryRole.Single(c => c.RoleID == item.ID);
                        u.Roles.Add(r);
                    }
                }
                repository.Update(u);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public List<Objects.Role> GetRoles()
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Role> repository = new Repositor<Role>(unitOfWork);
            try
            {          
                return repository.GetAll().Select(c => new Objects.Role() 
                {
                    ID = c.RoleID,
                    Name = c.RoleName
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public List<Objects.User> GetUsers()
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {        
                return repository.GetAll().Select(c => new Objects.User() 
                {
                   ID = c.UserID,
                   Name = c.UserName,
                   WorkerID = c.UserWorkerID,
                   Description = c.UserDescription,
                   DetachmentID = c.UserDetachmentID, 
                   Roles = c.Roles.Select(r => new Objects.Role()
                   {
                       ID = r.RoleID,
                       Name = r.RoleName
                   })
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public Objects.User GetUserInfo(string userName)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {
                var u = repository.Single(c => c.UserName == userName );
                var user = new Objects.User();
                user.ID = u.UserID;
                user.Name = u.UserName;
                user.WorkerID = u.UserWorkerID;
                user.Description = u.UserDescription;
                user.DetachmentID = u.UserDetachmentID;
                user.Roles = u.Roles.Select(c => new Objects.Role()
                {
                    ID = c.RoleID,
                    Name = c.RoleName
                });   
                return user;
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public bool ChangePassword(Objects.User user)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<User> repository = new Repositor<User>(unitOfWork);
            try
            {
                unitOfWork.BeginTransaction();
                var u = repository.Single(c => c.UserID == user.ID);
                u.UserPassword = String.IsNullOrWhiteSpace(user.Password) ? null : user.Password;
                repository.Update(u);
                unitOfWork.CommitTransaction();
                return true;
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }
    }
}
