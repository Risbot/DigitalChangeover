using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.Infrastructure;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using HA.MVVMClient.DataService;
using Microsoft.Practices.Unity;
using HA.MVVMClient.FullTextService;
using HA.MVVMClient.Views;
using System.Collections.ObjectModel;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
using HA.MVVMClient.SecurityService;
using System.Threading.Tasks;

namespace HA.MVVMClient.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {     
        #region Variables

        private string userName;
        private string errorMessage;
        private bool busy;

        #endregion

        #region Constructors

        public LoginViewModel()
        {
            Busy = false;
            LoginInit.user = new User();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            InitCommands();
        }

        #endregion

        #region Functions

        private Task<string> Login(string userNeme, string password)
        {
            return Task.Factory.StartNew(() =>
            {
                if (userName == null || password == null)
                {
                    return "Nesprávně zadané jméno nebo heslo!";
                }
                var pass = new Password(UserName, password);
                try
                {
                    LoginInit.user.Name = UserName;
                    LoginInit.user.Password = pass.Hash;
                    LoginInit.user = ContainerProvider.GetInstance.Resolve<SecurityServiceClient>().GetUserInfo(UserName);
                    LoginInit.user.Password = pass.Hash;
                }
                catch (MessageSecurityException)
                {
                    return "Nesprávně zadané jméno nebo heslo!";
                }
                catch (Exception)
                {
                    return "Nepodařilo se přihlásit!";
                }
                return null;
            });
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            LoginCommand = new Command(c => OnLoginExecute(c));
        }

        public ICommand LoginCommand
        {
            get;
            private set;
        }

        private async void OnLoginExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            ErrorMessage = "Probíhá autentizace...";
            Busy = true;
            var res = await Login(userName, passwordBox.Password);
            Busy = false;
            if (res != null)
            {
                passwordBox.Password = null;
                UserName = null;
                ErrorMessage = res;
            }
            else
            {
                var n = ContainerProvider.GetInstance.Resolve<INavigator>();
                n.Startup<MainWindowView, MainWindowViewModel>(ViewMode.Window);
                OnCloseView();
            }
        }

        #endregion

        #region Properties

        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                if (busy != !value)
                {
                    busy = !value;
                    OnPropertyChanged(() => Busy);
                }
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    ErrorMessage = null;
                    OnPropertyChanged(() => UserName);
                }
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged(() => ErrorMessage);
                }
            }
        }

        #endregion  
    }
}
