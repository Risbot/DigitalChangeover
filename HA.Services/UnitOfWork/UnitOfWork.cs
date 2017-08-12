using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace HA.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope transactionScope;

        public UnitOfWork(DbContext orm)
        {
            Orm = orm;
        }

        public DbContext Orm
        {
            get;
            private set;
        }

        public void Add<E>(E entity) where E : class
        {
              Orm.Set<E>().Add(entity);
        }

        public void Update<E>(E entity) where E : class
        {     
            DbEntityEntry<E> entry = Orm.Entry(entity);
            Orm.Set<E>().Attach(entity);
            entry.State = EntityState.Modified;   
        }

        public void Delete<E>(E entity) where E : class
        {         
            Orm.Set<E>().Remove(entity);
        }

        public void BeginTransaction()
        {         
            transactionScope = new TransactionScope();
        }

        public void CommitTransaction()
        {         
            if (transactionScope == null)
            {
                throw new TransactionException("Aktuální transakce není zahájena!");
            }
            Orm.SaveChanges();
            transactionScope.Complete();
            transactionScope.Dispose();
        }

        public void RollbackTransaction()
        {  
            if (transactionScope != null)
            {
                transactionScope.Dispose();
            } 
        }
    }
}
