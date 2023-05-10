using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class CategoriesTypesServiceDAO : IServiceDAO<CategoriesTypesDAO>
    {
        public enum eCategoriesTypes : int
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public List<CategoriesTypesDAO>? getAllFilterTransfer()
        {
            return RYCContextServiceDAO.getInstance().BBDD.categoriesTypes?
                .Where(x => !x.id.Equals((int)eCategoriesTypes.Transfers) &&
                !x.id.Equals((int)eCategoriesTypes.Transfers)).ToList();
        }
    }
}
