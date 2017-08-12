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
using FluentValidation.Results;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class AttendanceViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<Attendance> attendances;
        private Attendance selectedAttendance;
        private ObservableCollection<Worker> workers;
        private Worker selectedWorker;
        private WorkerDetail worker;
        private ObservableCollection<WorkerState> workerStates;
        private WorkerState selectedWorkerState;
        private ObservableCollection<Tour> workerTours;
        private AttendanceViewModelValidator validator;
        private Tour selectedWorkerTour;
        private string firstName, lastName, description;
        private DataServiceClient dataClient;
        private Date date;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public AttendanceViewModel(DataServiceClient dataClient, AttendanceViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            busyCount = 1;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Date = Navigator.Parameters.Date;
            Attendances = Navigator.Parameters.Attendances;
            InitCommands();
            this.dataClient.FindWorkerStatesCompleted += FindWorkerStatesCompleted;
            this.dataClient.FindWorkerStatesAsync(LoginInit.user.DetachmentID);
            this.dataClient.FindWorkersCompleted += FindWorkersCompleted;
            this.dataClient.FindWorkersAsync(LoginInit.user.DetachmentID);
            PropertyChanged += (s, e) =>
            {
                (AddCommand as Command).OnCanExecuteChanged();
                (RemoveCommand as Command).OnCanExecuteChanged();
                (UpdateCommand as Command).OnCanExecuteChanged();
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

        void DeleteAttendanceCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                FirstName = null;
                LastName = null;
                SelectedWorkerState = null;
                SelectedWorkerTour = null;
                WorkerTours = null;
                Description = null;
                Attendances.Remove(SelectedAttendance);
                SelectedAttendance = null;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteAttendanceCompleted -= DeleteAttendanceCompleted;
            Busy = false;
        }

        void AddAttendanceCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
          
            if (e.Error == null)
            {
                Attendances.Add(selectedAttendance);
                OnPropertyChanged(() => SelectedAttendance);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.AddAttendanceCompleted -= AddAttendanceCompleted;
            Busy = false;
        }

        void UpdateAttendanceCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
          
            if (e.Error == null)
            {
                var a = SelectedAttendance;
                var index = Attendances.IndexOf(SelectedAttendance);
                Attendances.RemoveAt(index);
                Attendances.Insert(index, a);
                SelectedAttendance = a;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            Busy = false;
        }

        void FindWorkersCompleted(object sender, FindWorkersCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                Workers = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkersCompleted -= FindWorkersCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkerCompleted(object sender, FindWorkerCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                Worker = e.Result;
                if (SelectedAttendance != null)
                {
                    SelectedWorkerState = WorkerStates.FirstOrDefault(c => c.ID == selectedAttendance.WorkerStateID);
                    SelectedWorkerTour = WorkerTours.FirstOrDefault(c => c.ID == selectedAttendance.WorkerTourID);
                    Description = selectedAttendance.Description;
                    (UpdateCommand as Command).OnCanExecuteChanged();
                }
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkerCompleted -= FindWorkerCompleted;
            Busy = false;
        }

        void FindWorkerStatesCompleted(object sender, FindWorkerStatesCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                WorkerStates = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkerStatesCompleted -= FindWorkerStatesCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            AddCommand = new Command(OnAddExecute, OnAddCanExecute);
            RemoveCommand = new Command(OnRemoveExecute, OnRemoveCanExecute);
            UpdateCommand = new Command(OnUpdateExecute, OnUpdateCanExecute);
        }

        private void OnUpdateExecute()
        {
            Busy = true;
            SelectedAttendance.WorkerStateID = SelectedWorkerState.ID;
            SelectedAttendance.WorkerState = SelectedWorkerState.Name;
            SelectedAttendance.WorkerTourID = SelectedWorkerTour.ID;
            SelectedAttendance.WorkerTour = SelectedWorkerTour.StartTime.ToString() + " - " + SelectedWorkerTour.EndTime.ToString();
            SelectedAttendance.Description = Description;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.UpdateAttendanceCompleted += UpdateAttendanceCompleted;
            dataClient.UpdateAttendanceAsync(SelectedAttendance);
        }

        private bool OnUpdateCanExecute()
        {
            return SelectedAttendance != null && !Date.IsClosed &&
                (SelectedAttendance.WorkerTourID != (SelectedWorkerTour != null ? SelectedWorkerTour.ID : 0)||
                SelectedAttendance.WorkerStateID != (SelectedWorkerState != null ? SelectedWorkerState.ID : 0) ||
                SelectedAttendance.Description != (String.IsNullOrWhiteSpace(Description) ? null : Description));
        }

        private void OnRemoveExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.DeleteAttendanceCompleted += DeleteAttendanceCompleted;
            dataClient.DeleteAttendanceAsync(SelectedAttendance);
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedAttendance != null && !Date.IsClosed;
        }

        private void OnAddExecute()
        {
            Busy = true;
            selectedAttendance = new Attendance()
            {
                DateID = Date.ID,
                Description = Description,
                FirstName = FirstName,
                LastName = LastName,
                SapNumber = Worker.SapNumber,
                WorkerID = Worker.ID,
                WorkerState = SelectedWorkerState.Name,
                WorkerStateID = SelectedWorkerState.ID,
                WorkerTour = SelectedWorkerTour.StartTime.ToString() + " - " + SelectedWorkerTour.EndTime.ToString(),
                WorkerTourID = SelectedWorkerTour.ID
            };
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.AddAttendanceCompleted += AddAttendanceCompleted;
            dataClient.AddAttendanceAsync(selectedAttendance);
        }

        private bool OnAddCanExecute()
        {
            return Worker != null &&
                SelectedWorkerTour != null &&
                SelectedWorkerState != null &&
                SelectedWorker != null &&
                !Attendances.Any(c=>c.WorkerID == SelectedWorker.ID);
        }

        public ICommand AddCommand
        {
            get;
            private set;
        }

        public ICommand RemoveCommand
        {
            get;
            private set;
        }

        public ICommand UpdateCommand
        {
            get;
            private set;
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

        public ObservableCollection<Attendance> Attendances
        {
            get { return attendances; }
            set
            {
                if (attendances != value)
                {
                    attendances = value;
                    OnPropertyChanged(() => Attendances);
                }
            }
        }

        public Attendance SelectedAttendance
        {
            get { return selectedAttendance; }
            set
            {
                if (selectedAttendance != value)
                {
                    selectedAttendance = value;
                    if (selectedAttendance != null)
                    {
                        SelectedWorker = null;
                        dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                        dataClient.FindWorkerCompleted += FindWorkerCompleted;
                        dataClient.FindWorkerAsync(selectedAttendance.WorkerID);
                    }
                    OnPropertyChanged(() => SelectedAttendance);
                }
            }
        }

        public ObservableCollection<Worker> Workers
        {
            get { return workers; }
            set
            {
                if (workers != value)
                {
                    workers = value;
                    OnPropertyChanged(() => Workers);
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
                    if (selectedWorker != null)
                    {
                        SelectedAttendance = null;
                        SelectedWorkerState = null;
                        Description = null;
                        SelectedWorkerTour = null;
                        dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                        dataClient.FindWorkerCompleted += FindWorkerCompleted;
                        dataClient.FindWorkerAsync(selectedWorker.ID);
                        OnPropertyChanged(() => SelectedWorkerTour);
                        OnPropertyChanged(() => SelectedWorkerState);
                    }
                    OnPropertyChanged(() => SelectedWorker);
                }
            }
        }

        public WorkerDetail Worker
        {
            get { return worker; }
            set
            {
                if (worker != value)
                {
                    worker = value;
                    FirstName = worker != null ? worker.FirstName : null;
                    LastName = worker != null ? worker.LastName : null;
                    WorkerTours = worker != null ? worker.Tours : null;                 
                    OnPropertyChanged(() => Worker);
                }
            }
        }

        public ObservableCollection<WorkerState> WorkerStates
        {
            get { return workerStates; }
            set
            {
                if (workerStates != value)
                {
                    workerStates = value;
                    OnPropertyChanged(() => WorkerStates);
                }
            }
        }

        public WorkerState SelectedWorkerState
        {
            get { return selectedWorkerState; }
            set
            {
                if (selectedWorkerState != value)
                {
                    selectedWorkerState = value;
                    OnPropertyChanged(() => SelectedWorkerState);
                }
            }
        }

        public ObservableCollection<Tour> WorkerTours
        {
            get { return workerTours; }
            set
            {
                if (workerTours != value)
                {
                    workerTours = value;
                    OnPropertyChanged(() => WorkerTours);
                }
            }
        }

        public Tour SelectedWorkerTour
        {
            get { return selectedWorkerTour; }
            set
            {
                if (selectedWorkerTour != value)
                {
                    selectedWorkerTour = value;
                    OnPropertyChanged(() => SelectedWorkerTour);
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

        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(() => FirstName);
                }
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(() => LastName);
                }
            }
        }

        #endregion
    }
}
