using DAOLib.Models;
using DAOLib.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class ManagerBase<T> where T : ModelBaseDAO
    {
#pragma warning disable CS8603
        public virtual Expression<Func<T, object>>[] getIncludes()
        {
            return null;
        }
#pragma warning restore CS8603

        public IQueryable<T>? getEntyWithInclude(Repository<T> repository)
        {
            var query = repository.entities.AsQueryable();

            foreach (var include in getIncludes())
            {
                query = query?.Include(include);
            }

            return query;
        }

        public IQueryable<T>? getEntyWithInclude()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                var query = repository.entities.AsQueryable();

                foreach (var include in getIncludes())
                {
                    query = query?.Include(include);
                }

                return query;
            }
        }

        public List<T>? getAll()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                return repository.GetAllWithInclude(getIncludes());
            }
        }


        public T? getByID(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                return repository.GetWithInclude(id, getIncludes());
            }
        }

        public T? update(T? obj, bool save = true)
        {
            if (obj != null)
            {
                using (var unitOfWork = new UnitOfWork(new RYCContext()))
                {
                    var repository = unitOfWork.GetRepositoryModelBase<T>();
                    var entity = repository.Update(obj);
                    if (save)
                    {
                        repository.saveChanges();
                    }

                    return entity;
                }
            }
            return null;
        }

        public void delete(T? obj)
        {
            if (obj != null)
            {
                using (var unitOfWork = new UnitOfWork(new RYCContext()))
                {
                    var repository = unitOfWork.GetRepositoryModelBase<T>();
                    repository.Delete(obj);
                    repository.saveChanges();
                }
            }
        }
    }
}
