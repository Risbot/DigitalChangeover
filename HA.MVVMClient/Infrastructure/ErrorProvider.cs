using HA.MVVMClient.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;

namespace HA.MVVMClient.Infrastructure
{
    public class ErrorProvider
    {
        public static void ShowError(Exception exception, INavigator navigator)
        {
            var n = navigator.CreateChild();
            var e = exception as FaultException<DataService.WcfException>;
            //if (exception is CommunicationException)
            //{
            //    n.ShowMessageBox("Chyba komunikace!", "Chyba", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxResult.None, System.Windows.MessageBoxImage.Error);
            //    return; 
            //}
            if (e == null)
            {
                if (n.ShowMessageBox("Nastala neočekávaná chyba!", "Chyba", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxResult.None, System.Windows.MessageBoxImage.Error) == System.Windows.MessageBoxResult.OK)
                    Application.Current.Shutdown();
                return;
            }
            switch(e.Detail.Status)
            {
                case ErrorStatus.DatabaseInfo:
                    n.ShowMessageBox(e.Detail.Message, "Informace", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Information);
                    break;
                case ErrorStatus.DatabaseError:
                    n.ShowMessageBox(e.Detail.Message, "Chyba databáze ", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Error);
                    break;
                case ErrorStatus.SecurityError:
                    n.ShowMessageBox(e.Detail.Message, "Bezpečnostní  chyba", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Error);
                    break;
                case ErrorStatus.UnknowenError:
                    n.ShowMessageBox(e.Detail.Message, "Neznáma chyba", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Error);
                    break;
                case ErrorStatus.ValidationError:
                    n.ShowMessageBox(e.Detail.Message, "Chyba zadaných parametru", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Error);
                    break;
                case ErrorStatus.DateError:
                    n.ShowMessageBox(e.Detail.Message, "Zastaveno", MessageBoxButton.OK, MessageBoxResult.None, MessageBoxImage.Stop);
                    break;
            }   
        }
    }
}
