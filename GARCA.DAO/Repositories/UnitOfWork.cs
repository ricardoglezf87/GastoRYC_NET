using GARCA.DAO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

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

        public Repository<TEntity> GetRepositoryModelBase<TEntity>() where TEntity : ModelBaseDAO
        {
            return new Repository<TEntity>(context);
        }

        public RepositoryGeneral<TEntity> GetRepositoryGeneral<TEntity>() where TEntity : class
        {
            return new RepositoryGeneral<TEntity>(context);
        }

        public DatabaseFacade getDataBase()
        {
            return context.Database;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
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
