using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Views;
using System.Windows.Input;
using System.Windows;
using HA.MVVMClient.Infrastructure;
using System.Collections.ObjectModel;
using Microsoft.Practices.Unity;
using System.Globalization;
using System.Threading.Tasks;


namespace HA.MVVMClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<Changeover> changeovers;
        private ObservableCollection<Work> works;
        private ObservableCollection<Attendance> attendances;
        private ObservableCollection<WorkerState> workerStates;
        private YearBaseViewModel tree;
        private Date date;
        private int count;
        private bool workTabSelected, attendanceTabSelected, changeoverTabSelected;
        private DataServiceClient dataClient;
        private Work selectedWork;
        private Changeover selectedChangeover;
        private ObservableCollection<string> topFaultWorks;
        private ObservableCollection<string> topCauseWorks;
        private Visibility toolsVisibility;
        private Visibility administrativeVisibility;
        private bool busy;
        private int busyCount;

        #endregion

        #region Constructors

        public MainWindowViewModel(DataServiceClient dataClient, INavigator navigator)
        {
          
            this.dataClient = dataClient;
            Navigator = navigator;
            OnRefreshTreeExecute();
            workTabSelected = true;
            AdministrativeVisibility = Visibility.Collapsed;
            ToolsVisibility = Visibility.Collapsed;
            if (LoginInit.user.Roles.Any(c => c.Name == "Admin"))
                AdministrativeVisibility = Visibility.Visible;
            if (LoginInit.user.Roles.Any(c => c.Name == "Write"))
                ToolsVisibility = Visibility.Visible;
            InitCommands();
         
            this.PropertyChanged += (s, e) =>
            {
                (PrintDialog as Command).OnCanExecuteChanged();
                (PreviewDialog as Command).OnCanExecuteChanged();
                (NewEntry as Command).OnCanExecuteChanged();
                (DeleteDay as Command).OnCanExecuteChanged();
                (DeleteEntry as Command).OnCanExecuteChanged();
                (AttendanceDialog as Command).OnCanExecuteChanged();
                (TransferEntry as Command).OnCanExecuteChanged();
                (LockDay as Command).OnCanExecuteChanged();
                (UnlockDay as Command).OnCanExecuteChanged();
            };
        }

        #endregion

        #region Events

        void EventSelectedDay(Date date)
        {
            Date = date;
           
            Busy = true;
            dataClient.FindWorksCompleted += FindWorksCompleted;
            dataClient.FindWorksAsync(date.ID, LoginInit.user.DetachmentID);
            dataClient.FindAttendancesCompleted += FindAttendancesCompleted;
            dataClient.FindAttendancesAsync(date.ID, LoginInit.user.DetachmentID);
        }

        void GetTopFaultWorksCompleted(object sender, GetTopFaultWorksCompletedEventArgs e)
        {
            if (e.Error == null)
                topFaultWorks = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.GetTopFaultWorksCompleted -= GetTopFaultWorksCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void GetTopCauseWorksCompleted(object sender, GetTopCauseWorksCompletedEventArgs e)
        {
            if (e.Error == null)
                topCauseWorks = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.GetTopCauseWorksCompleted -= GetTopCauseWorksCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindChangeoversCompleted(object sender, FindChangeoversCompletedEventArgs e)
        {
            if (e.Error == null)
                Changeovers = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindChangeoversCompleted -= FindChangeoversCompleted;
            if (busyCount == 0)
                Busy = false;
            else
                busyCount--;
        }

        void FindAttendancesCompleted(object sender, FindAttendancesCompletedEventArgs e)
        {
            if (e.Error == null)
                Attendances = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindAttendancesCompleted -= FindAttendancesCompleted;
            Busy = false;
        }

        void FindWorksCompleted(object sender, FindWorksCompletedEventArgs e)
        {   
            if (e.Error == null)
                Works = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindWorksCompleted -= FindWorksCompleted;
            Busy = false;
        }

        void DeleteDateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (Changeovers.Any(c => c.DateID == Date.ID))
                    Changeovers.Remove(Changeovers.FirstOrDefault(c => c.DateID == Date.ID));
                Tree.Years.
                    First(c => c.Year == Date.DateContent.Year.ToString()).
                    Months.First(c => c.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.DateContent.Month)).
                    Days.First(c => c.Day == Date).Remove();
                if (Tree.Years.Count != 0)
                    Tree.Years.Last().Months.Last().Days.Last().IsSelected = true;
                else
                {
                    Date = null;
                    if (Works !=null)
                        Works.Clear();
                    if (Attendances != null)
                        Attendances.Clear();
                }
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteDateCompleted -= DeleteDateCompleted;
            Busy = false;
        }
   
        void UpdateDateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Date.IsClosed = Date.IsClosed = !Date.IsClosed;
                ErrorProvider.ShowError(e.Error, Navigator);
            }           
            dataClient.UpdateDateCompleted -= UpdateDateCompleted;
            Busy = false;
        }

        void DeleteChangeoverCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Changeovers.Remove(SelectedChangeover);
                SelectedChangeover = null;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteChangeoverCompleted -= DeleteChangeoverCompleted;
            Busy = false;
        }

        void DeleteWorkCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Works.Remove(SelectedWork);
                SelectedWork = null;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteWorkCompleted -= DeleteWorkCompleted;
            Busy = false;
        }

        void TransferChangeoverCompleted(object sender, TransferChangeoverCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (Works != null)
                    Works.Add(e.Result);
                Changeovers.Remove(SelectedChangeover);
                SelectedChangeover = null;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.TransferChangeoverCompleted -= TransferChangeoverCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            NewEntry = new Command(OnNewEntryExecute, OnNewEntryCanExecute);

            TransferEntry = new Command(OnTransferEntryExecute, OnTransferEntryCanExecute);

            DeleteEntry = new Command(OnDeleteEntryExecute, OnDeleteEntryCanExecute);

            LockDay = new Command(OnLockDayExecute, OnLockDayCanExecute);

            UnlockDay = new Command(OnUnlockDayExecute, OnUnlockDayCanExecute);

            RefreshTree = new Command(OnRefreshTreeExecute);

            EditWorkCommand = new Command((p) => 
            {
                var n = Navigator.CreateChild();
                n.Startup<CreateWorkView, UpdateWorkViewModel>(ViewMode.Dialog, null , new ParameterStorage { NWork=(Work)p, Date = Date, TopFaultWorks = topFaultWorks, TopCauseWorks = topCauseWorks}); 
            }, (p) => { return SelectedWork != null; });

            Exit = new Command(() =>
            {
                Application.Current.Shutdown();
            });

            HelpView = new Command(() =>
            {
                System.Windows.Forms.Help.ShowHelp(null, @"Help.chm");
            });

            PreviewDialog = new Command(() =>
            {
                try
                {
                    var n = Navigator.CreateChild();
                    dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                    Works = dataClient.FindWorks(Date.ID);
                    Attendances = dataClient.FindAttendances(Date.ID);
                    Changeovers = dataClient.FindChangeovers(LoginInit.user.DetachmentID);
                    n.ShowPreview(Date, Works.ToList(), Attendances.ToList(), Changeovers.ToList());
                }
                catch (Exception e)
                {
                    ErrorProvider.ShowError(e, Navigator); 
                }
            }, () => { return Date != null; });


            PrintDialog = new Command(() =>
            {
                try
                {
                    var n = Navigator.CreateChild();
                    dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                    Works = dataClient.FindWorks(Date.ID);
                    Attendances = dataClient.FindAttendances(Date.ID);
                    Changeovers = dataClient.FindChangeovers(LoginInit.user.DetachmentID);
                    n.ShowPrint(Date, Works.ToList(), Attendances.ToList(), Changeovers.ToList());
                }
                catch (Exception e)
                {
                    ErrorProvider.ShowError(e, Navigator);
                } 
            }, () => { return Date != null; });

            AccountDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<AccountView, AccountViewModel>(ViewMode.Dialog);
            });

            WorkerStateDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<WindowView, WorkerStateViewModel>(ViewMode.Dialog);
            });

            DetachmentDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<WindowView, DetachmentViewModel>(ViewMode.Dialog);
            },
            () => { return LoginInit.user.Roles.Any(c => c.Name == "Admin"); });

            UserDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<UserView, UserViewModel>(ViewMode.Dialog);
            },
            () => { return LoginInit.user.Roles.Any(c => c.Name == "Admin"); });

            CreateDayDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<CreateDayView, CreateDayViewModel>(ViewMode.Dialog, (s, e) =>
                {
                    OnCreateDayExecute(s, e);
                });
            },
            () => { return LoginInit.user.Roles.Any(c => c.Name == "Write"); });

            DeleteDay = new Command(() =>
            {
                var n = Navigator.CreateChild();
                if (n.ShowMessageBox("Jste si jistí že chcete smazat " + (Date.IsNight ? "noční" : " denní") + " směnu, dne " + Date.DateContent.ToString("D", CultureInfo.CreateSpecificCulture("cs-CZ")) + "?\nSmažou se i veškerá data obsažená v této směně!", "Varováni", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Busy = true;
                    dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                    dataClient.DeleteDateCompleted += DeleteDateCompleted;
                    dataClient.DeleteDateAsync(Date.ID);
                }
            }, OnDeleteDayCanExecute);

            VehicleDialog = new Command(() => 
            {
                var n = Navigator.CreateChild();
                n.Startup<WindowView, VehicleViewModel>(ViewMode.Dialog);
            });
            TourDialog = new Command(() => 
            {
                var n = Navigator.CreateChild();
                n.Startup<TourView, TourViewModel>(ViewMode.Dialog);
            });
            WorkerDialog = new Command(() => 
            {
                var n = Navigator.CreateChild();
                n.Startup<WorkerView, WorkerViewModel>(ViewMode.Dialog);
            });
            FullTextDialog = new Command(() => 
            {
                var n = Navigator.CreateChild();
                n.Startup<FullTextView, FullTextViewModel>(ViewMode.Dialog);
            });
            WorkTypeDialog = new Command(() => 
            {
                var n = Navigator.CreateChild();
                n.Startup<WindowView, WorkTypeViewModel>(ViewMode.Dialog);
            });

            AttendanceDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<AttendanceView, AttendanceViewModel>(ViewMode.Dialog, null, new ParameterStorage { Date = Date, Attendances = this.Attendances });
            }, OnAttendanceDialogCanExecute);

            AboutDialog = new Command(() =>
            {
                var n = Navigator.CreateChild();
                n.Startup<AboutView>();
            });
        }

        public ICommand RefreshTree
        {
            get;
            private set;
        }

        public ICommand HelpView
        {
            get;
            private set;
        }

        public ICommand LockDay
        {
            get;
            private set;
        }

        public ICommand UnlockDay
        {
            get;
            private set;
        }

        public ICommand PrintDialog
        {
            get;
            private set;
        }

        public ICommand PreviewDialog
        {
            get;
            private set;
        }
 
        public ICommand TransferEntry
        {
            get;
            private set;
        }

        public ICommand DeleteEntry
        {
            get;
            private set;
        }

        public ICommand AttendanceDialog
        {
            get;
            private set;
        }

        public ICommand DeleteDay
        {
            get;
            private set;
        }

        public ICommand AboutDialog
        {
            get;
            private set;
        }

        public ICommand AccountDialog
        {
            get;
            private set;
        }

        public ICommand WorkerStateDialog
        {
            get;
            private set;
        }

        public ICommand UserDialog
        {
            get;
            private set;
        }

        public ICommand DetachmentDialog
        {
            get;
            private set;
        }

        public ICommand CreateDayDialog
        {
            get;
            private set;
        }

        public ICommand VehicleDialog
        {
            get;
            private set;
        }

        public ICommand TourDialog
        {
            get;
            private set;
        }

        public ICommand WorkerDialog
        {
            get;
            private set;
        }

        public ICommand FullTextDialog
        {
            get;
            private set;
        }

        public ICommand WorkTypeDialog
        {
            get;
            private set;
        }

        public ICommand Exit
        {           
            get;
            private set;
        }

        public ICommand NewEntry
        {
            get;
            private set;
        }

        public ICommand EditWorkCommand
        {
            get;
            private set;
        }

        private async void OnRefreshTreeExecute()
        {
            Busy = true;
            busyCount = 3;
            Tree = await LoadDates();
            Tree.EventSelectedDay += EventSelectedDay;
            if (Tree.Years.Count != 0)
                Tree.Years.Last().Months.Last().Days.Last().IsSelected = true;
            else
                busyCount--;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            this.dataClient.FindChangeoversCompleted += FindChangeoversCompleted;
            this.dataClient.FindChangeoversAsync(LoginInit.user.DetachmentID);
            this.dataClient.GetTopFaultWorksCompleted += GetTopFaultWorksCompleted;
            this.dataClient.GetTopFaultWorksAsync(100, LoginInit.user.DetachmentID);
            this.dataClient.GetTopCauseWorksCompleted += GetTopCauseWorksCompleted;
            this.dataClient.GetTopCauseWorksAsync(100, LoginInit.user.DetachmentID);
          
        }

        private Task<YearBaseViewModel> LoadDates()
        {
            return Task.Factory.StartNew(() =>
            {
                return new YearBaseViewModel(dataClient);
            });
        }

        private void OnCreateDayExecute(object sender, EventArgs e)
        {
            var vm = sender as CreateDayViewModel;
            YearViewModel year = Tree.Years.FirstOrDefault(c => c.Year == vm.SelectedDate.Year.ToString());
            MonthViewModel month = null;
            DayViewModel day = null;

            if (year == null)
                Tree.Years.Add(year = new YearViewModel(vm.Date.DateContent.Year, Tree, dataClient));
            else
            {
                month = year.Months.FirstOrDefault(c => c.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(vm.Date.DateContent.Month));
                if (month == null)
                    year.Months.Add(month = new MonthViewModel(vm.Date.DateContent.Year, vm.Date.DateContent.Month, year, dataClient));
                else
                {
                    month.Days.Add(new DayViewModel(vm.Date, month));
                }
            }
            year = Tree.Years.FirstOrDefault(c => c.Year == vm.Date.DateContent.Year.ToString());
            month = year.Months.FirstOrDefault(c => c.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(vm.Date.DateContent.Month));
            day = month.Days.FirstOrDefault(c => c.Day.ID == vm.Date.ID);
            day.IsSelected = true;
        }

        private bool OnDeleteDayCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return Date != null && !Date.IsClosed;
        }

        private void OnLockDayExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.UpdateDateCompleted += UpdateDateCompleted;
            Date.IsClosed = true;
            dataClient.UpdateDateAsync(Date);
        }

        private bool OnLockDayCanExecute()
        {
            if (Date == null || Date.IsClosed) 
                return false;
            return LoginInit.user.Roles.Any(c => c.Name == "Write");
        }

        private void OnUnlockDayExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.UpdateDateCompleted += UpdateDateCompleted;
            Date.IsClosed = false;
            dataClient.UpdateDateAsync(Date);
        }

        private bool OnUnlockDayCanExecute()
        {
            if (Date == null || !Date.IsClosed)
                return false;
            return LoginInit.user.Roles.Any(c => c.Name == "Admin");
        }

        private void OnNewEntryExecute()
        {
            if (WorkTabSelected)
            {
                var n = Navigator.CreateChild();
                n.Startup<CreateWorkView, CreateWorkViewModel>(ViewMode.Dialog, (s, e) =>
                {
                    var vm = s as CreateWorkViewModel;
                    this.Works.Add(vm.Work);
                }, new ParameterStorage { Date = Date, TopFaultWorks = topFaultWorks, TopCauseWorks = topCauseWorks });
            }
            if (ChangeoverTabSelected)
            {
                var n = Navigator.CreateChild();
                n.Startup<CreateWorkView, CreateChangeoverViewModel>(ViewMode.Dialog, (s, e) =>
                {
                    var vm = s as CreateChangeoverViewModel;
                    this.Changeovers.Add(vm.Changeover);
                }, new ParameterStorage { Date = Date, TopFaultWorks = topFaultWorks }); 
            }
         
        }

        private bool OnNewEntryCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return (WorkTabSelected || ChangeoverTabSelected) && Date != null;
        }

        private void OnDeleteEntryExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            if (WorkTabSelected)
            {
                dataClient.DeleteWorkCompleted += DeleteWorkCompleted;
                dataClient.DeleteWorkAsync(SelectedWork.ID);
            }
            if (ChangeoverTabSelected)
            {
                dataClient.DeleteChangeoverCompleted += DeleteChangeoverCompleted;
                dataClient.DeleteChangeoverAsync(SelectedChangeover.ID);
            } 
        }

        private bool OnDeleteEntryCanExecute()
        {
            return SelectedChangeover !=null || SelectedWork != null && !Date.IsClosed;
        }

        private void OnTransferEntryExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.TransferChangeoverCompleted += TransferChangeoverCompleted;
            dataClient.TransferChangeoverAsync(Date.ID, SelectedChangeover.ID);
        }

        private bool OnTransferEntryCanExecute()
        {
            return SelectedChangeover != null;
        }

        private bool OnAttendanceDialogCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return AttendanceTabSelected && Date != null;
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

        public Visibility AdministrativeVisibility
        {
            get { return administrativeVisibility; }
            set
            {
                if (administrativeVisibility != value)
                {
                    administrativeVisibility = value;
                    OnPropertyChanged(() => AdministrativeVisibility);
                }
            }
        }

        public Visibility ToolsVisibility
        {
            get { return toolsVisibility; }
            set
            {
                if (toolsVisibility != value)
                {
                    toolsVisibility = value;
                    OnPropertyChanged(() => ToolsVisibility);
                }
            }
        }

    
        public YearBaseViewModel Tree
        {
            get { return tree; }
            set
            {
                if (tree != value)
                {
                    tree = value;
                    OnPropertyChanged(() => Tree);
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

        public bool WorkTabSelected
        {
            get { return workTabSelected; }
            set
            {
                if (workTabSelected != value)
                {
                    workTabSelected = value;
                    SelectedChangeover = null;
                    OnPropertyChanged(() => WorkTabSelected);
                }
            }
        }

        public bool AttendanceTabSelected
        {
            get { return attendanceTabSelected; }
            set
            {
                if (attendanceTabSelected != value)
                {
                    attendanceTabSelected = value;
                    SelectedChangeover = null;
                    SelectedWork = null;
                    OnPropertyChanged(() => AttendanceTabSelected);
                }
            }
        }

        public bool ChangeoverTabSelected
        {
            get { return changeoverTabSelected; } 
            set
            {
                if (changeoverTabSelected != value)
                {
                    changeoverTabSelected = value;
                    SelectedWork = null;
                    OnPropertyChanged(() => ChangeoverTabSelected);
                }
            }
        }

        public int Count
        {
            get { return count; }          
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged(() => Count);
                }
            }
        }

        public ObservableCollection<Changeover> Changeovers
        {
            get { return changeovers; }
            set
            {
                if (changeovers != value)
                {
                    changeovers = value;
                    OnPropertyChanged(() => Changeovers);
                }
            }
        }

        public ObservableCollection<Work> Works
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

        public Changeover SelectedChangeover
        {
            get { return selectedChangeover; }
            set
            {
                if (selectedChangeover != value)
                {
                    selectedChangeover = value;
                    OnPropertyChanged(() => SelectedChangeover);
                }
            }
        }

        public Work SelectedWork
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
        
        #endregion
    }
}
