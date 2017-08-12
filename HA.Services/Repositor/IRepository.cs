using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.Services
{   
    public interface IRepository<E> 
    {
        IUnitOfWork UoW { get; }
        E Add(E entity);
        E Update(E entity);
        void Delete(E entity);
        E Single(Func<E, bool> predicate);
        IQueryable<E> Find(Func<E, bool> predicate);
        IQueryable<E> GetAll();      
    }
}
