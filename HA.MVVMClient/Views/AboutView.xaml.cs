using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
            Product.Content = GetAssemblyAttribute<AssemblyProductAttribute>(c => c.Product); 
            Copyright.Content = GetAssemblyAttribute<AssemblyCopyrightAttribute>(c => c.Copyright);
            Version.Content = "Verze " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (ApplicationDeployment.IsNetworkDeployed)
            {              
                Version.Content =  ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
           
        }


        private static string GetAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(T));
            return value.Invoke(attribute);
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            System.Diagnostics.Process.Start(hyperlink.NavigateUri.ToString());
        }
    }
}
