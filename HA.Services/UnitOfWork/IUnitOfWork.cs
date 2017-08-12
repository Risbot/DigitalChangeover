using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace HA.Services
{
    public interface IUnitOfWork
    {
        DbContext Orm { get; }
        void Add<E>(E entity) where E : class;
        void Update<E>(E entity) where E : class;
        void Delete<E>(E entity) where E : class;
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
