using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class SplitsArchivedService
    {

        private readonly SplitsArchivedManager splitsManager;

        public SplitsArchivedService()
        {
            splitsManager = new SplitsArchivedManager();
        }

        public HashSet<SplitsArchived?>? GetbyTransactionidNull()
        {
            return splitsManager.GetbyTransactionidNull()?.ToHashSetBo();
        }

        public HashSet<SplitsArchived?>? GetbyTransactionid(int transactionid)
        {
            return splitsManager.GetbyTransactionid(transactionid)?.ToHashSetBo();
        }

        public SplitsArchived? GetById(int? id)
        {
            return (SplitsArchived?)splitsManager.GetById(id);
        }

        public void Update(SplitsArchived splits)
        {
            splitsManager.Update(splits.ToDao());
        }

        public void Delete(SplitsArchived splits)
        {
            splitsManager.Delete(splits.ToDao());
        }

        public void SaveChanges(SplitsArchived splits)
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
