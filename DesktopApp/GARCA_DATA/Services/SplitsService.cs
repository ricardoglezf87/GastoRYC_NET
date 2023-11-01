using GARCA.Models;
using GARCA.Data.Managers;
using GARCA.Data.IOC;


namespace GARCA.Data.Services
{
    public class SplitsService
    {

        private readonly SplitsManager splitsManager;

        public SplitsService()
        {
            splitsManager = new SplitsManager();
        }

        public HashSet<Splits?>? GetbyTransactionidNull()
        {
            return splitsManager.GetbyTransactionidNull()?.ToHashSet();
        }

        public HashSet<Splits?>? GetbyTransactionid(int transactionid)
        {
            return splitsManager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public Splits? GetById(int? id)
        {
            return (Splits)splitsManager.GetById(id);
        }

        public void Update(Splits splits)
        {
            splitsManager.Update(splits);
        }

        public void Delete(Splits splits)
        {
            splitsManager.Delete(splits);
        }

        public void SaveChanges(Splits splits)
        {
            if (splits.Category == null && splits.Categoryid != null)
            {
                splits.Category = DependencyConfig.CategoriesService.GetById(splits.Categoryid);
            }

            splits.AmountIn ??= 0;

            splits.AmountOut ??= 0;

            Update(splits);
        }

    }
}
