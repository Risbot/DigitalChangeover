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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel vm)
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
