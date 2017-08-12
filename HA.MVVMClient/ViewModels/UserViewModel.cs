using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.SecurityService;
using System.Collections.ObjectModel;
using HA.MVVMClient.Infrastructure;
using System.Windows.Input;
using HA.MVVMClient.DataService;
using FluentValidation.Results;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation;
using Microsoft.Practices.Unity;


namespace HA.MVVMClient.ViewModels
{
    public class UserViewModel : BaseViewModel
    {   
        #region Variables

        private SecurityServiceClient securityClient;
        private DataServiceClient dataClient;
        private ObservableCollection<Role> roles;
        private ObservableCollection<User> users;
        private ObservableCollection<Role> userInRole;
        private ObservableCollection<Detachment> detachments;
        private bool enabled;
        private Role selectedUserInRole;
        private User selectedUser;
        private Role selectedRole;
        private Detachment selectedDetachment;
        private string userName;
        private string password;
        private Worker selectedWorker;
        private ObservableCollection<Worker> workers;
        private User oldObject;
        private UserViewModelValidator validator;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public UserViewModel(SecurityServiceClient securityClient, DataServiceClient dataClient, UserViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            busyCount = 2;
            this.securityClient = securityClient;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.securityClient.GetRolesCompleted += GetRolesCompleted;
            this.securityClient.GetRolesAsync();
            this.securityClient.GetUsersCompleted += GetUsersCompleted;
            this.securityClient.GetUsersAsync();
            this.dataClient.GetDetachmentsCompleted += GetDetachmentsCompleted;
            this.dataClient.GetDetachmentsAsync();
            PropertyChanged += (s, e) =>
            {
                (SaveCommand as Command).OnCanExecuteChanged();
                (NewCommand as Command).OnCanExecuteChanged();
                (DeleteCommand as Command).OnCanExecuteChanged();
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

        void GetUsersCompleted(object sender, GetUsersCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                e.Result.Remove(e.Result.FirstOrDefault(c => c.ID == LoginInit.user.ID));
                Users = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            securityClient.GetUsersCompleted -= GetUsersCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void GetRolesCompleted(object sender, GetRolesCompletedEventArgs e)
        {
            if (e.Error == null)
                Roles = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            securityClient.GetRolesCompleted -= GetRolesCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void GetDetachmentsCompleted(object sender, GetDetachmentsCompletedEventArgs e)
        {
            if (e.Error == null)
                Detachments = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.GetDetachmentsCompleted -= GetDetachmentsCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkersCompleted(object sender, FindWorkersCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Workers = e.Result;
                if(SelectedUser != null)
                    SelectedWorker = (selectedUser.WorkerID != null ? Workers.FirstOrDefault(c => c.ID == selectedUser.WorkerID) : null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkersCompleted -= FindWorkersCompleted;
            Busy = false;
        }

        void DeleteUserCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {  
                Users.Remove(SelectedUser);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            securityClient.DeleteUserCompleted -= DeleteUserCompleted;
            Busy = false;
        }

        void UpdateUserCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                oldObject = null;
                (SaveCommand as Command).OnCanExecuteChanged();
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedUser = oldObject;
            }
            securityClient.UpdateUserCompleted -= UpdateUserCompleted;
            Busy = false;
        }

        void AddUserCompleted(object sender, AddUserCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SelectedUser = e.Result;
                Password = e.Result.Password;
                Users.Add(SelectedUser);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            securityClient.AddUserCompleted -= AddUserCompleted;
            Busy = false;
        }

        void ResetPasswordCompleted(object sender, ResetPasswordCompletedEventArgs e)
        {
            if (e.Error == null)
                Password = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            securityClient.ResetPasswordCompleted -= ResetPasswordCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            SaveCommand = new Command(OnSaveExecute, OnSaveCanExecute);
            NewCommand = new Command(OnNewExecute, OnNewCanExecute);
            DeleteCommand = new Command(OnDeleteExecute, OnDeleteCanExecute);
            InsertRoleCommand = new Command(OnInsertRoleExecute, OnInsertRoleCanExecute);
            RemoveRoleCommand = new Command(OnRemoveRoleExecute, OnRemoveRoleCanExecute);
            ResetPasswordCommand = new Command(OnResetPasswordExecute, OnResetPasswordCanExecute); 
        }

        public ICommand SaveCommand
        {
            get;
            private set;
        }

        public ICommand NewCommand
        {
            get;
            private set;
        }

        public ICommand DeleteCommand
        {
            get;
            private set;
        }

        public ICommand InsertRoleCommand
        {
            get;
            private set;
        }

        public ICommand RemoveRoleCommand
        {
            get;
            private set;
        }

        public ICommand ResetPasswordCommand
        {
            get;
            private set;
        }

        private void OnSaveExecute()
        {
            Busy = true;
            securityClient = ContainerProvider.GetInstance.Resolve<SecurityServiceClient>();
            if (SelectedUser != null)
            {
                oldObject = SelectedUser;
                SelectedUser.Password = null;
                SelectedUser.Roles = (UserInRole != null ? Functionality.CloneCollection(UserInRole) : null);
                SelectedUser.DetachmentID = SelectedDetachment.ID;
                SelectedUser.WorkerID = (SelectedWorker != null ? SelectedWorker.ID : (int?)null);
                securityClient.UpdateUserCompleted += UpdateUserCompleted;
                securityClient.UpdateUserAsync(SelectedUser);
            }
            else
            {
                User user = new User()
                {
                    Name = UserName, 
                    Roles = (UserInRole != null ? UserInRole : null),
                    WorkerID = (SelectedWorker != null ? SelectedWorker.ID : (int?)null),
                    DetachmentID = SelectedDetachment.ID
                };
                securityClient.AddUserCompleted += AddUserCompleted;
                securityClient.AddUserAsync(user);
            }
        }

        private bool OnSaveCanExecute()
        {
            return IsValid &&         
                (SelectedUser == null ? true : SelectedUser.DetachmentID != (SelectedDetachment == null ? 0 : SelectedDetachment.ID) ||          
                SelectedUser == null ? true : SelectedUser.WorkerID != (SelectedWorker == null ? (int?)null : SelectedWorker.ID) ||          
                SelectedUser == null ? true : !Functionality.CompareCollection(SelectedUser.Roles, UserInRole, new CompareRole()));
        }

        private void OnNewExecute()
        {
            Enabled = true;
            UserInRole = null;
            UserName = null;
            Password = null;
            SelectedWorker = null;
            SelectedUserInRole = null;
            SelectedRole = null;
            SelectedUser = null;    
            SelectedDetachment = null;
        }

        private bool OnNewCanExecute()
        {
            return SelectedUser != null;
        }

        private void OnDeleteExecute()
        {
            Busy = true;
            securityClient = ContainerProvider.GetInstance.Resolve<SecurityServiceClient>();
            securityClient.DeleteUserCompleted += DeleteUserCompleted;
            securityClient.DeleteUserAsync(SelectedUser.ID);
        }

        private bool OnDeleteCanExecute()
        {
            return SelectedUser != null;
        }

        private void OnInsertRoleExecute()
        {
            if (UserInRole == null)
                UserInRole = new ObservableCollection<Role>();
            if (SelectedUser != null)
            {
                if (SelectedUser.Roles == null)
                    SelectedUser.Roles = new ObservableCollection<Role>();
            }
            UserInRole.Add(SelectedRole);
            SelectedRole = null;          
        }

        private bool OnInsertRoleCanExecute()
        {
            return SelectedRole != null && (UserInRole == null ? true : !UserInRole.Any(c => c.ID == SelectedRole.ID));
        }

        private void OnRemoveRoleExecute()
        {      
            UserInRole.Remove(SelectedUserInRole);
        }

        private bool OnRemoveRoleCanExecute()
        {
            return SelectedUserInRole != null;
        }

        private void OnResetPasswordExecute()
        {
            Busy = true;
            securityClient = ContainerProvider.GetInstance.Resolve<SecurityServiceClient>();
            securityClient.ResetPasswordCompleted += ResetPasswordCompleted;
            securityClient.ResetPasswordAsync(SelectedUser.ID);
        }

        private bool OnResetPasswordCanExecute()
        {
            return SelectedUser != null;
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

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(() => Enabled);
                }
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(() => Password);
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

        public ObservableCollection<Detachment> Detachments
        {
            get { return detachments; }   
            set
            {
                if (detachments != value)
                {
                    detachments = value;
                    OnPropertyChanged(() => Detachments);
                }
            }
        }

        public ObservableCollection<Role> UserInRole
        {
            get { return userInRole; }
            set
            {
                if (userInRole != value)
                {
                    userInRole = value;
                    OnPropertyChanged(() => UserInRole);
                }
            }
        }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                if (users != value)
                {
                    users = value;
                    OnPropertyChanged(() => Users);
                }
            }
        }

        public ObservableCollection<Role> Roles
        {
            get { return roles; }
            set
            {
                if (roles != value)
                {
                    roles = value;
                    OnPropertyChanged(() => Roles);
                }
            }
        }

        public ObservableCollection<Worker> Workers
        {
            get
            {
                return workers;
            }
            set
            {
                if (workers != value)
                {
                    workers = value;
                    OnPropertyChanged(() => Workers);
                }
            }
        }

        public Detachment SelectedDetachment
        {
            get { return selectedDetachment; }
            set
            {
                if (selectedDetachment != value)
                {
                    selectedDetachment = value;
                    if (selectedDetachment != null)
                    {
                        dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                        dataClient.FindWorkersCompleted += FindWorkersCompleted;
                        dataClient.FindWorkersAsync(selectedDetachment.ID);
                    }   
                    OnPropertyChanged(() => SelectedDetachment);
                }
            }
        }

        public User SelectedUser
        {
            get { return selectedUser; }     
            set
            {
                selectedUser = value;
                if (selectedUser != null)
                {
                    Enabled = false;
                    Password = null;
                    SelectedRole = null;
                    SelectedUserInRole = null;
                    SelectedDetachment = null;
                    UserName = selectedUser.Name;
                    UserInRole = (selectedUser.Roles != null ? Functionality.CloneCollection(selectedUser.Roles) : null);                    
                    SelectedDetachment = Detachments.FirstOrDefault(c => c.ID == selectedUser.DetachmentID);
                }
                (ResetPasswordCommand as Command).OnCanExecuteChanged();
                OnPropertyChanged(() => SelectedUser);
            }
        }

        public Role SelectedUserInRole
        {
            get { return selectedUserInRole; }
            set
            {
                if (selectedUserInRole != value)
                {
                    selectedUserInRole = value;
                    if (selectedUserInRole != null)
                        SelectedRole = null;
                    (RemoveRoleCommand as Command).OnCanExecuteChanged();
                    OnPropertyChanged(() => SelectedUserInRole);
                }
            }
        }

        public Role SelectedRole
        {
            get { return selectedRole; }
            set
            {
                if (selectedRole != value)
                {
                    selectedRole = value;
                    if (selectedRole != null)
                        SelectedUserInRole = null;
                    (InsertRoleCommand as Command).OnCanExecuteChanged();
                    OnPropertyChanged(() => SelectedRole);
                }
            }
        }

        public Worker SelectedWorker
        {
            get { return selectedWorker; }
            set
            {
                if (selectedWorker != value)
                {
                    selectedWorker = value;
                    OnPropertyChanged(() => SelectedWorker);
                }
            }
        }
        
        #endregion
    }
}
