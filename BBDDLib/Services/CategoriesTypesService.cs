using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class CategoriesTypesService
    {

        public List<CategoriesTypes>? getAll()
        {
            return RYCContextService.getInstance().BBDD.categoriesTypes?.ToList();
        }

        public List<CategoriesTypes>? getAllFilterTransfer()
        {
            return RYCContextService.getInstance().BBDD.categoriesTypes?.Where(x=>!x.id.Equals((int)CategoriesService.eCategoriesTypes.Transferencias)).ToList();
        }

        public CategoriesTypes? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.categoriesTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
