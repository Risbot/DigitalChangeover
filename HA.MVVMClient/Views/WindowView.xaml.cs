using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HA.MVVMClient.Views
{
    /// <summary>
    /// Interaction logic for WindowView.xaml
    /// </summary>
    public partial class WindowView : Window
    {
        public WindowView(BaseViewModel vm)
        {
            InitializeComponent();
            if (vm is VehicleViewModel)
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/train.png", UriKind.RelativeOrAbsolute));
            if (vm is WorkerStateViewModel)
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/state.png", UriKind.RelativeOrAbsolute));
            if (vm is DetachmentViewModel)
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/detachment.png", UriKind.RelativeOrAbsolute));
            if (vm is WorkTypeViewModel)
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/work_type.png", UriKind.RelativeOrAbsolute));
      
            this.DataContext = vm;
            vm.CloseView += (s, e) =>
            {
               
                DataContext = null;
                this.Close();
            };
        }
    }

    public class ListTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null)
            {
                if (item is Vehicle)              
                    return element.FindResource("VehicleTemplate") as DataTemplate;
                if (item is WorkerState)
                    return element.FindResource("WorkerStateTemplate") as DataTemplate;
                if (item is WorkType)
                    return element.FindResource("WorkTypeTemplate") as DataTemplate;
                if (item is Detachment)
                    return element.FindResource("DetachmentTemplate") as DataTemplate;
            }
            return null;
        }
    }

    public class ItemContainerSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element != null && item != null)
            {
                if (item is Vehicle)
                    return element.FindResource("vehicleStyle") as Style;
                else
                    return element.FindResource("otherStyle") as Style;
            }
            return null;
        }
    }
}
