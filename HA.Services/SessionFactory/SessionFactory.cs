using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace HA.Services
{
    public class SessionFactory
    {

        public static IUnitOfWork GetUnitOfWork
        {
            get
            {

                return new UnitOfWork(new Entities());
            } 
        }

     
    }
}
