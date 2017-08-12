using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HA.MVVMClient.Infrastructure;
using System.Globalization;
using HA.MVVMClient.DataService;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class MonthViewModel : BaseViewModel
    {
        #region Variables

        private Int32 month;
        private ObservableCollection<DayViewModel> days;
        private bool isExpanded;

        #endregion

        #region Constructors

        public MonthViewModel(Int32 year, Int32 month, YearViewModel parent, DataServiceClient dataClient)
        {
            Parent = parent;
            this.month = month;
            Days = new ObservableCollection<DayViewModel>(dataClient.FindDates(year, month, LoginInit.user.DetachmentID).Select(c => new DayViewModel(c, this)));
        }

        #endregion

        #region Functions

        public void Remove()
        {
            Parent.Months.Remove(this);
            if (Parent.Months.Count == 0)
                Parent.Remove();
        }

        #endregion

        #region Properties

        public YearViewModel Parent
        {
            get;
            private set;
        }

        public string Month
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    Parent.IsExpanded = isExpanded = value;
                    OnPropertyChanged(() => IsExpanded);
                }
            }
        }

        public ObservableCollection<DayViewModel> Days
        {
            get { return days; }
            set
            {
                if (days != value)
                {
                    days = value;
                    OnPropertyChanged(() => Days);
                }
            }
        }

        #endregion
    }
}
