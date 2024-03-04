using Dommel;
using GARCA.Models;

namespace GARCA.wsData.Managers
{
    public class ManagerBase<Q> : IManagerBase<Q>
        where Q : ModelBase, new()
    {

        public virtual async Task<IEnumerable<Q>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<Q>();
        }

        public virtual async Task<Q?> GetById(int id)
        {
            return await dbContext.OpenConnection().GetAsync<Q>(id);
        }

        public virtual async Task<Q?> GetById(DateTime id)
        {
            return await dbContext.OpenConnection().GetAsync<Q>(id);
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
            return await dbContext.OpenConnection().UpdateAsync(obj);
        }

        public virtual async Task<long> Insert(Q obj)
        {
            return (long)await dbContext.OpenConnection().InsertAsync<Q>(obj);
        }

        public virtual async Task<bool> Delete(Q obj)
        {
            return await Delete(obj.Id);
        }

        public virtual async Task<bool> Delete(int id)
        {
            return await dbContext.OpenConnection().DeleteAsync(new Q() { Id = id });
        }
    }
}
