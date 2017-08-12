using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HA.Services
{
    public class Repositor<E> : IRepository<E> where E : class
    {
        #region Constructor

        public Repositor(IUnitOfWork uow)
        {
            UoW = uow;
        }

        #endregion

        #region Property

        public IUnitOfWork UoW
        {
            get;
            private set;
        }

        #endregion

        #region IRepositor

        public E Add(E entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            UoW.Add<E>(entity);
            return entity;
        }

        public E Update(E entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            UoW.Update<E>(entity);
            return entity;
        }

        public void Delete(E entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            UoW.Delete<E>(entity);
        }

        public E Single(Func<E, bool> predicate)
        {
            return UoW.Orm.Set<E>().Single(predicate);
        }

        public IQueryable<E> Find(Func<E, bool> predicate)
        {
            return UoW.Orm.Set<E>().Where(predicate).AsQueryable();
        }

        public IQueryable<E> GetAll()
        {
            return UoW.Orm.Set<E>().AsQueryable();
        }
  
        #endregion

    }
}
