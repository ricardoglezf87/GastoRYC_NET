using GARCA.Data.Managers;
using GARCA.Models;

namespace GARCA.Data.Services
{
    public class ServiceBase<T, Q>
        where Q : ModelBase, new()
        where T : ManagerBase<Q>, new()
    {
        protected T manager;

        public ServiceBase()
        {
            manager = new T();
        }

        public virtual async Task<IEnumerable<Q>?> GetAll()
        {
            return await manager.GetAll();
        }

        public virtual async Task<Q?> GetById(int id)
        {
            return await manager.GetById(id);
        }

        public virtual async Task<Q?> GetById(DateTime id)
        {
            return await manager.GetById(id);
        }

        public virtual async Task<bool> Delete(Q? obj)
        {
            return obj != null && await manager.Delete(obj);
        }

        public virtual async Task<bool> Delete(int i)
        {
            return await manager.Delete(i);
        }

        public virtual async Task<Q> Save(Q obj)
        {
            return await manager.Save(obj);
        }

    }
}
