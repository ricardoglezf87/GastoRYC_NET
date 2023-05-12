using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using DAOLib.Models;
using DAOLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOLib.Managers
{
    public class ServiceBase<T,Z,Y> 
        where T : ModelBase
        where Z : ModelBaseDAO, new()
        where Y : ManagerBase<Z>, new()
    {
        private readonly Y manager;

        public ServiceBase()
        {
            manager = new();
        }

        public List<T>? getAll()
        {
            return ListExtensionBase<T, Z>.toListBO(manager.getAll());
        }

        public T? getByID(int? id)
        {
            return (T)manager.getByID(id);
        }

        public void update(T obj)
        {
            manager.update((Z)obj.toDAO());
        }

        public void delete(T obj)
        {
            manager.delete((Z)obj.toDAO());
        }
    }
}
