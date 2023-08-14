using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GARCA.DAO.Repositories
{
    public class RepositoryGeneral<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> entities;

        public RepositoryGeneral(DbContext dbContext)
        {
            context = dbContext;
            entities = context.Set<TEntity>();
        }

        public TEntity? GetById(DateTime? id)
        {
            return entities.Find(id);
        }

        public HashSet<TEntity> GetAll()
        {
            return entities.ToHashSet();
        }

        public void Add(TEntity entity)
        {
            entities.Add(entity);
        }

        public void Update(TEntity entity)
        {
            entities.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            entities.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }

}
