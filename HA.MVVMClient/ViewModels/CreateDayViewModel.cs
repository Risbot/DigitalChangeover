using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class CreateDayViewModel : BaseViewModel
    {
        #region Variables

        private DataServiceClient dataClient;
        private bool isNight;
        private bool isDay;
        private string description;
        private DateTime selectedDate;
        private bool busy;

        #endregion

        #region Constructors

        public CreateDayViewModel(INavigator navigator)
        {
            Busy = false;
            Navigator = navigator;
            SelectedDate = DateTime.Now;
            IsDay = true;
            InitCommands();
            PropertyChanged += (s, e) =>
            {
                (CreateCommand as Command).OnCanExecuteChanged();
            };
        }

        #endregion
   
        #region Events

        void AddDateCompleted(object sender, AddDateCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Date.ID = e.Result;
                OnCloseView();
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.AddDateCompleted -= AddDateCompleted;
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

        private void OnCreateExecute()
        {
            Busy = true;
            Date = new Date()
            {
                DateContent = SelectedDate,
                Description = Description,
                DetachmentID = LoginInit.user.DetachmentID,
                IsNight = IsNight
            };
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.AddDateCompleted += AddDateCompleted;
            dataClient.AddDateAsync(Date);
        }

        private bool OnCreateCanExecute()
        {
            return IsNight != IsDay;
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
            get;
            private set;
        }

        public bool IsNight
        {
            get { return isNight; }      
            set
            {
                if (isNight != value)
                {
                    isNight = value;
                    OnPropertyChanged(() => IsNight); 
                }
            }
        }

        public bool IsDay
        {
            get { return isDay; }
            set
            {
                if (isDay != value)
                {
                    isDay = value;
                    OnPropertyChanged(() => IsDay);
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

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged(() => SelectedDate); 
                }
            }
        }

        #endregion
    }
}
