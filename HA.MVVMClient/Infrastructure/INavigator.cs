using HA.MVVMClient.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HA.MVVMClient.Infrastructure
{
    public interface INavigator
    {

        Window View
        {
            get;
        }

        ParameterStorage Parameters
        {
            get;
        }

        INavigator Parent
        {
            get;
        }

        MessageBoxResult ShowMessageBox(string messageText, string caption, MessageBoxButton button, MessageBoxResult defaultButton, MessageBoxImage icon);
        void Startup<T, VM>(ViewMode mode, EventHandler close = null, ParameterStorage parameters = null) where T : class where VM : class;
        void Startup<T>() where T : class;
        INavigator CreateChild();
        void ShowPrint(Date date, List<Work> works, List<Attendance> attendances, List<Changeover> changeovers);
        void ShowPreview(Date date, List<Work> works, List<Attendance> attendances, List<Changeover> changeovers);
    }

    public enum ViewMode
    {
        Window,
        Dialog
    }
}
