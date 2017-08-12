using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HA.Services
{
    [ServiceContract]
    interface IFullTextService
    {
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.FullTextWork> FindWork (Parameters parameters);
    }
}
