using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class TransactionsService
    {
        #region Propiedades y Contructor

        private readonly TransactionsManager transactionsManager;
        private readonly SplitsService splitsService;
        private readonly AccountsService accountsService;
        private readonly CategoriesService categoriesService;
        private readonly PersonsService personsService;

        public TransactionsService()
        {
            transactionsManager = InstanceBase<TransactionsManager>.Instance;
            accountsService = InstanceBase<AccountsService>.Instance;
            splitsService = InstanceBase<SplitsService>.Instance;
            categoriesService = InstanceBase<CategoriesService>.Instance;
            personsService = InstanceBase<PersonsService>.Instance;
        }

        #endregion Propiedades y Contructor

        #region TransactionsActions

        public List<Transactions>? getAll()
        {
            return transactionsManager.getAll()?.toListBO();
        }

        public Transactions? getByID(int? id)
        {
            return (Transactions)transactionsManager.getByID(id);
        }

        public List<Transactions>? getByInvestmentProduct(int? id)
        {
            return transactionsManager.getByInvestmentProduct(id)?.toListBO();
        }

        public List<Transactions>? getByInvestmentProduct(InvestmentProducts? investment)
        {
            return getByInvestmentProduct(investment.id);
        }

        public void update(Transactions transactions)
        {
            transactions.date = transactions.date.removeTime();
            transactionsManager.update(transactions?.toDAO());
        }

        public void delete(Transactions? transactions)
        {
            transactionsManager.delete(transactions?.toDAO());
        }

        public List<Transactions>? getByAccount(int? id)
        {
            return transactionsManager.getByAccount(id)?.toListBO();
        }

        public List<Transactions>? getByAccount(Accounts? accounts)
        {
            return getByAccount(accounts?.id);
        }

        public List<Transactions>? getByPerson(int? id)
        {
            return transactionsManager.getByPerson(id)?.toListBO();
        }

        public List<Transactions>? getByPerson(Persons? person)
        {
            return getByPerson(person?.id);
        }

        public int getNextID()
        {
            return transactionsManager.getNextID();
        }

        public void saveChanges(Transactions transactions)
        {
            transactions.amountIn ??= 0;

            transactions.amountOut ??= 0;

            updateTranfer(transactions);
            updateTranferFromSplit(transactions);
            update(transactions);
            personsService.setCategoryDefault(transactions.person);
        }

        public void updateTranfer(Transactions transactions)
        {
            if (transactions.tranferid != null &&
                transactions.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    delete(tContraria);
                }
                transactions.tranferid = null;
            }
            else if (transactions.tranferid == null &&
                transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                transactions.tranferid = getNextID();

                Transactions? tContraria = new()
                {
                    date = transactions.date,
                    accountid = transactions.category.accounts.id,
                    personid = transactions.personid,
                    categoryid = accountsService.getByID(transactions.accountid)?.categoryid,
                    memo = transactions.memo,
                    tagid = transactions.tagid,
                    amountIn = transactions.amountOut,
                    amountOut = transactions.amountIn
                };

                if (transactions.id != 0)
                    tContraria.tranferid = transactions.id;
                else
                    tContraria.tranferid = getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                update(tContraria);

            }
            else if (transactions.tranferid != null &&
                transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = transactions.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = accountsService.getByID(transactions.accountid)?.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    update(tContraria);
                }
            }
        }

        #endregion

        #region SplitsActions

        public void updateTranferFromSplit(Transactions transactions)
        {
            if (transactions.tranferSplitid != null &&
                transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Splits? tContraria = splitsService.getByID(transactions.tranferSplitid);
                if (tContraria != null)
                {
                    tContraria.transaction.date = transactions.date;
                    tContraria.transaction.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transaction.transactionStatusid = transactions.transactionStatusid;
                    splitsService.update(tContraria);
                }
            }
        }

        public void updateTransactionAfterSplits(Transactions? transactions)
        {
            List<Splits>? lSplits = transactions.splits ?? splitsService.getbyTransactionid(transactions.id);

            if (lSplits != null && lSplits.Count != 0)
            {
                transactions.amountIn = 0;
                transactions.amountOut = 0;

                foreach (Splits splits in lSplits)
                {
                    transactions.amountIn += (splits.amountIn == null ? 0 : splits.amountIn);
                    transactions.amountOut += (splits.amountOut == null ? 0 : splits.amountOut);
                }

                transactions.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactions.categoryid != null
                && transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactions.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactions.id == 0)
            {
                update(transactions);
                foreach (Splits splits in lSplits)
                {
                    splits.transactionid = transactions.id;
                    splitsService.update(splits);
                }
            }
            else
            {
                update(transactions);
            }
        }

        public void updateTranferSplits(Transactions? transactions, Splits splits)
        {
            if (splits.tranferid != null &&
                splits.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(splits.tranferid);
                if (tContraria != null)
                {
                    delete(tContraria);
                }
                splits.tranferid = null;
            }
            else if (splits.tranferid == null &&
                splits.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                splits.tranferid = getNextID();

                Transactions? tContraria = new()
                {
                    date = transactions.date,
                    accountid = splits.category.accounts.id,
                    personid = transactions.personid,
                    categoryid = transactions.account.categoryid,
                    memo = splits.memo,
                    tagid = transactions.tagid,
                    amountIn = splits.amountOut,
                    amountOut = splits.amountIn,
                    tranferSplitid = (splits.id != 0 ? splits.id : getNextID() + 1),
                    transactionStatusid = transactions.transactionStatusid
                };

                update(tContraria);

            }
            else if (splits.tranferid != null &&
                splits.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(splits.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = splits.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = splits.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = splits.amountOut ?? 0;
                    tContraria.amountOut = splits.amountIn ?? 0;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    update(tContraria);
                }
            }
        }

        #endregion

    }
}
