using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.DataService;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Practices.Unity;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation;
using FluentValidation.Results;

namespace HA.MVVMClient.ViewModels
{
    public class WorkTypeViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<WorkType> workTypes;
        private WorkType selectedWorkType;
        private WorkType oldObject;
        private string description, type;
        private bool enabled;
        private DataServiceClient dataClient;
        private WorkTypeViewModelValidator validator;
        private bool busy;

        #endregion

        #region Constructors

        public WorkTypeViewModel(DataServiceClient dataClient, WorkTypeViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.dataClient.FindWorkTypesCompleted += FindWorkTypesCompleted;
            this.dataClient.FindWorkTypesAsync(LoginInit.user.DetachmentID);
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

        void FindWorkTypesCompleted(object sender, FindWorkTypesCompletedEventArgs e)
        {
            if (e.Error == null)
                Items = e.Result;    
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkTypesCompleted -= FindWorkTypesCompleted;
            Busy = false;
        }

        void UpdateWorkTypeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                oldObject = null;
                (SaveCommand as Command).OnCanExecuteChanged();
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = oldObject;
            }
            dataClient.UpdateWorkTypeCompleted -= UpdateWorkTypeCompleted;
            Busy = false;
        }

        void AddWorkTypeCompleted(object sender, AddWorkTypeCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                selectedWorkType.ID = e.Result;
                Items.Add(selectedWorkType);
                SelectedItem = selectedWorkType;
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = null;           
            }
            dataClient.AddWorkTypeCompleted -= AddWorkTypeCompleted;
            Busy = false;
        }

        void DeleteWorkTypeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Items.Remove(SelectedItem);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteWorkTypeCompleted -= DeleteWorkTypeCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            SaveCommand = new Command(OnSaveExecute, OnSaveCanExecute);
            NewCommand = new Command(OnNewExecute, OnNewCanExecute);
            DeleteCommand = new Command(OnDeleteExecute, OnDeleteCanExecute);
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

        private void OnSaveExecute ()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            if (SelectedItem == null)
            {
                selectedWorkType = new WorkType()
                {
                    Description = Description,
                    DetachmentID = LoginInit.user.DetachmentID,
                    Name = Key
                };
                dataClient.AddWorkTypeCompleted += AddWorkTypeCompleted;
                dataClient.AddWorkTypeAsync(selectedWorkType);
            }
            else
            {
                oldObject = SelectedItem;
                SelectedItem.Description = Description;
                dataClient.UpdateWorkTypeCompleted += UpdateWorkTypeCompleted;
                dataClient.UpdateWorkTypeAsync(SelectedItem);
            }
        }

        private bool OnSaveCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return (SelectedItem == null ? true :
            (String.IsNullOrWhiteSpace(SelectedItem.Description) ? null : SelectedItem.Description) !=
            (String.IsNullOrWhiteSpace(Description) ? null : Description)) && IsValid;
        }

        private void OnNewExecute()
        {
            SelectedItem = null;
            Description = null;
            Key = null;
            Enabled = true;
        }

        private bool OnNewCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedItem != null;
        }

        private void OnDeleteExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.DeleteWorkTypeCompleted += DeleteWorkTypeCompleted;
            dataClient.DeleteWorkTypeAsync(SelectedItem.ID);
        }

        private bool OnDeleteCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedItem != null;
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return "Typy oprav"; }
        }

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

        public string Name
        {
            get { return "Typ:"; }
        }

        public string List
        {
            get { return "Typy:"; }
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

        public WorkType SelectedItem
        {
            get { return selectedWorkType; }
            set
            {                
                selectedWorkType = value;
                if (selectedWorkType != null)
                {
                    Description = selectedWorkType.Description;
                    Key = selectedWorkType.Name;
                    Enabled = false;
                }
                OnPropertyChanged(() => SelectedItem);          
            }
        }

        public ObservableCollection<WorkType> Items
        {
            get { return workTypes; }
            set 
            {
                if (workTypes != value)
                {
                    workTypes = value;
                    OnPropertyChanged(() => Items);
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(() => Description);
                }
            }
        }

        public string Key
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(() => Key);
                }
            }
        }

        #endregion
    }
}
