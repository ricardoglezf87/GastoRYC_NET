using Dommel;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.wsData.Repositories
{
    public class RepositoryBase<Q> : IRepositoryBase<Q>
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

        public virtual async Task<IEnumerable<Q>?> Get(Expression<Func<Q,bool>> predicate)
        {
            return await dbContext.OpenConnection().SelectAsync<Q>(predicate);
        }

        public virtual async Task<Q> Save(Q obj)
        {
            if (obj.Id != 0)
            {
                await Update(obj);
            }
            else
            {
                obj.Id = await Insert(obj);
            }
            return obj;
        }

        public virtual async Task<bool> Update(Q obj)
        {
            return await dbContext.OpenConnection().UpdateAsync(obj);
        }

        public virtual async Task<int> Insert(Q obj)
        {
            return (int)(UInt64)await dbContext.OpenConnection().InsertAsync<Q>(obj);
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
