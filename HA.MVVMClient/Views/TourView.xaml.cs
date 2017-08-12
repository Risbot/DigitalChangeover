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
using Xceed.Wpf.Toolkit;

namespace HA.MVVMClient.Views
{
    /// <summary>
    /// Interaction logic for TourDialogView.xaml
    /// </summary>
    public partial class TourView : Window
    {
        public TourView(TourViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            vm.CloseView += (s, e) =>
            {
                DataContext = null;
                this.Close();
            };
        }
    }
}
