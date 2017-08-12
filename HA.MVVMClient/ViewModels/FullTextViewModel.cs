using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.FullTextService;
using HA.MVVMClient.DataService;
using System.Collections.ObjectModel;
using HA.MVVMClient.Infrastructure;
using System.Windows.Input;
using System.Windows.Controls;
using HA.MVVMClient.Views;
using System.Windows;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class FullTextViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<FullTextWork> works;
        private ObservableCollection<WorkType> workTypes;
        private DateTime from;
        private DateTime to;
        private string searchKey;
        private ObservableCollection<Vehicle> vehicles;
        private Vehicle selectedVehicle;
        private WorkType selectedWorkType;
        private DataServiceClient dataClient;
        private FullTextServiceClient fullTextClient;
        private FullTextWork selectedWork;
        private ObservableCollection<Detachment> detachments;
        private Detachment selectedDetachment;
        private System.Windows.Visibility visibility;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public FullTextViewModel(DataServiceClient dataClient, INavigator navigator)
        {
            
            Busy = true;
            busyCount = 2;
            Navigator = navigator;
            this.dataClient = dataClient;
            Visibility = System.Windows.Visibility.Collapsed;
            From = DateTime.Now.Date;
            To = DateTime.Now.Date;
            InitCommands();
            this.dataClient.FindVehiclesCompleted += FindVehiclesCompleted;
            this.dataClient.FindVehiclesAsync(LoginInit.user.DetachmentID);
            this.dataClient.FindWorkTypesCompleted += FindWorkTypesCompleted;
            this.dataClient.FindWorkTypesAsync(LoginInit.user.DetachmentID);
            this.dataClient.GetDetachmentsCompleted += GetDetachmentsCompleted;
            this.dataClient.GetDetachmentsAsync();
            PropertyChanged += (s, e) =>
            {
                (SearchCommand as Command).OnCanExecuteChanged();
                (ResetCommand as Command).OnCanExecuteChanged();
            };
        }

        #endregion

        #region Events 

        void GetDetachmentsCompleted(object sender, GetDetachmentsCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                Detachments = e.Result;
                SelectedDetachment = Detachments.FirstOrDefault(c => c.ID == LoginInit.user.DetachmentID);
                if (LoginInit.user.Roles.Any(c => c.Name == "Master"))
                    Visibility = System.Windows.Visibility.Visible;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.GetDetachmentsCompleted -= GetDetachmentsCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkTypesCompleted(object sender, FindWorkTypesCompletedEventArgs e)
        {
            
            if (e.Error == null)
                WorkTypes = e.Result;
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
                Vehicles = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindVehiclesCompleted -= FindVehiclesCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindWorkCompleted(object sender, FindWorkCompletedEventArgs e)
        {
           
            if (e.Error == null)
                Works = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            fullTextClient.FindWorkCompleted -= FindWorkCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            SearchCommand = new Command(OnSearchExecute, OnSearchCanExecute);
            ResetCommand = new Command(OnResetExecute, OnResetCanExecute);
            DetailCommand = new Command(OnDetailExecute, OnDetailCanExecute);
        }

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public ICommand ResetCommand
        {
            get;
            private set;
        }

        public ICommand DetailCommand
        {
            get;
            private set;
        }

        private void OnSearchExecute()
        {
            Busy = true;
            Parameters parameters = new Parameters();
            parameters.From = From;
            parameters.To = To;
            parameters.DetachmentID = SelectedDetachment.ID;
            parameters.SearchKey = String.IsNullOrWhiteSpace(SearchKey) ? null : SearchKey;
            if (SelectedVehicle != null)
                parameters.VehicleID = SelectedVehicle.ID;
            else
                parameters.VehicleID = 0;

            if (SelectedWorkType != null)
                parameters.TypeID = SelectedWorkType.ID;
            else
                parameters.TypeID = 0;
            fullTextClient = ContainerProvider.GetInstance.Resolve<FullTextServiceClient>();
            fullTextClient.FindWorkCompleted += FindWorkCompleted;
            fullTextClient.FindWorkAsync(parameters);
        }

        private bool OnSearchCanExecute()
        {
            return SelectedDetachment != null;
        }

        private void OnResetExecute()
        {
            Works = null;
            SelectedVehicle = null;
            SelectedWorkType = null;
            SearchKey = null;
            From = DateTime.Now;
            To = DateTime.Now;
        }

        private bool OnResetCanExecute()
        {
            return Works != null || SelectedWorkType != null || SelectedVehicle != null || SearchKey != null;         
        }

        private void OnDetailExecute()
        {
            var n = Navigator.CreateChild();
            n.Startup<FullTextDetailView, FullTextDetailViewModel>(ViewMode.Dialog, null, new ParameterStorage() { Work = SelectedWork });
        }

        private bool OnDetailCanExecute()
        {
            return SelectedWork != null;
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

        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    OnPropertyChanged(() => Visibility);
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

        public Detachment SelectedDetachment
        {
            get { return selectedDetachment; }
            set
            {
                if (selectedDetachment != value)
                {
                    selectedDetachment = value;
                    OnPropertyChanged(() => SelectedDetachment);
                }
            }
        }

        public FullTextWork SelectedWork
        {
            get { return selectedWork; }
            set
            {
                if (selectedWork != value)
                {
                    selectedWork = value;
                    OnPropertyChanged(() => SelectedWork);
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

        public DateTime From
        {
            get { return from; }
            set
            {
                if (from != value)
                {
                    from = value;
                    OnPropertyChanged(() => From);
                }
            }
        }

        public DateTime To
        {
            get { return to; }
            set
            {
                if (to != value)
                {
                    to = value;
                    OnPropertyChanged(() => To);
                }
            }
        }

        public string SearchKey
        {
            get { return searchKey; }
            set
            {
                if (searchKey != value)
                {
                    searchKey = value;
                    OnPropertyChanged(() => SearchKey);
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

        public ObservableCollection<FullTextWork> Works
        {
            get { return works; }
            set
            {
                if (works != value)
                {
                    works = value;
                    OnPropertyChanged(() => Works);
                }
            }
        }

        #endregion
    }
}
