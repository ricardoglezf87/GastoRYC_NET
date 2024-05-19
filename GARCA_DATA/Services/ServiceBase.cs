using GARCA.wsData.Repositories;
using GARCA.Models;

namespace GARCA.Data.Services
{
    public class ServiceBase<T, Q>
        where Q : ModelBase, new()
        where T : RepositoryBase<Q>, new()
    {
        protected T repository;

        public ServiceBase()
        {
            repository = new T();
        }

        public virtual async Task<IEnumerable<Q>?> GetAll()
        {
            return await repository.GetAll();
        }

        public virtual async Task<Q?> GetById(int id)
        {
            return await repository.GetById(id);
        }

        public virtual async Task<bool> Delete(Q? obj)
        {
            return obj != null && await repository.Delete(obj);
        }

        public virtual async Task<Q> Save(Q obj)
        {
            return await repository.Save(obj);
        }

    }
}
