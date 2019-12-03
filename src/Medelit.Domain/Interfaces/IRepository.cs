using System;
using System.Linq;
using Medelit.Common;

namespace Medelit.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {     
        IQueryable<TEntity> GetAll();      
              
    }
}
