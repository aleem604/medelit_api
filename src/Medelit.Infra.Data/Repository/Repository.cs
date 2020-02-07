using System;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly MedelitContext Db;
        public readonly IMediatorHandler _bus;
        protected readonly DbSet<TEntity> DbSet;
        private IHttpContextAccessor _httpContext;

        public Repository(MedelitContext context, IHttpContextAccessor httpContext, IMediatorHandler bus)
        {
            Db = context;
            _bus = bus;
            DbSet = Db.Set<TEntity>();
            _httpContext = httpContext;
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }

        public virtual TEntity GetById(long id)
        {
            return DbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual void Remove(long id)
        {
            DbSet.Remove(DbSet.Find(id));
        }

        public int SaveChanges()
        {
            return Db.SaveChanges();
        }

        public AuthClaims CurrentUser
        {
            get
            {
                return _httpContext.HttpContext.Items.Where(x => x.Key.Equals(eTinUser.TinUser)).FirstOrDefault().Value as AuthClaims;
            }
        }

        public void HandleResponse(Type type, object result)
        {
            if(result is null)
            {
                _bus.RaiseEvent(new DomainNotification(type.Name, "data can't be null."));
                return;
            }
        }


        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
