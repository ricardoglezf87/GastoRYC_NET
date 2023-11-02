using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
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
            return splitsManager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public SplitsArchived? GetById(int? id)
        {
            return splitsManager.GetById(id);
        }

        public void Update(SplitsArchived splits)
        {
            splitsManager.Update(splits);
        }

        public void Delete(SplitsArchived splits)
        {
            splitsManager.Delete(splits);
        }

    }
}
