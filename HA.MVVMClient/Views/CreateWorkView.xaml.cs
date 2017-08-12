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
    /// Interaction logic for CreateWorkView.xaml
    /// </summary>
    public partial class CreateWorkView : Window
    {
        public CreateWorkView(BaseViewModel vm)
        {
            InitializeComponent();
            if (vm is CreateWorkViewModel)
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/note_add.png", UriKind.RelativeOrAbsolute));
            if (vm is UpdateWorkViewModel)
            {
                Dialog.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/note_edit.png", UriKind.RelativeOrAbsolute));
                OkBtn.Content = "Uložit";
            }
                
            this.DataContext = vm;
            vm.CloseView += (s, e) =>
            {
                DataContext = null;
                this.Close();
            };
        }
    }
}
