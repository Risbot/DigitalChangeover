using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation;
using FluentValidation.Results;
using System.Windows;

namespace HA.MVVMClient.ViewModels
{
    public class WorkerViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<Tour> workerTours;
        private ObservableCollection<Tour> tours;
        private Worker selectedWorker;
        private WorkerDetail worker;
        private ObservableCollection<Worker> workers;
        private ImageSource photo;
        private bool enabled;
        private Tour selectedTour;
        private Tour selectedWorkerTour;
        private DataServiceClient dataClient;
        private string lastName;
        private string firstName;
        private string description;
        private string personalEmail;
        private string personalPhone;
        private string sapNumber;
        private string serviceEmail;
        private string servicePhone;
        private string serviceNumber;
        private bool update = false;
        private WorkerViewModelValidator validator;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public WorkerViewModel(DataServiceClient dataClient, WorkerViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            busyCount = 1;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.dataClient.FindWorkersCompleted += FindWorkersCompleted;
            this.dataClient.FindWorkersAsync(LoginInit.user.DetachmentID);
            this.dataClient.FindToursCompleted += FindToursCompleted;
            this.dataClient.FindToursAsync(LoginInit.user.DetachmentID);
            PropertyChanged += (s, e) =>
            {
                (SaveCommand as Command).OnCanExecuteChanged();
            };
        }

        #endregion

        #region Functions

        private BitmapImage PhotoConvert(byte[] source)
        {
            if (source != null)
            {
                BitmapImage bitmapimage = new BitmapImage();
                MemoryStream stream = new MemoryStream(source);
                bitmapimage.StreamSource = stream;
                return bitmapimage;
            }
            return null;
        }

        public override ValidationResult Validator(string propertyName)
        {
            return validator.Validate(this, propertyName);
        }

        #endregion

        #region Events 

        void FindToursCompleted(object sender, FindToursCompletedEventArgs e)
        {
            if (e.Error == null)
                Tours = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindToursCompleted -= FindToursCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkersCompleted(object sender, FindWorkersCompletedEventArgs e)
        {
            if (e.Error == null)
                Workers = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkersCompleted -= FindWorkersCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void UpdateWorkerCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {  
                dataClient.FindWorkerCompleted += FindWorkerCompleted;
                dataClient.FindWorkerAsync(selectedWorker.ID);
                (SaveCommand as Command).OnCanExecuteChanged();
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.UpdateWorkerCompleted -= UpdateWorkerCompleted;
            Busy = false;
        }

        void AddWorkerCompleted(object sender, AddWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Worker.ID = e.Result;
                var w = new Worker()
                {
                    ID = Worker.ID,
                    FirstName = Worker.FirstName,
                    LastName = Worker.LastName,
                    SapNumber = Worker.SapNumber,
                    ServicePhone = Worker.ServicePhone,
                    DetachmentID = Worker.DetachmentID
                };
                Workers.Add(w);
                SelectedWorker = w;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.AddWorkerCompleted -= AddWorkerCompleted;
            Busy = false;
        }

        void DeleteWorkerCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Workers.Remove(SelectedWorker);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteWorkerCompleted -= DeleteWorkerCompleted;
            Busy = false;
        }

        void FindWorkerCompleted(object sender, FindWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                
                update = false;
                Worker = e.Result;
                WorkerTours = Functionality.CloneCollection(Worker.Tours);
                Description = Worker.Description;
                FirstName = Worker.FirstName;
                LastName = Worker.LastName;
                PersonalEmail = Worker.PersonalEmail;
                PersonalPhone = Worker.PersonalPhone;
                SapNumber = Worker.SapNumber;
                ServiceEmail = Worker.ServiceEmail;
                ServiceNumber = Worker.ServiceNumber;
                ServicePhone = Worker.ServicePhone;
                Photo = PhotoConvert(worker.Photo);
                SelectedTour = null;
                SelectedWorkerTour = null;
                Enabled = false;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorkerCompleted -= FindWorkerCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            SaveCommand = new Command(OnSaveExecute, OnSaveCanExecute);
            NewCommand = new Command(OnNewExecute, OnNewCanExecute);
            DeleteCommand = new Command(OnDeleteExecute, OnDeleteCanExecute);
            InsertTourCommand = new Command(OnInsertTourExecute, OnInsertTourCanExecute);
            RemoveTourCommand = new Command(OnRemoveTourExecute, OnRemoveTourCanExecute); 
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

        public ICommand InsertTourCommand
        {
            get;
            private set;
        }

        public ICommand RemoveTourCommand
        {
            get;
            private set;
        }

        private void OnSaveExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            if (Worker == null)
            {
                Worker = new WorkerDetail()
                {
                    Description = Description,
                    DetachmentID = LoginInit.user.DetachmentID,
                    FirstName = FirstName,
                    LastName = LastName,
                    PersonalEmail = PersonalEmail,
                    PersonalPhone = PersonalPhone,
                    SapNumber = SapNumber,
                    ServiceEmail = ServiceEmail,
                    ServiceNumber = ServiceNumber,
                    ServicePhone = ServicePhone,
                    Tours = WorkerTours
                };
                dataClient.AddWorkerCompleted += AddWorkerCompleted;
                dataClient.AddWorkerAsync(Worker);
            }
            else
            {
                update = true;
                Worker.Description = Description;
                Worker.PersonalEmail = PersonalEmail;
                Worker.PersonalPhone = PersonalPhone;
                Worker.ServiceEmail = ServiceEmail;
                Worker.ServicePhone = ServicePhone;
                Worker.Tours = WorkerTours;
                var w = SelectedWorker;
                var index = Workers.IndexOf(SelectedWorker);
                Workers.Remove(SelectedWorker);
                w.ServicePhone = ServicePhone;
                Workers.Insert(index, w);
                SelectedWorker = w;
                dataClient.UpdateWorkerCompleted += UpdateWorkerCompleted;
                dataClient.UpdateWorkerAsync(Worker);    
            }
        }

        private bool OnSaveCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return IsValid &&
                (Worker == null ? true : ServiceEmail != Worker.ServiceEmail ||
                Worker == null ? true : PersonalEmail != Worker.PersonalEmail ||
                Worker == null ? true : ServicePhone != Worker.ServicePhone ||
                Worker == null ? true : PersonalPhone != Worker.PersonalPhone ||
                Worker == null ? true : Description != Worker.Description ||
                Worker == null ? true : WorkerTours == null ? false : !Functionality.CompareCollection<Tour>(Worker.Tours, WorkerTours, new CompareTour()) && WorkerTours.Count != 0);
        }

