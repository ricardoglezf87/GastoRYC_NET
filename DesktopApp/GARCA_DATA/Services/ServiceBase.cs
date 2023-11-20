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
    public class ServiceBase <T,Q>
        where Q : ModelBase, new()
        where T : ManagerBase<Q>, new()
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

        public async virtual Task<Q?> GetById(int id)
        {
            return await manager.GetById(id);
        }

        public async virtual Task<Q?> GetById(DateTime id)
        {
            return await manager.GetById(id);
        }

        public async virtual Task<bool> Delete(Q obj)
        {
            return await manager.Delete(obj);
        }

        public async virtual Task<bool> Delete(int i)
        {
            return await manager.Delete(i);
        }

        public async virtual Task<Q> Save(Q obj)
        {
            return await manager.Save(obj);
        }

    }
}
