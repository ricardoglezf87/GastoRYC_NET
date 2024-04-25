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
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<Q>();
            }
        }

        public virtual async Task<Q?> GetById(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAsync<Q>(id);
            }
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
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.UpdateAsync(obj);
            }
        }

        public virtual async Task<int> Insert(Q obj)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return Convert.ToInt32(await connection.InsertAsync<Q>(obj));
            }
        }

        public virtual async Task<bool> Delete(Q obj)
        {
            return await Delete(obj.Id);
        }

        public virtual async Task<bool> Delete(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.DeleteAsync(new Q() { Id = id });
            }
        }
    }
}
