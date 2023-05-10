using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class CategoriesServiceDAO
    {
        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        private readonly SimpleInjector.Container servicesContainer;

        public CategoriesServiceDAO(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        public List<CategoriesDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.categories?.ToList();
        }

        public List<CategoriesDAO>? getAllFilterTransfer()
        {
            return RYCContextServiceDAO.getInstance().BBDD.categories?
                .Where(x => !x.id.Equals(CategoriesTypesServiceDAO.eCategoriesTypes.Transfers) &&
                !x.id.Equals(CategoriesTypesServiceDAO.eCategoriesTypes.Specials)).ToList();
        }

        public CategoriesDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.categories?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(CategoriesDAO categories)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(categories);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(CategoriesDAO categories)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(categories);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
