using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.Views;
using HA.MVVMClient.ViewModels;
using HA.MVVMClient.DataService;
using HA.MVVMClient.FullTextService;
using Microsoft.Practices.Unity;
using HA.MVVMClient.SecurityService;

namespace HA.MVVMClient
{
    
    public class GetterLifetimeManager : TransientLifetimeManager
    {
        private Func<object> factory;

        public GetterLifetimeManager(Func<object> factory)
        {
            this.factory = factory;
        }

        public override object GetValue()
        {
            return factory();
        }
    }


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);  
            IUnityContainer container = ContainerProvider.GetInstance;
            container.RegisterType<INavigator, Navigator>();
            container.RegisterType<DataServiceClient, DataServiceClient>(
                new GetterLifetimeManager(
                () =>
                {
                    var dataClient = new DataServiceClient();
                    dataClient.ClientCredentials.UserName.UserName = LoginInit.user.Name;
                    dataClient.ClientCredentials.UserName.Password = LoginInit.user.Password;
                    dataClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode =  System.ServiceModel.Security.X509CertificateValidationMode.None;
                    return dataClient;
                }));

            container.RegisterType<SecurityServiceClient, SecurityServiceClient>(
               new GetterLifetimeManager(
               () =>
               {
                   var securityClient = new SecurityServiceClient();
                   securityClient.ClientCredentials.UserName.UserName = LoginInit.user.Name;
                   securityClient.ClientCredentials.UserName.Password = LoginInit.user.Password;
                   securityClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                   return securityClient;
               }));

            container.RegisterType<FullTextServiceClient, FullTextServiceClient>(
              new GetterLifetimeManager(
              () =>
              {
                  var fullTextClient = new FullTextServiceClient();
                  fullTextClient.ClientCredentials.UserName.UserName = LoginInit.user.Name;
                  fullTextClient.ClientCredentials.UserName.Password = LoginInit.user.Password;
                  fullTextClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                  return fullTextClient;
              }));
        
            var navigator = container.Resolve<INavigator>();

            navigator.Startup<LoginView, LoginViewModel>(ViewMode.Window);
        }
    }
}
