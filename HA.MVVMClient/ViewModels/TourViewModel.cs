using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HA.MVVMClient.DataService;
using System.Windows.Input;
using HA.MVVMClient.Infrastructure;
using Microsoft.Practices.Unity;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation.Results;
using FluentValidation;

namespace HA.MVVMClient.ViewModels
{
    public class TourViewModel : BaseViewModel
    {      
        #region Variables

        private bool enabled;
        private string startTime;
        private string endTime;
        private Tour selectedTour;
        private string description;
        private ObservableCollection<Tour> tours;
        private DataServiceClient dataClient;
        private Tour oldObject;
        private bool busy;
        private TourViewModelValidator validator;

        #endregion

        #region Constructors

        public TourViewModel(DataServiceClient dataClient, TourViewModelValidator validator ,INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            Navigator = navigator;
            this.validator = validator;
            Enabled = true;
            InitCommands();
            this.dataClient.FindToursCompleted += FindToursCompleted;
            this.dataClient.FindToursAsync(LoginInit.user.DetachmentID);
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

        void FindToursCompleted(object sender, FindToursCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Tours = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindToursCompleted -= FindToursCompleted;
            Busy = false;
        }

        void AddTourCompleted(object sender, AddTourCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                selectedTour.ID = e.Result;
                Tours.Add(selectedTour);
                SelectedTour = selectedTour;
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedTour = null;
            }   
            dataClient.AddTourCompleted -= AddTourCompleted;
            Busy = false;
        }

        void DeleteTourCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Tours.Remove(SelectedTour);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteTourCompleted -= DeleteTourCompleted;
            Busy = false;
        }

        void UpdateTourCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                oldObject = null;
                (SaveCommand as Command).OnCanExecuteChanged();
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedTour = oldObject;
            }
            dataClient.UpdateTourCompleted -= UpdateTourCompleted;
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
            if (SelectedTour == null)
            {
                selectedTour = new Tour()
                {
                    Description = this.Description,
                    DetachmentID = LoginInit.user.DetachmentID,
                    StartTime = TimeSpan.Parse(this.StartTime),
                    EndTime = TimeSpan.Parse(this.EndTime)
                };
                dataClient.AddTourCompleted += AddTourCompleted;
                dataClient.AddTourAsync(selectedTour);
            }
            else
            {
                oldObject = SelectedTour;
                SelectedTour.Description = this.Description;
                dataClient.UpdateTourCompleted += UpdateTourCompleted;
                dataClient.UpdateTourAsync(SelectedTour);
            }
        }

        private bool OnSaveCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return (SelectedTour == null ? true :
            (String.IsNullOrWhiteSpace(SelectedTour.Description) ? null : SelectedTour.Description) !=
            (String.IsNullOrWhiteSpace(Description) ? null : Description)) && IsValid;
        }

        private void OnNewExecute()
        {
            Description = null;
            SelectedTour = null;
            StartTime = null;
            EndTime = null;
            Enabled = true;
        }

        private bool OnNewCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedTour != null;
        }

        private void OnDeleteExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.DeleteTourCompleted += DeleteTourCompleted;
            dataClient.DeleteTourAsync(SelectedTour.ID);
        }

        private bool OnDeleteCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedTour != null;
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
                enabled = value;
                OnPropertyChanged(() => Enabled);
            }
        }

        public Tour SelectedTour
        {
            get { return selectedTour; }
            set
            {
                selectedTour = value;
                if (selectedTour != null)
                {
                    StartTime = selectedTour.StartTime.ToString("hh\\:mm");
                    EndTime = selectedTour.EndTime.ToString("hh\\:mm");
                    Description = selectedTour.Description;
                    Enabled = false;
                }
                OnPropertyChanged(() => SelectedTour);  
            }
        }
    
        public string StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    OnPropertyChanged(() => StartTime);
                }
            }
        }

        public string EndTime
        {
            get { return endTime; }  
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    OnPropertyChanged(() => EndTime);
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

        public ObservableCollection<Tour> Tours 
        {
            get { return tours; }
            set 
            {
                if (tours != value)
                {
                    tours = value;
                    OnPropertyChanged(() => Tours);
                }
            }
        }

        #endregion
    }
}
