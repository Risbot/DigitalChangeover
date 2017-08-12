using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{  
    public class YearBaseViewModel : BaseViewModel
    {
        #region Variables

        private ObservableCollection<YearViewModel> years;

        #endregion

        #region Constructors

        public YearBaseViewModel(DataServiceClient dataClient)
        {
            Years = new ObservableCollection<YearViewModel>(dataClient.GetYears(LoginInit.user.DetachmentID).Select(c => new YearViewModel(c, this, dataClient)));
        }

        #endregion

        #region Events

        public delegate void SelectedDay(Date date);
        public event SelectedDay EventSelectedDay;

        public void OnSelectedDay(Date date)
        {
            if (EventSelectedDay != null)
            {
                EventSelectedDay(date);
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<YearViewModel> Years
        {
            get { return years; }
            set
            {
                if (years != value)
                {
                    years = value;
                    OnPropertyChanged(() => Years);
                }
            }
        }

        #endregion
    }   
}
