using GARCA.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Repositories
{
    public class Repository<TEntity> where TEntity : ModelBaseDAO
    {
        private readonly DbContext context;

        public DbSet<TEntity> Entities { get; }

        public Repository(DbContext dbContext)
        {
            context = dbContext;
            Entities = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Entities;
        }

        public TEntity Update(TEntity entity)
        {
            return Entities.Update(entity).Entity;
        }

        public void Delete(TEntity entity)
        {
            Entities.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public TEntity? GetWithInclude(int? id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Entities.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.FirstOrDefault(e => e.id == id);
        }

        public HashSet<TEntity> GetAllWithInclude(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Entities.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.ToHashSet();
        }
    }

}
