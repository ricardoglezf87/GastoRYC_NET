using Dapper;

using GARCA.Data.Managers;
using GARCA.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Data.Services
{   
    public class IServiceCache<TEntity> where TEntity : ModelBase<Int32>
    {
        protected ObjectCache oCache = MemoryCache.Default;
        private readonly DbContext context;
        
        private DbSet<TEntity> Entities { get; }

        public IServiceCache()
        {
            context = new RycContext();
            Entities = context.Set<TEntity>();
            FillCache();
        }

        public IEnumerable<TEntity> GetAll()
        {
            if (oCache[GetType().Name] == null)
            {
                FillCache();
            }

            return (IEnumerable<TEntity>)oCache[GetType().Name];
        }

        public TEntity? GetById(int? id)
        {
            return GetAll()?.First(x => x.Id == (id ?? -99));
        }

        public virtual TEntity Update(TEntity entity)
        {
            return null;
            return Entities.Update(entity).Entity;
        }

        public virtual void Delete(TEntity entity)
        {
          //  Entities.Remove(entity);
        }

        public void SaveChanges()
        {
            //context.SaveChangesAsync();
        }

        protected void FillCache()
        {            
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60)                
            };

            oCache.Add(this.GetType().Name, GetAllCache() ?? new HashSet<TEntity>(), policy);
        }

        protected virtual IEnumerable<TEntity>? GetAllCache()
        {
            return null;
        }
    }
}
