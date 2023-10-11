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

    }
}
