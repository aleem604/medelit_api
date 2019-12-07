using System;
using System.Linq;
using Medelit.Common;

namespace Medelit.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {     
        IQueryable<TEntity> GetAll();
        void Add(TEntity obj);
        TEntity GetById(long id);
        void Update(TEntity obj);
        void Remove(long id);
        int SaveChanges();

    }
}
