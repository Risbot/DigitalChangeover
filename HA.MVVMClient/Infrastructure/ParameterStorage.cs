using HA.MVVMClient.DataService;
using HA.MVVMClient.FullTextService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.Infrastructure
{
    public class ParameterStorage
    {
        public Date Date
        {
            get;
            set;
        }

        public FullTextWork Work
        {
            get;
            set;
        }

        public Work NWork
        {
            get;
            set;
        }

        public ObservableCollection<Attendance> Attendances
        {
            get;
            set;
        }

        public ObservableCollection<string> TopFaultWorks
        {
            get;
            set;
        }

        public ObservableCollection<string> TopCauseWorks
        {
            get;
            set;
        }
    }
}
