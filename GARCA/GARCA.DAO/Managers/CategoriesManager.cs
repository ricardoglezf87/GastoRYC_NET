using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GARCA.DAO.Managers
{
    public class CategoriesManager : ManagerBase<CategoriesDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<CategoriesDAO, object>>[] getIncludes()
        {
            return new Expression<Func<CategoriesDAO, object>>[]
            {
                a => a.categoriesTypes
            };
        }
#pragma warning restore CS8603

        public IEnumerable<CategoriesDAO>? getAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesDAO>();
                var query =  getEntyWithInclude(repository)?
                .Where(x => !x.categoriesTypesid.Equals((int)CategoriesTypesManager.eCategoriesTypes.Transfers) &&
                !x.categoriesTypesid.Equals((int)CategoriesTypesManager.eCategoriesTypes.Specials));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';";

                unitOfWork.getDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                var id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
