using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface ICategoriesTypesService
    {
        public enum eCategoriesTypes : int
        {
            Incomes = 1,
            Expenses = 2,
            Transfers = 3,
            Specials = 4
        }

        public List<CategoriesTypes>? getAll();

        public List<CategoriesTypes>? getAllFilterTransfer();

        public CategoriesTypes? getByID(int? id);

    }
}
