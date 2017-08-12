using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.DataService;
using System.Collections.ObjectModel;
using HA.MVVMClient.Infrastructure;
using Microsoft.Practices.Unity;


namespace HA.MVVMClient.ViewModels
{
    public class YearViewModel : BaseViewModel
    {      
        #region Variables

        private Int32 year;
        private ObservableCollection<MonthViewModel> months;
        private bool isExpanded;

        #endregion

        #region Constructors

        public YearViewModel(Int32 year, YearBaseViewModel parent, DataServiceClient dataClient)
        {
            Parent = parent;
            this.year = year;
            Months = new ObservableCollection<MonthViewModel>(dataClient.GetMonths(year, LoginInit.user.DetachmentID).Select(c => new MonthViewModel(this.year, c, this, dataClient)));
        }

        #endregion

        #region Functions

        public void Remove()
        {
            Parent.Years.Remove(this);
        }

        #endregion

        #region Properties

        public YearBaseViewModel Parent
        {
            get;
            private set;
        }

        public string Year
        {
            get { return year.ToString(); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    OnPropertyChanged(() => IsExpanded);
                }
            }
        }

        public ObservableCollection<MonthViewModel> Months
        {
            get { return months; }
            set
            {
                if (months != value)
                {
                    months = value;
                    OnPropertyChanged(() => Months);
                }
            }
        }

        #endregion
    }
}
