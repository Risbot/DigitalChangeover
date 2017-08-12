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
using HA.MVVMClient.ViewModels;
using HA.MVVMClient.Infrastructure;

namespace HA.MVVMClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(MainWindowViewModel vm)
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            this.DataContext = vm; 
        }
    }
}
