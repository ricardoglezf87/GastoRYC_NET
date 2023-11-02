using GARCA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GARCA.DAO.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly DbContext context;
        private bool disposed = false;

        public UnitOfWork(DbContext dbContext)
        {
            context = dbContext;
        }

        public Repository<TEntity> GetRepositoryModelBase<TEntity>() where TEntity : ModelBase
        {
            return new Repository<TEntity>(context);
        }

        public RepositoryGeneral<TEntity> GetRepositoryGeneral<TEntity>() where TEntity : class
        {
            return new RepositoryGeneral<TEntity>(context);
        }

        public DatabaseFacade GetDataBase()
        {
            return context.Database;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }            
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
