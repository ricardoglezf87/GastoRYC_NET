using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class SplitsService : ServiceBase<SplitsManager, Splits, Int32>
    {

        private readonly SplitsManager splitsManager;

        public SplitsService()
        {
            splitsManager = new SplitsManager();
        }

        public HashSet<Splits>? GetbyTransactionidNull()
        {
            return splitsManager.GetbyTransactionidNull()?.ToHashSet();
        }

        public HashSet<Splits>? GetbyTransactionid(int transactionid)
        {
            return splitsManager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public void Update(Splits splits)
        {
            splitsManager.Update(splits);
        }

        public void SaveChanges(Splits splits)
        {
            if (splits.Category == null && splits.Categoryid != null)
            {
                splits.Category = iCategoriesService.GetById(splits.Categoryid ?? -99);
            }

            splits.AmountIn ??= 0;

            splits.AmountOut ??= 0;

            Update(splits);
        }

    }
}
