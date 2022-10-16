using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class CategoriesService
    {
        public List<Categories>? getAll()
        {
            return RYCContextService.Instance.BBDD.categories?.ToList();
        }

        public Categories? getByID(long? id)
        {
            return RYCContextService.Instance.BBDD.categories?.FirstOrDefault(x => id.Equals(x.id));
        }
    }
}
