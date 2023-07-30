using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class ManagerBase<T> where T : ModelBaseDAO
    {
#pragma warning disable CS8603
        protected virtual Expression<Func<T, object>>[] GetIncludes()
        {
            return null;
        }
#pragma warning restore CS8603

        protected IQueryable<T>? GetEntyWithInclude(Repository<T> repository)
        {
            var query = repository.Entities.AsQueryable();

            foreach (var include in GetIncludes())
            {
                query = query?.Include(include);
            }

            return query;
        }

        public IEnumerable<T>? GetAll()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                var query = repository.GetAllWithInclude(GetIncludes());

                foreach (var item in query)
                {
                    yield return item;
                }
            }
        }

        public T? GetById(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                return repository.GetWithInclude(id, GetIncludes());
            }
        }

        public T? Update(T? obj)
        {
            if (obj != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryModelBase<T>();
                    var entity = repository.Update(obj);
                    repository.SaveChanges();
                    return entity;
                }
            }
            return null;
        }

        public void UpdateList(List<T?>? lObj)
        {
            if (lObj != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryModelBase<T>();
                    foreach (var item in lObj)
                    {
                        if (item != null)
                        {
                            repository.Update(item);
                        }
                    }
                    repository.SaveChanges();
                }
            }
        }

        public void Delete(T? obj)
        {
            if (obj != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryModelBase<T>();
                    repository.Delete(obj);
                    repository.SaveChanges();
                }
            }
        }

        public void SaveChanges()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<T>();
                repository.SaveChanges();
            }
        }
    }
}
