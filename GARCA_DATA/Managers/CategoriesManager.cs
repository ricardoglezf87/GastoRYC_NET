using Dapper;
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class CategoriesManager : ManagerBase<Categories>
    {
        public override async Task<IEnumerable<Categories>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<Categories, CategoriesTypes, Categories>();
            }
        }

        public async Task<int> GetNextId()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';");
            }
        }
    }
}
