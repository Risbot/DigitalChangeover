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

namespace HA.MVVMClient.Views
{
    /// <summary>
    /// Interaction logic for WorkerDialogView.xaml
    /// </summary>
    public partial class WorkerView : Window
    {
        public WorkerView(WorkerViewModel vm)
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
