using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.Infrastructure;
using System.Globalization;
using System.Windows;
using HA.MVVMClient.DataService;

namespace HA.MVVMClient.ViewModels
{
    public class DayViewModel : BaseViewModel
    {
        #region Variables

        private Date day;
        private bool isSelected;

        #endregion

        #region Constructors

        public DayViewModel(Date day, MonthViewModel parent)
        {
            this.day = day;
            Parent = parent;
        }

        #endregion

        #region Functions

        public void Remove()
        {
            Parent.Days.Remove(this);
            if (Parent.Days.Count == 0)
                Parent.Remove();
        }

        #endregion

        #region Properties

        public MonthViewModel Parent
        {
            get;
            private set;
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    if (IsSelected)
                    {
                        Parent.IsExpanded = true;
                        Parent.Parent.Parent.OnSelectedDay(day);
                    }
                    OnPropertyChanged(() => IsSelected);
                }
            }
        }

        public Date Day
        {
            get { return day; }
        }

        #endregion
    }
}
