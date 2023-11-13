using Dapper;
using GARCA.DAO.Repositories;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA_DATA.Services;
using Microsoft.Data.Sqlite;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class CategoriesService : IServiceCache<Categories>
    {
        public enum ESpecialCategories
        {
            Cierre = -2,
            Split = -1,
            WithoutCategory = 0
        }

        protected override IEnumerable<Categories>? GetAllCache()
        {
            return iRycContextService.getConnection().Query<Categories, CategoriesTypes, Categories>(
                @"
                    select * 
                    from Categories
                        inner join CategoriesTypes on CategoriesTypes.Id = Categories.categoriesTypesid
                "
                , (a, at) =>
                {
                    a.CategoriesTypes = at;
                    return a;
                }).AsEnumerable();

        }

        public HashSet<Categories>? GetAllWithoutSpecialTransfer()
        {
            return GetAll().Where(x => !x.CategoriesTypesid.
                Equals((int)CategoriesTypesService.ECategoriesTypes.Transfers) &&
                !x.CategoriesTypesid.Equals((int)CategoriesTypesService.ECategoriesTypes.Specials))?.ToHashSet();
        }

        public int GetNextId()
        {
            return 1000; //TODO:Poner dengro de CacheService
            //using (var unitOfWork = new UnitOfWork(new RycContext()))
            //{
            //    var cmd = unitOfWork.GetDataBase().
            //    GetDbConnection().CreateCommand();
            //    cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';";

            //    unitOfWork.GetDataBase().OpenConnection();
            //    var result = cmd.ExecuteReader();
            //    result.Read();
            //    var id = Convert.ToInt32(result[0]);
            //    result.Close();

            //    return id;
            //}
        }
    }
}
