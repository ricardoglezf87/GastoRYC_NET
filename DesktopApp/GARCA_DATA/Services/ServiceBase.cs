using GARCA.Data.Managers;
using GARCA.Models;
using GARCA_DATA.Managers;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Data.Services
{
    public class ServiceBase <T,Q,Z> 
        where T : ManagerBase<Q,Z>, new()
        where Q : ModelBase<Z>
    {
        protected T manager;

        public ServiceBase()
        {
            manager = new T();
        }

        public async virtual Task<IEnumerable<Q>?> GetAll()
        {
            return await manager.GetAll();
        }

        public async virtual Task<Q?> GetById(Z id)
        {
            return await manager.GetById(id);
        }

        public async virtual Task<bool> Delete(Q obj)
        {
            return await manager.Delete(obj);
        }

        public async virtual Task<bool> Delete(Z i)
        {
            return await manager.Delete(i);
        }

        public async virtual Task<bool> Update(Q obj)
        {
           return await manager.Update(obj);
        }

        public async virtual Task<bool> Update(IEnumerable<Q> lObj)
        {
            return await manager.Update(lObj);
        }

        public async virtual Task<Q> Insert(Q obj)
        {
            return await manager.Insert(obj);
        }

        public async virtual Task<IEnumerable<Q>> Insert(IEnumerable<Q> lObj)
        {
            return await manager.Insert(lObj);
        }

        public async virtual Task<Q> Save(Q obj)
        {
            return await manager.Save(obj);
        }

    }
}
