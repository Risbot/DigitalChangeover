using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.ViewModelsValidators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class CreateChangeoverViewModel : BaseViewModel
    {   
        #region Variables

        private ObservableCollection<Vehicle> vehicles;
        private ObservableCollection<WorkType> workTypes;
        private CreateChangeoverViewModelValidator validator;
        private DataServiceClient dataClient;
        private string faultDescription;
        private Date date;
        private bool enabled;
        private Vehicle selectedVehicle;
        private WorkType selectedWorkType;
        private string selectedTopFaultWork;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public CreateChangeoverViewModel(DataServiceClient dataClient, CreateChangeoverViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            Enabled = true;
            busyCount = 1;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Date = Navigator.Parameters.Date;
            TopFaultWorks = Navigator.Parameters.TopFaultWorks;
            InitCommands();
            this.dataClient.FindVehiclesCompleted += FindVehiclesCompleted;
            this.dataClient.FindVehiclesAsync(LoginInit.user.DetachmentID);
            this.dataClient.FindWorkTypesCompleted += FindWorkTypesCompleted;
            this.dataClient.FindWorkTypesAsync(LoginInit.user.DetachmentID);
            PropertyChanged += (s, e) =>
            {
                (CreateCommand as Command).OnCanExecuteChanged();
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

        void FindVehiclesCompleted(object sender, FindVehiclesCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                Vehicles = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindVehiclesCompleted -= FindVehiclesCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkTypesCompleted(object sender, FindWorkTypesCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                WorkTypes = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkTypesCompleted -= FindWorkTypesCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void AddChangeoverCompleted(object sender, AddChangeoverCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                Changeover.ID = e.Result;
                OnCloseView();
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.AddChangeoverCompleted -= AddChangeoverCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            CreateCommand = new Command(OnCreateExecute, OnCreateCanExecute);
        }

        public ICommand CreateCommand
        {
            get;
            private set;
        }

        public void OnCreateExecute()
        {
            Busy = true;
            Changeover = new Changeover
            {
                DateID = Date.ID,
                DateContent = Date.DateContent,
                IsNight = Date.IsNight,
                Description = String.IsNullOrWhiteSpace(FaultDescription) ? SelectedTopFaultWork : FaultDescription,
                DetachmentID = LoginInit.user.DetachmentID,
                VehicleID = SelectedVehicle.ID,
                VehicleNumber = SelectedVehicle.Number,
                WorkTypeID = SelectedWorkType.ID,
                WorkTypeName = SelectedWorkType.Name
            };
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.AddChangeoverCompleted += AddChangeoverCompleted;
            dataClient.AddChangeoverAsync(Changeover);
        }

        public bool OnCreateCanExecute()
        {
            return IsValid;
            //return SelectedVehicle != null &&
            //    SelectedWorkType != null &&
            //    (SelectedTopFaultWork != null || Description != null);
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return "Zakládaní předávky"; }
        }

        public int FaultSpan
        {
            get { return 2; }
        }

        public Visibility CauseVisibility
        {
            get { return Visibility.Collapsed; }
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

        public Changeover Changeover
        {
            get;
            private set;
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

        public Vehicle SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                if (selectedVehicle != value)
                {
                    selectedVehicle = value;
                    OnPropertyChanged(() => SelectedVehicle);
                }
            }
        }

        public WorkType SelectedWorkType
        {
            get { return selectedWorkType; }
            set
            {
                if (selectedWorkType != value)
                {
                    selectedWorkType = value;
                    OnPropertyChanged(() => SelectedWorkType);
                }
            }
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get { return vehicles; }
            set
            {
                if (vehicles != value)
                {
                    vehicles = value;
                    OnPropertyChanged(() => Vehicles);
                }
            }
        }

        public ObservableCollection<WorkType> WorkTypes
        {
            get { return workTypes; }
            set
            {
                if (workTypes != value)
                {
                    workTypes = value;
                    OnPropertyChanged(() => WorkTypes);
                }
            }
        }

        public ObservableCollection<string> TopFaultWorks
        {
            get;
            private set;
        }

        public string SelectedTopFaultWork
        {
            get { return selectedTopFaultWork; }
            set
            {
                if (selectedTopFaultWork != value)
                {
                    selectedTopFaultWork = value;
                    OnPropertyChanged(() => FaultDescription);
                    OnPropertyChanged(() => SelectedTopFaultWork);
                }
            }
        }

        public Date Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged(() => Date);
                }
            }
        }

        public string FaultDescription
        {
            get { return faultDescription; }
            set
            {
                if (faultDescription != value)
                {
                    faultDescription = value;
                    OnPropertyChanged(() => SelectedTopFaultWork);
                    OnPropertyChanged(() => FaultDescription);
                }
            }
        }

        #endregion  
    }
}
