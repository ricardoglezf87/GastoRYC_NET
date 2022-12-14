using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class CategoriesService
    {

        public enum eCategoriesTypes : int
        {
            Incomes = 1,
            Expenses = 2,            
            Transfers = 3,
            Specials = 4
        }

        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        public List<Categories>? getAll()
        {
            return RYCContextService.getInstance().BBDD.categories?.ToList();
        }

        public List<Categories>? getAllFilterTransfer()
        {
            return RYCContextService.getInstance().BBDD.categories?
                .Where(x => !x.id.Equals((int)CategoriesService.eCategoriesTypes.Transfers) &&
                !x.id.Equals((int)CategoriesService.eCategoriesTypes.Specials)).ToList();
        }

        public Categories? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.categories?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Categories categories)
        {
            RYCContextService.getInstance().BBDD.Update(categories);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Categories categories)
        {
            RYCContextService.getInstance().BBDD.Remove(categories);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
