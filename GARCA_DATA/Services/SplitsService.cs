using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsService : ServiceBase<SplitsRepository, Splits>
    {
        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            return await repository.GetbyTransactionidNull();
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            return await repository.GetbyTransactionid(transactionid);
        }

    }
}
