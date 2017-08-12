using HA.MVVMClient.DataService;
using HA.MVVMClient.SecurityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.Infrastructure
{
    public class CompareRole : IEqualityComparer<Role>
    {
        public bool Equals(Role x, Role y)
        {
            if (x.ID == y.ID)
                return true;
            return false;
        }

        public int GetHashCode(Role obj)
        {
            return obj.GetHashCode();
        }
    }

    public class CompareTour : IEqualityComparer<Tour>
    {
        public bool Equals(Tour x, Tour y)
        {
            if (x.ID == y.ID)
                return true;
            return false;
        }

        public int GetHashCode(Tour obj)
        {
            return obj.GetHashCode();
        }
    }
}
