using DAOLib.Models;
using DAOLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class CategoriesManagerDAO : IManagerDAO<CategoriesDAO>
    {
        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        private readonly SimpleInjector.Container servicesContainer;

        public CategoriesManagerDAO(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        public List<CategoriesDAO>? getAllFilterTransfer()
        {
            return RYCContextServiceDAO.getInstance().BBDD.categories?
                .Where(x => !x.id.Equals(CategoriesTypesManagerDAO.eCategoriesTypes.Transfers) &&
                !x.id.Equals(CategoriesTypesManagerDAO.eCategoriesTypes.Specials)).ToList();
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
