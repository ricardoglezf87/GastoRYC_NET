using DAOLib.Models;
using DAOLib.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
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

        public List<CategoriesDAO>? getAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesDAO>();
                return getEntyWithInclude(repository)?
                .Where(x => !x.categoriesTypesid.Equals((int)CategoriesTypesManager.eCategoriesTypes.Transfers) &&
                !x.categoriesTypesid.Equals((int)CategoriesTypesManager.eCategoriesTypes.Specials)).ToList();
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
                int id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