        private void OnNewExecute()
        {
            SelectedTour = null;
            SelectedWorkerTour = null;
            Photo = null;          
            WorkerTours = null;
            ServiceEmail = null;
            ServicePhone = null;
            ServiceNumber = null;
            SapNumber = null;
            PersonalEmail = null;
            PersonalPhone = null;
            Description = null;
            FirstName = null;
            LastName = null;
            SelectedWorker = null;
            Worker = null;       
            Enabled = true;
        }

        private bool OnNewCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedWorker != null;
        }

        private void OnDeleteExecute()
        {
            var n = Navigator.CreateChild();
            if (n.ShowMessageBox("Jste si jistí že chcete smazat zaměstnance " + SelectedWorker.FirstName + " " + SelectedWorker.LastName  + "?\nSmažou se i veškeré záznamy spojené s tímto zaměstnancem!", "Varováni", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Busy = true;
                dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                dataClient.DeleteWorkerCompleted += DeleteWorkerCompleted;
                dataClient.DeleteWorkerAsync(SelectedWorker.ID);
            }
        }

        private bool OnDeleteCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedWorker != null;
        }

        private void OnInsertTourExecute()
        {
            if (WorkerTours == null)
                WorkerTours = new ObservableCollection<Tour>();
            WorkerTours.Add(SelectedTour);
            OnPropertyChanged(() => WorkerTours);
            SelectedTour = null;
        }

        private bool OnInsertTourCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedTour != null && (WorkerTours == null? true : !WorkerTours.Any(c=>c.ID == SelectedTour.ID));
        }

        private void OnRemoveTourExecute()
        {
            WorkerTours.Remove(SelectedWorkerTour);
            OnPropertyChanged(() => WorkerTours);
        }

        private bool OnRemoveTourCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedWorkerTour != null;
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

        public Tour SelectedTour
        {
            get { return selectedTour; }
            set
            {
                if (selectedTour != value)
                {
                    selectedTour = value;
                    (InsertTourCommand as Command).OnCanExecuteChanged();
                    OnPropertyChanged(() => SelectedTour);
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
                    (RemoveTourCommand as Command).OnCanExecuteChanged();
                    OnPropertyChanged(() => SelectedWorkerTour);
                }
            }
        }

        public ImageSource Photo
        {
            get 
            { 
                if (photo != null)
                    return photo;
                else
                    return photo = new BitmapImage(new Uri(@".\anonim_user.png", UriKind.RelativeOrAbsolute));       
            }
            set 
            {
                if (photo != value)
                {
                    photo = value;
                    OnPropertyChanged(() => Photo);
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
                    OnPropertyChanged(() => Worker);
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
                    (NewCommand as Command).OnCanExecuteChanged();
                    (DeleteCommand as Command).OnCanExecuteChanged();
                    if (selectedWorker != null && !update)
                    {
                        dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                        dataClient.FindWorkerCompleted += FindWorkerCompleted;
                        dataClient.FindWorkerAsync(selectedWorker.ID);
                    }
                    OnPropertyChanged(() => SelectedWorker);
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

        public string ServiceNumber
        {
            get { return serviceNumber; }
            set
            {
                if (serviceNumber != value)
                {
                    serviceNumber = value;
                    OnPropertyChanged(() => ServiceNumber);
                }
            }
        }

        public string ServicePhone
        {
            get { return servicePhone; }
            set
            {
                if (servicePhone != value)
                {
                    servicePhone = value;
                    OnPropertyChanged(() => ServicePhone);
                }
            }
        }

        public string ServiceEmail
        {
            get { return serviceEmail; }
            set
            {
                if (serviceEmail != value)
                {
                    serviceEmail = value;
                    OnPropertyChanged(() => ServiceEmail);
                }
            }
        }

        public string SapNumber
        {
            get { return sapNumber; }
            set
            {
                if (sapNumber != value)
                {
                    sapNumber = value;
                    OnPropertyChanged(() => SapNumber);
                }
            }
        }

        public string PersonalPhone
        {
            get { return personalPhone; }
            set
            {
                if (personalPhone != value)
                {
                    personalPhone = value;
                    OnPropertyChanged(() => PersonalPhone);
                }
            }
        }

        public string PersonalEmail
        {
            get { return personalEmail; }
            set
            {
                if (personalEmail != value)
                {
                    personalEmail = value;
                    OnPropertyChanged(() => PersonalEmail);
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
