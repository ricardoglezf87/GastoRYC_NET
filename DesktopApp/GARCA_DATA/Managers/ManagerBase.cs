using Dapper;
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ManagerBase<Q> 
        where Q : ModelBase, new()        
    {
        public async virtual Task<IEnumerable<Q>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<Q>();   
        }

        public async virtual Task<Q?> GetById(int id)
        { 
            return await iRycContextService.getConnection().GetAsync<Q>(id);
        }

        public async virtual Task<Q?> GetById(DateTime id)
        {
            return await iRycContextService.getConnection().GetAsync<Q>(id);
        }

        public async virtual Task<Q> Save(Q obj)
        {
            if(obj.Id != 0)
            {
                await Update(obj);
            }
            else  
            {
               obj.Id = Convert.ToInt32(await Insert(obj));
            }            
            return obj;           
        }

        public async virtual Task<bool> Update(Q obj)
        {
            return await iRycContextService.getConnection().UpdateAsync(obj);            
        }

        public async virtual Task<bool> Update(IEnumerable<Q> lObj)
        {
            foreach (var obj in lObj)
            {
                await iRycContextService.getConnection().UpdateAsync(obj);
            }
            return true;
        }

        public async virtual Task<long> Insert(Q obj)
        {
            return (long) await iRycContextService.getConnection().InsertAsync<Q>(obj);
        }

        public async virtual Task<bool> Delete(Q obj)
        {
            return await Delete(obj.Id);
        }

        public async virtual Task<bool> Delete(int id)
        {
            return await iRycContextService.getConnection().DeleteAsync(new Q() { Id = id });
        }
    }
}
