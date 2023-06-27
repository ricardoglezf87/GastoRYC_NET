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
        private readonly DbSet<TEntity> _entities;

        public DbSet<TEntity> entities => _entities;

        public Repository(DbContext dbContext)
        {
            context = dbContext;
            _entities = context.Set<TEntity>();
        }

        public TEntity? GetById(int? id)
        {
            return _entities.Find(id);
        }

        public List<TEntity> GetAll()
        {
            return _entities.ToList();
        }

        public TEntity Add(TEntity entity)
        {
            return _entities.Add(entity).Entity;
        }

        public TEntity Update(TEntity entity)
        {
            return _entities.Update(entity).Entity;
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void saveChanges()
        {
            context.SaveChanges();
        }

        public TEntity? GetWithInclude(int? id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _entities.AsQueryable();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.FirstOrDefault(e => e.id == id);
        }

        public List<TEntity> GetAllWithInclude(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _entities.AsQueryable();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.ToList();
        }
    }

}
