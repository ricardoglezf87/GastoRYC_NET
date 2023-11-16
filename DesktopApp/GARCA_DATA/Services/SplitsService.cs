using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class SplitsService : ServiceBase<SplitsManager, Splits>
    {

        private readonly SplitsManager splitsManager;

        public SplitsService()
        {
            splitsManager = new SplitsManager();
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            return await splitsManager.GetbyTransactionidNull();
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            return await splitsManager.GetbyTransactionid(transactionid);
        }

        public async Task SaveChanges(Splits splits)
        {
            if (splits.Category == null && splits.Categoryid != null)
            {
                splits.Category = await iCategoriesService.GetById(splits.Categoryid ?? -99);
            }

            splits.AmountIn ??= 0;

            splits.AmountOut ??= 0;

            await Update(splits);
        }

    }
}
