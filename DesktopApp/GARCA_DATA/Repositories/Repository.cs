using GARCA.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GARCA.DAO.Repositories
{
    public class Repository<TEntity> where TEntity : ModelBase
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
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.FirstOrDefault(e => e.Id == id);
        }

        public HashSet<TEntity> GetAllWithInclude(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Entities.AsQueryable();

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.ToHashSet();
        }
    }

}
