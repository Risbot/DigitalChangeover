using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.Infrastructure;
using System.Windows.Input;
using HA.MVVMClient.SecurityService;
using HA.MVVMClient.DataService;
using System.ServiceModel;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        #region Variables

        private string oldPassword;
        private string newPassword;
        private string confirmPassword;
        private string status;
        private string detachment;
        private bool writeAutorization;
        private bool masterAutorization;
        private bool adminAutorization;
        private string userName;
        private string tmpPass;
        private SecurityServiceClient securityClient;
        private DataServiceClient dataClient;
        private AccountViewModelValidator validator;
        private bool busy;

        #endregion

        #region Constructors

        public AccountViewModel(DataServiceClient dataClient, INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            validator = new AccountViewModelValidator();
            Navigator = navigator;
            UserName = LoginInit.user.Name;
            WriteAutorization = LoginInit.user.Roles.Any(c => c.Name == "Write");
            AdminAutorization = LoginInit.user.Roles.Any(c => c.Name == "Admin");
            MasterAutorization = LoginInit.user.Roles.Any(c => c.Name == "Master");
            InitCommands();
            this.dataClient.FindDetachmentCompleted += FindDetachmentCompleted;
            this.dataClient.FindDetachmentAsync(LoginInit.user.DetachmentID);
            PropertyChanged += (s, e) =>
            {
                (ChangeCommand as Command).OnCanExecuteChanged();
            };
        }

        #endregion

        #region Functions

        public override ValidationResult Validator(string propertyName)
        {
            return validator.Validate(this, propertyName);
        }

        #endregion
      
        #region Events

        void FindDetachmentCompleted(object sender, FindDetachmentCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Detachment = e.Result.Name;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindDetachmentCompleted -= FindDetachmentCompleted;
            Busy = false;
        }

        void ChangePasswordCompleted(object sender, ChangePasswordCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                tmpPass = null;
                Status = "Heslo bylo úspěšně změněno!";
                NewPassword = null;
                OldPassword = null;
                ConfirmPassword = null;
            }
            else
            {
                LoginInit.user.Password = tmpPass;
                tmpPass = null;
                ErrorProvider.ShowError(e.Error, Navigator);
            }
               
           
                
            securityClient.ChangePasswordCompleted -= ChangePasswordCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            ChangeCommand = new Command(OnExecute, OnCanExecute);
        }

        public ICommand ChangeCommand
        {
            get;
            private set;
        }

        private void OnExecute()
        {
            Password password = new Password(UserName, OldPassword);
            if (password.Hash != LoginInit.user.Password || NewPassword != ConfirmPassword)
                Status = "Heslo se nepodařilo změnit!";
            else
            {
                Busy = true;
                securityClient = ContainerProvider.GetInstance.Resolve<SecurityServiceClient>();
                securityClient.ChangePasswordCompleted += ChangePasswordCompleted;
                tmpPass = LoginInit.user.Password;
                password = new Password(UserName, NewPassword);
                LoginInit.user.Password = password.Hash;
                securityClient.ChangePasswordAsync(LoginInit.user);
            }
        }

        private bool OnCanExecute()
        {
            return IsValid;
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
                if (busy != value)
                {
                    busy = value;
                    OnPropertyChanged(() => Busy);
                }
            }
        }

        public bool WriteAutorization
        {
            get { return writeAutorization; }  
            set
            {
                if (writeAutorization != value)
                {
                    writeAutorization = value;
                    OnPropertyChanged(() => WriteAutorization);
                }
            }
        }

        public bool MasterAutorization
        {
            get { return masterAutorization; } 
            set
            {
                if (masterAutorization != value)
                {
                    masterAutorization = value;
                    OnPropertyChanged(() => MasterAutorization);
                }
            }
        }

        public bool AdminAutorization
        {
            get { return adminAutorization; } 
            set
            {
                if (adminAutorization != value)
                {
                    adminAutorization = value;
                    OnPropertyChanged(() => AdminAutorization);
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
                    OnPropertyChanged(() => UserName);
                }
            }
        }

        public string Detachment
        {
            get { return detachment; }
            set
            {
                if (detachment != value)
                {
                    detachment = value;
                    OnPropertyChanged(() => Detachment);
                }
            }
        }

        public string OldPassword
        {
            get
            {
                return oldPassword;
            }
            set
            {
                if (value != oldPassword)
                {
                    oldPassword = value;
                    
                    OnPropertyChanged(() => OldPassword);
                }
            }
        }

        public string NewPassword
        {
            get
            {
                return newPassword;
            }
            set
            {
                if (value != newPassword)
                {
                    newPassword = value;
                    OnPropertyChanged(() => ConfirmPassword);
                    OnPropertyChanged(() => NewPassword);
                }
            }
        }

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                if (confirmPassword != value)
                {
                    confirmPassword = value;
                    OnPropertyChanged(() => NewPassword);
                    OnPropertyChanged(() => ConfirmPassword);
                }
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(() => Status);
                }
            }
        }

        #endregion
    }
}
