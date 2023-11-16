using Dapper;
using Dommel;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            if(obj?.Id != null)
            {
                await Update(obj);
            }
            else
            {
                obj = await Insert(obj);
            }
            return obj;           
        }

        public async virtual Task<bool> Update(Q obj)
        {
            return await iRycContextService.getConnection().UpdateAsync(obj);            
        }

        public async virtual Task<bool> Update(IEnumerable<Q> lObj)
        {
            return await iRycContextService.getConnection().UpdateAsync(lObj);
        }

        public async virtual Task<Q> Insert(Q obj)
        {
            return (Q) await iRycContextService.getConnection().InsertAsync(obj);
        }

        public async virtual Task<IEnumerable<Q>> Insert(IEnumerable<Q> lObj)
        {
            return (IEnumerable<Q>) await iRycContextService.getConnection().InsertAsync(lObj);
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
