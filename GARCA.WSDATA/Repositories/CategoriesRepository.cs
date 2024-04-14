using Dapper;
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class CategoriesRepository : RepositoryBase<Categories>
    {
        public override async Task<IEnumerable<Categories>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<Categories, CategoriesTypes, Categories>();
            }
        }

        public async Task<int> GetNextId()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';");
            }
        }
    }
}
