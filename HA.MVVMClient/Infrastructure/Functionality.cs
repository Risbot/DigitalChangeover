using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.Infrastructure
{
    public static class Functionality
    {
        public static ObservableCollection<T> CloneCollection<T>(ICollection<T> collection)
        {
            var list = new ObservableCollection<T>();
            foreach (var item in collection)
            {
                list.Add(item);
            }
            return list;
        }

        public static bool CompareCollection<T>(ICollection<T> col1, ICollection<T> col2, IEqualityComparer<T> compare) where T : class
        {
            if (col1 == null && col2 == null)
                return true;
            if (col1 == null || col2 == null)
                return false;
            if (col1.Count != col2.Count)
                return false;
            foreach (var item in col1)
            {
                if (!col2.Any(c => compare.Equals(c,item)))
                    return false;
            }
            return true;
        }
       
    }
}
