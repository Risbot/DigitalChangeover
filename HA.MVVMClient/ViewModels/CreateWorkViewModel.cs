using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using System.Windows.Input;
using System.Windows;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class CreateWorkViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<Vehicle> vehicles;
        private ObservableCollection<WorkType> workTypes;
        private DataServiceClient dataClient;
        private string faultDescription;
        private string causeDescription;
        private Date date;
        private bool enabled;
        private Vehicle selectedVehicle;
        private WorkType selectedWorkType;
        private string selectedTopFaultWork;
        private string selectedTopCauseWork;
        private CreateWorkViewModelValidator validator;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public CreateWorkViewModel(DataServiceClient dataClient, CreateWorkViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            busyCount = 1;
            this.dataClient = dataClient;
            this.validator = validator;
            Enabled = true;
            Navigator = navigator;
            Date = Navigator.Parameters.Date;
            TopFaultWorks = Navigator.Parameters.TopFaultWorks;
            TopCauseWorks = Navigator.Parameters.TopCauseWorks;
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

        void AddWorkCompleted(object sender, AddWorkCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                Work.ID = e.Result;
                OnCloseView();
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.AddWorkCompleted -= AddWorkCompleted;
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
            Work = new Work()
            {
                DateID = Date.ID,
                FaultDescription = String.IsNullOrWhiteSpace(FaultDescription) ? SelectedTopFaultWork : FaultDescription,
                CauseDescription = String.IsNullOrWhiteSpace(CauseDescription) ? SelectedTopCauseWork : CauseDescription,
                VehicleID = SelectedVehicle.ID,
                VehicleNumber = SelectedVehicle.Number,
                WorkTypeID = SelectedWorkType.ID,
                WorkTypeName = SelectedWorkType.Name
            };
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.AddWorkCompleted += AddWorkCompleted;
            dataClient.AddWorkAsync(Work);
        }

        public bool OnCreateCanExecute()
        {
            return IsValid;
                //SelectedVehicle != null &&
                //SelectedWorkType != null &&
                //(SelectedTopFaultWork != null || FaultDescription != null);
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return "Zakládaní práce"; }
        }

        public int FaultSpan
        {
            get { return 1; }
        }

        public Visibility CauseVisibility
        {
            get { return Visibility.Visible; }
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

        public Work Work
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

        public ObservableCollection<string> TopCauseWorks
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

        public string SelectedTopCauseWork
        {
            get { return selectedTopCauseWork; }
            set
            {
                if (selectedTopCauseWork != value)
                {
                    selectedTopCauseWork = value;
                    OnPropertyChanged(() => SelectedTopCauseWork);
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

        public string CauseDescription
        {
            get { return causeDescription; }
            set
            {
                if (causeDescription != value)
                {
                    causeDescription = value;
                    OnPropertyChanged(() => CauseDescription);
                }
            }
        }

        #endregion
    }
}
