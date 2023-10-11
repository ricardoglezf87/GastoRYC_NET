using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class CategoriesManager : ManagerBase<CategoriesDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<CategoriesDao, object>>[] GetIncludes()
        {
            return new Expression<Func<CategoriesDao, object>>[]
            {
                a => a.CategoriesTypes
            };
        }
#pragma warning restore CS8603

        public IEnumerable<CategoriesDao>? GetAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesDao>();
                var query = GetEntyWithInclude(repository)?
                .Where(x => !x.CategoriesTypesid.Equals((int)CategoriesTypesManager.ECategoriesTypes.Transfers) &&
                !x.CategoriesTypesid.Equals((int)CategoriesTypesManager.ECategoriesTypes.Specials));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public int GetNextId()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var cmd = unitOfWork.GetDataBase().
                GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';";

                unitOfWork.GetDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                var id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
