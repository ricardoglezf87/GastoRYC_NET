using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;

namespace GARCA.BO.Services
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
            return splitsManager.GetbyTransactionidNull()?.ToHashSetBo();
        }

        public HashSet<Splits?>? GetbyTransactionid(int transactionid)
        {
            return splitsManager.GetbyTransactionid(transactionid)?.ToHashSetBo();
        }

        public Splits? GetById(int? id)
        {
            return (Splits?)splitsManager.GetById(id);
        }

        public void Update(Splits splits)
        {
            splitsManager.Update(splits.ToDao());
        }

        public void Delete(Splits splits)
        {
            splitsManager.Delete(splits.ToDao());
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
