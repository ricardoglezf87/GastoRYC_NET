using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GARCA.DAO.Repositories
{
    public class RepositoryGeneral<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> _entities;
        
        public RepositoryGeneral(DbContext dbContext)
        {
            context = dbContext;
            _entities = context.Set<TEntity>();
        }
        
        public TEntity? GetById(DateTime? id)
        {
            return _entities.Find(id);
        }

        public HashSet<TEntity> GetAll()
        {
            return _entities.ToHashSet();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void saveChanges()
        {
            context.SaveChanges();
        }
    }

}
