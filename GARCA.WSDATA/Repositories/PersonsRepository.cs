using Dommel;
using GARCA.Models;

namespace GARCA.wsData.Repositories
{
    public class PersonsRepository : RepositoryBase<Persons>
    {
        public override async Task<IEnumerable<Persons>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<Persons, Categories, Persons>();
            }
        }
    }
}
