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
    }
}
