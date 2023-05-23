using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class TransactionsRemindersService
    {
        private readonly TransactionsRemindersManager transactionsRemindersManager;
        private static TransactionsRemindersService? _instance;
        private static readonly object _lock = new();

        public static TransactionsRemindersService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new TransactionsRemindersService();
                    }
                }
                return _instance;
            }
        }

        private TransactionsRemindersService()
        {
            transactionsRemindersManager = new();
        }

        #region TransactionsRemindersActions

        public List<TransactionsReminders?>? getAll()
        {
            return transactionsRemindersManager.getAll()?.toListBO();
        }

        public TransactionsReminders? getByID(int? id)
        {
            return (TransactionsReminders?)transactionsRemindersManager.getByID(id);
        }

        public TransactionsReminders? update(TransactionsReminders transactionsReminders)
        {
            ExpirationsRemindersService.Instance.deleteByTransactionReminderid(transactionsReminders.id);
            return (TransactionsReminders?)transactionsRemindersManager.update(transactionsReminders.toDAO());
        }

        public void delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                ExpirationsRemindersService.Instance.deleteByTransactionReminderid(transactionsReminders.id);
                transactionsRemindersManager.delete(transactionsReminders.toDAO());
            }
        }

        public int getNextID()
        {
            return transactionsRemindersManager.getNextID();
        }

        public void saveChanges(TransactionsReminders transactionsReminders)
        {
            transactionsReminders.amountIn ??= 0;

            transactionsReminders.amountOut ??= 0;

            update(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public void updateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            List<SplitsReminders?>? lSplitsReminders = transactionsReminders.splits ?? SplitsRemindersService.Instance.getbyTransactionid(transactionsReminders.id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.amountIn = 0;
                transactionsReminders.amountOut = 0;

                foreach (SplitsReminders? splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.amountIn += splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn;
                    transactionsReminders.amountOut += splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut;
                }

                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactionsReminders.category = CategoriesService.Instance.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid is not null
                and ((int)CategoriesService.eSpecialCategories.Split))
            {
                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = CategoriesService.Instance.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsReminders? splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    SplitsRemindersService.Instance.update(splitsReminders);
                }
            }
            else
            {
                update(transactionsReminders);
            }
        }

        #endregion

    }
}
