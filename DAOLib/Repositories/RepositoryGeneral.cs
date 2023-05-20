using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAOLib.Repositories
{
    public class RepositoryGeneral<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> _entities;

        public DbSet<TEntity> entities { get { return _entities; } }

        public RepositoryGeneral(DbContext dbContext)
        {
            context = dbContext;
            _entities = context.Set<TEntity>();
        }        

        public TEntity? GetById(int? id)
        {
            return _entities.Find(id);
        }

        public TEntity? GetById(DateTime? id)
        {
            return _entities.Find(id);
        }

        public List<TEntity> GetAll()
        {
            return _entities.ToList();
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
