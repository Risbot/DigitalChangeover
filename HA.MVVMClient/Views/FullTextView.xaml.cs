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
    /// Interaction logic for FullTextView.xaml
    /// </summary>
    public partial class FullTextView : Window
    {
        public FullTextView(FullTextViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            vm.CloseView += (s, e) =>
            {
                DataContext = null;
                this.Close();
            };
        }

        private void GridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as FullTextViewModel;
            if (vm != null && vm.DetailCommand.CanExecute(null))
                vm.DetailCommand.Execute(null);
        }
    }
}
