using Dapper;
using Dommel;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ManagerBase<T,Q> 
        where T : ModelBase<Q>, new()        
    {
        public ManagerBase()
        {

        }

        public async virtual Task<IEnumerable<T>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<T>();   
        }

        public async virtual Task<T?> GetById(Q id)
        {
            if (id != null)
            {
                return await iRycContextService.getConnection().GetAsync<T>(id);
            }
            return null;
        }

        public async virtual Task<T> Save(T obj)
        {
            if(obj.Id != null)
            {
                await Update(obj);
            }
            else
            {
                obj = await Insert(obj);
            }
            return obj;           
        }

        public async virtual Task<bool> Update(T obj)
        {
            return await iRycContextService.getConnection().UpdateAsync(obj);            
        }

        public async virtual Task<bool> Update(IEnumerable<T> lObj)
        {
            return await iRycContextService.getConnection().UpdateAsync(lObj);
        }

        public async virtual Task<T> Insert(T obj)
        {
            return (T) await iRycContextService.getConnection().InsertAsync(obj);
        }

        public async virtual Task<IEnumerable<T>> Insert(IEnumerable<T> lObj)
        {
            return (IEnumerable<T>) await iRycContextService.getConnection().InsertAsync(lObj);
        }

        public async virtual Task<bool> Delete(T obj)
        {
            return await Delete(obj.Id);
        }

        public async virtual Task<bool> Delete(Q id)
        {
            return await iRycContextService.getConnection().DeleteAsync(new T() { Id = id });
        }
    }
}
