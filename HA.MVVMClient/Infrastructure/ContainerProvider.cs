using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.Infrastructure
{
    public static class ContainerProvider
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() => new UnityContainer());
      
        public static IUnityContainer GetInstance
        {
            get
            {       
                return container.Value;
            }
        }
    }
}
