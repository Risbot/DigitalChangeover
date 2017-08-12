using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

using System.IO;
using HA.MVVMClient.DataService;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.Packaging;

namespace HA.MVVMClient.Infrastructure
{

    public class Navigator : INavigator
    {
        public Navigator()
        {
            Parent = null;
        }

        public INavigator Parent
        {
            get;
            private set;
        }

        public Window View
        {
            get;
            private set;
        }

        public INavigator CreateChild()
        {
            return new Navigator()
            {
                Parent = this
            };
        }

        public ParameterStorage Parameters
        {
            private set;
            get;
        }

       
        public MessageBoxResult ShowMessageBox(string messageText, string caption =  "Chyba", MessageBoxButton button = MessageBoxButton.OK, MessageBoxResult defaultButton = MessageBoxResult.None, MessageBoxImage icon = MessageBoxImage.Error)
        {
            if (Parent != null)
                return Xceed.Wpf.Toolkit.MessageBox.Show(Parent.View, messageText, caption, button, icon, defaultButton);
            else
                return Xceed.Wpf.Toolkit.MessageBox.Show(messageText, caption, button, icon, defaultButton);
        }


        public void ShowPrint(Date date, List<Work> works, List<Attendance> attendances, List<Changeover> changeovers)
        {
            PrintDialog print = new PrintDialog();
            FixedDocument document = CreateDocument.Create(date, works, attendances, changeovers);
            print.UserPageRangeEnabled = true;    
            if (print.ShowDialog() == true)
            {
                print.PrintDocument(document.DocumentPaginator, "Výkaz práce");
            }
        }


        public void ShowPreview(Date date, List<Work> works, List<Attendance> attendances, List<Changeover> changeovers)
        {
            PrintPrewiev prewiev = new PrintPrewiev()
            {
                Document = CreateDocument.Create(date, works, attendances, changeovers)
            };
            prewiev.Description = "Výkaz práce";
            (prewiev.Template.FindName("PART_FindToolBarHost", prewiev) as ContentControl).Visibility = Visibility.Collapsed;
            var window = new Window();
            window.Owner = Parent.View;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Content = prewiev;
            window.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/preview.png", UriKind.RelativeOrAbsolute));
            window.Show();
        }

        public void Startup<T>()
            where T : class
        {
            View = ContainerProvider.GetInstance.Resolve<T>() as Window;
            if (Parent != null)
                View.Owner = Parent.View;
            if (View != null)
                View.ShowDialog();
        }

       
        public void Startup<T, VM>(ViewMode mode, EventHandler close = null, ParameterStorage parameters = null)
            where T : class 
            where VM : class
        {
            Parameters = parameters;
            View = ContainerProvider.GetInstance.Resolve<T>(new ParameterOverride
            ("vm", ContainerProvider.GetInstance.Resolve<VM>(new ParameterOverride("navigator", this)))) as Window;
            if (close != null)
                (View.DataContext as BaseViewModel).CloseView += close;
            if (Parent != null)
            {
                View.Owner = Parent.View;
            }
            if (View != null)
            {
                if (mode == ViewMode.Window)
                    View.Show();
                else
                    View.ShowDialog();
            }
        }
    }
}
