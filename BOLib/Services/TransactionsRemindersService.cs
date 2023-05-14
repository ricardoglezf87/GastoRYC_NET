using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class TransactionsRemindersService
    {
        private readonly TransactionsRemindersManager transactionsRemindersManager;
        private readonly SplitsRemindersService splitsRemindersService;
        private readonly ExpirationsRemindersService expirationsRemindersService;
        private readonly CategoriesService categoriesService;

        public TransactionsRemindersService()
        {
            transactionsRemindersManager = InstanceBase<TransactionsRemindersManager>.Instance;
            expirationsRemindersService = InstanceBase<ExpirationsRemindersService>.Instance;
            splitsRemindersService = InstanceBase<SplitsRemindersService>.Instance;
            categoriesService = InstanceBase<CategoriesService>.Instance;
        }

        #region TransactionsRemindersActions

        public List<TransactionsReminders>? getAll()
        {
            return transactionsRemindersManager.getAll()?.toListBO();
        }

        public TransactionsReminders? getByID(int? id)
        {
            return (TransactionsReminders)transactionsRemindersManager.getByID(id);
        }

        public void update(TransactionsReminders transactionsReminders)
        {
            expirationsRemindersService.deleteByTransactionReminderid(transactionsReminders.id);
            transactionsRemindersManager.update(transactionsReminders.toDAO());
        }

        public void delete(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                expirationsRemindersService.deleteByTransactionReminderid(transactionsReminders.id);
                transactionsRemindersManager.delete(transactionsReminders.toDAO());
            }
        }

        public int getNextID()
        {
            return transactionsRemindersManager.getNextID();
        }

        public void saveChanges(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders.amountIn == null)
                transactionsReminders.amountIn = 0;

            if (transactionsReminders.amountOut == null)
                transactionsReminders.amountOut = 0;

            update(transactionsReminders);
        }

        #endregion

        #region SplitsRemindersActions

        public void updateSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            List<SplitsReminders>? lSplitsReminders = transactionsReminders.splits ?? splitsRemindersService.getbyTransactionid(transactionsReminders.id);

            if (lSplitsReminders != null && lSplitsReminders.Count != 0)
            {
                transactionsReminders.amountIn = 0;
                transactionsReminders.amountOut = 0;

                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    transactionsReminders.amountIn += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    transactionsReminders.amountOut += (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }

                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactionsReminders.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactionsReminders.categoryid != null
                && transactionsReminders.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactionsReminders.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactionsReminders.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactionsReminders.id == 0)
            {
                update(transactionsReminders);
                foreach (SplitsReminders splitsReminders in lSplitsReminders)
                {
                    splitsReminders.transactionid = transactionsReminders.id;
                    splitsRemindersService.update(splitsReminders);
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
