using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class SplitsRemindersService
    {
        private readonly SplitsRemindersManager splitsRemindersManager;
        private static SplitsRemindersService? _instance;
        private static readonly object _lock = new();

        public static SplitsRemindersService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SplitsRemindersService();
                    }
                }
                return _instance;
            }
        }

        private SplitsRemindersService()
        {
            splitsRemindersManager = new();
        }

        public List<SplitsReminders?>? getbyTransactionidNull()
        {
            return splitsRemindersManager.getbyTransactionidNull()?.toListBO();
        }

        public List<SplitsReminders?>? getbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.getbyTransactionid(transactionid)?.toListBO();
        }

        public void update(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.update(splitsReminders.toDAO());
        }

        public void delete(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.delete(splitsReminders.toDAO());
        }
    }
}
