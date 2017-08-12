using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HA.Services
{
    [ServiceContract]
    interface ISecurityService
    {
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Objects.User AddUser(Objects.User user);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteUser(Int32 userID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        string ResetPassword(Int32 userID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateUser(Objects.User user);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Role> GetRoles();
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.User> GetUsers();
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Objects.User GetUserInfo(string userName);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        bool ChangePassword(Objects.User user);
    }
}
