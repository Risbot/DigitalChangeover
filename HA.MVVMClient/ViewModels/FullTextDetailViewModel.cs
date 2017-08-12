using HA.MVVMClient.DataService;
using HA.MVVMClient.FullTextService;
using HA.MVVMClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModels
{
    public class FullTextDetailViewModel : BaseViewModel
    {    
        #region Variables

        private FullTextWork work;
        private DateTime date;
        private string dateDescription;
        private bool dateIsNight;
        private string workTypeName;
        private string workTypeDescription;
        private string vehicleNumber;
        private string vehicleDescription;
        private DataServiceClient dataClient;
        private string workFaultDescription;
        private string workCauseDescription;
        private ObservableCollection<Attendance> attendances;

        #endregion

        #region Constructors

        public FullTextDetailViewModel(DataServiceClient dataClient, INavigator navigator)
        {
            Navigator = navigator;
            this.dataClient = dataClient;
            Work = Navigator.Parameters.Work;
            Date = Work.DateContent;
            DateDescription = Work.DateDescriptor;
            DateIsNight = Work.DateIsNight;
            VehicleDescription = Work.VehicleDescription;
            VehicleNumber = Work.VehicleNumber;
            WorkTypeDescription = Work.WorkTypeDescription;
            WorkTypeName = Work.WorkTypeName;
            WorkFaultDescription = Work.FaultDescription;
            WorkCauseDescription = Work.CauseDescription;
            this.dataClient.FindAttendancesCompleted += FindAttendancesCompleted;
            this.dataClient.FindAttendancesAsync(Work.DateID, LoginInit.user.DetachmentID);
        }

        #endregion

        #region Events

        void FindAttendancesCompleted(object sender, FindAttendancesCompletedEventArgs e)
        {
            if (e.Error == null)
                Attendances = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindAttendancesCompleted -= FindAttendancesCompleted;
        }

        #endregion

        #region Properties

        public ObservableCollection<Attendance> Attendances
        {
            get
            {
                return attendances;
            }
            set
            {
                if (attendances != value)
                {
                    attendances = value;
                    OnPropertyChanged(() => Attendances);
                }
            }
        }

        public FullTextWork Work
        {
            get { return work; }
            set
            {
                if (work != value)
                {
                    work = value;
                    OnPropertyChanged(() => Work);
                }
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged(() => Date);
                }
            }
        }

        public bool DateIsNight
        {
            get
            {
                return dateIsNight;
            }
            set
            {
                if (dateIsNight != value)
                {
                    dateIsNight = value;
                    OnPropertyChanged(() => DateIsNight);
                }
            }
        }

        public string DateDescription
        {
            get
            {
                return dateDescription;
            }
            set
            {
                if (dateDescription != value)
                {
                    dateDescription = value;
                    OnPropertyChanged(() => DateDescription);
                }
            }
        }

        public string WorkTypeName
        {
            get
            {
                return workTypeName;
            }
            set
            {
                if (workTypeName != value)
                {
                    workTypeName = value;
                    OnPropertyChanged(() => WorkTypeName);
                }
            }
        }

        public string WorkTypeDescription
        {
            get
            {
                return workTypeDescription;
            }
            set
            {
                if (workTypeDescription != value)
                {
                    workTypeDescription = value;
                    OnPropertyChanged(() => WorkTypeDescription);
                }
            }
        }

        public string VehicleNumber
        {
            get
            {
                return vehicleNumber;
            }
            set
            {
                if (vehicleNumber != value)
                {
                    vehicleNumber = value;
                    OnPropertyChanged(() => VehicleNumber);
                }
            }
        }

        public string VehicleDescription
        {
            get
            {
                return vehicleDescription;
            }
            set
            {
                if (vehicleDescription != value)
                {
                    vehicleDescription = value;
                    OnPropertyChanged(() => VehicleDescription);
                }
            }
        }

        public string WorkFaultDescription
        {
            get { return workFaultDescription; }
            set
            {
                if (workFaultDescription != value)
                {
                    workFaultDescription = value;
                    OnPropertyChanged(() => WorkFaultDescription);
                }
            }
        }

        public string WorkCauseDescription
        {
            get { return workCauseDescription; }
            set
            {
                if (workCauseDescription != value)
                {
                    workCauseDescription = value;
                    OnPropertyChanged(() => WorkCauseDescription);
                }
            }
        }

        #endregion
    }
}
