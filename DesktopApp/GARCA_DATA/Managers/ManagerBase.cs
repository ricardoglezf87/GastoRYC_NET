using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ManagerBase<Q>
        where Q : ModelBase, new()
    {
        public virtual async Task<IEnumerable<Q>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<Q>();
        }

        public virtual async Task<Q?> GetById(int id)
        {
            return await iRycContextService.getConnection().GetAsync<Q>(id);
        }

        public virtual async Task<Q?> GetById(DateTime id)
        {
            return await iRycContextService.getConnection().GetAsync<Q>(id);
        }

        public virtual async Task<Q> Save(Q obj)
        {
            if (obj.Id != 0)
            {
                await Update(obj);
            }
            else
            {
                obj.Id = Convert.ToInt32(await Insert(obj));
            }
            return obj;
        }

        public virtual async Task<bool> Update(Q obj)
        {
            return await iRycContextService.getConnection().UpdateAsync(obj);
        }

        public virtual async Task<long> Insert(Q obj)
        {
            return (long)await iRycContextService.getConnection().InsertAsync<Q>(obj);
        }

        public virtual async Task<bool> Delete(Q obj)
        {
            return await Delete(obj.Id);
        }

        public virtual async Task<bool> Delete(int id)
        {
            return await iRycContextService.getConnection().DeleteAsync(new Q() { Id = id });
        }
    }
}
