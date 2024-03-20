using Dapper;
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class CategoriesRepository : RepositoryBase<Categories>
    {
        public override async Task<IEnumerable<Categories>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<Categories, CategoriesTypes, Categories>();
        }

        public async Task<int> GetNextId()
        {
            return await dbContext.OpenConnection().ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';");
        }
    }
}
