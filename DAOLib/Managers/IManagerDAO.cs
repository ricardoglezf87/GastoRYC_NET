using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLib.Managers
{
    public class IManagerDAO<T> where T : IModelDAO
    {
        public List<T>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.Set<T>().ToList();
        }

        public T? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.Set<T>().FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(T obj)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(obj);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(T obj)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(obj);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }
    }
}
