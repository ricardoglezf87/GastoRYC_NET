using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOLib.Managers
{
    public class ManagerBase<T> where T : ModelBaseDAO
    {
        public List<T>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.Set<T>().ToList();
        }

        public T? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.Set<T>().FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(T? obj, bool save = true)
        {
            if (obj != null)
            {
                RYCContextServiceDAO.getInstance().BBDD.Update(obj);
                if (save)
                    saveChanges();
            }
        }

        public void saveChanges()
        {
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public async Task saveChangesAsync()
        {
            await RYCContextServiceDAO.getInstance().BBDD.SaveChangesAsync();
        }

        public void delete(T? obj)
        {
            if (obj != null)
            {
                RYCContextServiceDAO.getInstance().BBDD.Remove(obj);
                RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
            }
        }
    }
}
