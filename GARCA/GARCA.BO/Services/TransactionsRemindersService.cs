using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;
using GARCA.IOC;

namespace GARCA.BO.Services
{
    public class TransactionsRemindersService
    {
        private readonly TransactionsRemindersManager transactionsRemindersManager;
        
        public TransactionsRemindersService()
        {
            transactionsRemindersManager = new();
        }

        #region TransactionsRemindersActions

        public HashSet<TransactionsReminders?>? getAll()
        {
            return transactionsRemindersManager.getAll()?.toHashSetBO();
        }

        public TransactionsReminders? getByID(int? id)
        {
            return (TransactionsReminders?)transactionsRemindersManager.getByID(id);
        }

        public TransactionsReminders? update(TransactionsReminders transactionsReminders)
        {
            DependencyConfig.iExpirationsRemindersService.deleteByTransactionReminderid(transactionsReminders.id);
            return (TransactionsReminders?)transactionsRemindersManager.update(transactionsReminders.toDAO());
        }

        public void delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                DependencyConfig.iExpirationsRemindersService.deleteByTransactionReminderid(transactionsReminders.id);
                transactionsRemindersManager.delete(transactionsReminders.toDAO());
            }
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
            List<SplitsReminders?>? lSplitsReminders = transactionsReminders.splits ?? DependencyConfig.iSplitsRemindersService.getbyTransactionid(transactionsReminders.id);

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
                transactionsReminders.category = DependencyConfig.iCategoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid is not null
                and ((int)CategoriesService.eSpecialCategories.Split))
            {
                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = DependencyConfig.iCategoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsReminders? splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    DependencyConfig.iSplitsRemindersService.update(splitsReminders);
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
