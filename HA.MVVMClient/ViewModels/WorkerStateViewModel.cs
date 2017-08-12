using FluentValidation.Results;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.ViewModelsValidators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using FluentValidation;
using Microsoft.Practices.Unity;


namespace HA.MVVMClient.ViewModels
{
    public class WorkerStateViewModel : BaseViewModel
    {
        #region Variables

        private string state;
        private string description;
        private ObservableCollection<WorkerState> workerStates;
        private WorkerState selectedWorkerState;
        private WorkerState oldObject;
        private bool enabled;
        private DataServiceClient dataClient;
        private WorkerStateViewModelValidator validator;
        private bool busy;

        #endregion

        #region Constructors

        public WorkerStateViewModel(DataServiceClient dataClient, WorkerStateViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.dataClient.FindWorkerStatesCompleted += FindWorkerStatesCompleted;
            this.dataClient.FindWorkerStatesAsync(LoginInit.user.DetachmentID);
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

        void FindWorkerStatesCompleted(object sender, FindWorkerStatesCompletedEventArgs e)
        {
            if (e.Error == null)
                Items = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkerStatesCompleted -= FindWorkerStatesCompleted;
            Busy = false;
        }

        void AddWorkerStateCompleted(object sender, AddWorkerStateCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                selectedWorkerState.ID = e.Result;
                Items.Add(selectedWorkerState);
                SelectedItem = selectedWorkerState;
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = null;
            }
            dataClient.AddWorkerStateCompleted -= AddWorkerStateCompleted;
            Busy = false;
        }

        void UpdateWorkerStateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
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
            dataClient.UpdateWorkerStateCompleted -= UpdateWorkerStateCompleted;
            Busy = false;
        }

        void DeleteWorkerStateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Items.Remove(SelectedItem);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteWorkerStateCompleted -= DeleteWorkerStateCompleted;
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

        private void OnSaveExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            if (SelectedItem == null)
            {
                selectedWorkerState = new WorkerState
                {
                    Description = Description,
                    DetachmentID = LoginInit.user.DetachmentID,
                    Name = Key
                };
                dataClient.AddWorkerStateCompleted += AddWorkerStateCompleted;
                dataClient.AddWorkerStateAsync(selectedWorkerState);
            }
            else
            {
                oldObject = SelectedItem;
                SelectedItem.Description = Description;
                dataClient.UpdateWorkerStateCompleted += UpdateWorkerStateCompleted;
                dataClient.UpdateWorkerStateAsync(SelectedItem);
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
            dataClient.DeleteWorkerStateCompleted += DeleteWorkerStateCompleted;
            dataClient.DeleteWorkerStateAsync(SelectedItem.ID);
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
            get { return "Statusy zaměstnanců"; }
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
            get { return "Status:"; }
        }

        public string List
        {
            get { return "Statusy:"; }
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

        public WorkerState SelectedItem
        {
            get { return selectedWorkerState; }
            set
            {
                selectedWorkerState = value;
                if (selectedWorkerState != null)
                {
                    Description = selectedWorkerState.Description;
                    Key = selectedWorkerState.Name;
                    Enabled = false;
                }
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public ObservableCollection<WorkerState> Items
        {
            get { return workerStates; }
            set
            {
                if (workerStates != value)
                {
                    workerStates = value;
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
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(() => Key);
                }
            }
        }

        #endregion
    }
}
