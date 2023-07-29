using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class TransactionsService
    {
        #region Propiedades y Contructor

        private readonly TransactionsManager transactionsManager;
        
        public TransactionsService()
        {
            transactionsManager = new();
        }

        #endregion Propiedades y Contructor

        #region TransactionsActions

        public HashSet<Transactions?>? getAll()
        {
            return transactionsManager.getAll()?.toHashSetBO();
        }

        public List<Transactions?>? getAllOpenned()
        {
            return transactionsManager.getAllOpenned()?.toListBO();
        }

        public List<Transactions?>? getAllOpennedOrderByOrderDesc(int startIndex, int nPage)
        {
            return transactionsManager.getAllOpennedOrderByOrdenDesc(startIndex, nPage)?.toListBO();
        }

        public Transactions? getByID(int? id)
        {
            return (Transactions?)transactionsManager.getByID(id);
        }

        public List<Transactions?>? getByInvestmentProduct(int? id)
        {
            return transactionsManager.getByInvestmentProduct(id)?.toListBO();
        }

        public List<Transactions?>? getByInvestmentProduct(InvestmentProducts? investment)
        {
            return getByInvestmentProduct(investment.id);
        }

        public Transactions? update(Transactions transactions)
        {
            transactions.date = transactions.date.removeTime();
            transactions.orden = createOrden(transactions);
            return (Transactions?)transactionsManager.update(transactions?.toDAO());
        }

        public void updateList(List<Transactions?>? lObj)
        {
            if (lObj != null)
            {
                transactionsManager.updateList(lObj?.toListDAO());
            }
        }

        public void delete(Transactions? transactions)
        {
            transactionsManager.delete(transactions?.toDAO());
        }

        public List<Transactions?>? getByAccount(int? id)
        {
            return transactionsManager.getByAccount(id)?.toListBO();
        }

        public List<Transactions?>? getByAccountOrderByOrderDesc(int? id)
        {
            return transactionsManager.getByAccountOrderByOrdenDesc(id)?.toListBO();
        }

        public List<Transactions?>? getByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {
            return transactionsManager.getByAccountOrderByOrdenDesc(id, startIndex, nPage)?.toListBO();
        }

        public List<Transactions?>? getByAccount(Accounts? accounts)
        {
            return getByAccount(accounts?.id);
        }

        public List<Transactions?>? getByPerson(int? id)
        {
            return transactionsManager.getByPerson(id)?.toListBO();
        }

        public List<Transactions?>? getByPerson(Persons? person)
        {
            return getByPerson(person?.id);
        }

        public int getNextID()
        {
            return transactionsManager.getNextID();
        }

        public double createOrden(Transactions transactions)
        {
            return Convert.ToDouble(
                    transactions.date?.Year.ToString("0000")
                    + transactions.date?.Month.ToString("00")
                    + transactions.date?.Day.ToString("00")
                    + transactions.id.ToString("000000")
                    + (transactions.amountIn != 0 ? "1" : "0"));
        }

        public void saveChanges(ref Transactions? transactions)
        {
            transactions.amountIn ??= 0;

            transactions.amountOut ??= 0;

            updateTranfer(transactions);
            updateTranferFromSplit(transactions);
            transactions = update(transactions);
            DependencyConfig.iPersonsService.setCategoryDefault(transactions.person);
            refreshBalanceTransactions(transactions);
        }

        public void refreshBalanceTransactions(Transactions? tUpdate, bool dateFilter = false)
        {
            var tList = getByAccountOrderByOrderDesc(tUpdate.accountid);
            decimal? balanceTotal = 0;

            if (tList != null)
            {
                balanceTotal = tList.Sum(x => x.amount);
            }

            if (tUpdate != null && tUpdate.date != null)
            {
                List<Transactions?>? aux = tList?.Where(x => x.date >= tUpdate?.date.addDay(-1) || dateFilter).ToList();
                for (int i = 0; i < aux.Count; i++)
                {
                    if (aux[i].amount != null)
                    {
                        aux[i].balance = balanceTotal;
                        balanceTotal -= aux[i].amount;
                    }
                }
                updateList(aux);
            }
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
                refreshBalanceTransactions(tContraria);
            }
            else if (transactions.tranferid == null &&
                transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                transactions.tranferid = getNextID();

                Transactions? tContraria = new()
                {
                    date = transactions.date,
                    accountid = DependencyConfig.iAccountsService.getByCategoryId(transactions.categoryid)?.id,
                    personid = transactions.personid,
                    categoryid = DependencyConfig.iAccountsService.getByID(transactions.accountid)?.categoryid,
                    memo = transactions.memo,
                    tagid = transactions.tagid,
                    amountIn = transactions.amountOut,
                    amountOut = transactions.amountIn
                };

                tContraria.tranferid = transactions.id != 0 ? transactions.id : getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                update(tContraria);
                refreshBalanceTransactions(tContraria);

            }
            else if (transactions.tranferid != null &&
                transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = DependencyConfig.iAccountsService.getByCategoryId(transactions.categoryid)?.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = DependencyConfig.iAccountsService.getByID(transactions.accountid)?.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    update(tContraria);
                    refreshBalanceTransactions(tContraria);
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
                Splits? tContraria = DependencyConfig.iSplitsService.getByID(transactions.tranferSplitid);
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
                    DependencyConfig.iSplitsService.update(tContraria);
                }
            }
        }

        public void updateTransactionAfterSplits(Transactions? transactions)
        {
            List<Splits?>? lSplits = transactions.splits ?? DependencyConfig.iSplitsService.getbyTransactionid(transactions.id);

            if (lSplits != null && lSplits.Count != 0)
            {
                transactions.amountIn = 0;
                transactions.amountOut = 0;

                foreach (Splits? splits in lSplits)
                {
                    transactions.amountIn += splits.amountIn == null ? 0 : splits.amountIn;
                    transactions.amountOut += splits.amountOut == null ? 0 : splits.amountOut;
                }

                transactions.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactions.category = DependencyConfig.iCategoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if (transactions.categoryid is not null
                and ((int)CategoriesService.eSpecialCategories.Split))
            {
                transactions.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
                transactions.category = DependencyConfig.iCategoriesService.getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            }

            if (transactions.id == 0)
            {
                update(transactions);
                foreach (Splits? splits in lSplits)
                {
                    splits.transactionid = transactions.id;
                    DependencyConfig.iSplitsService.update(splits);
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
                    accountid = DependencyConfig.iAccountsService.getByCategoryId(splits.categoryid)?.id,
                    personid = transactions.personid,
                    categoryid = DependencyConfig.iAccountsService.getByID(transactions.accountid).categoryid,
                    memo = splits.memo,
                    tagid = transactions.tagid,
                    amountIn = splits.amountOut,
                    amountOut = splits.amountIn,
                    tranferSplitid = splits.id != 0 ? splits.id : getNextID() + 1,
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
                    tContraria.accountid = DependencyConfig.iAccountsService.getByCategoryId(splits.categoryid)?.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = DependencyConfig.iAccountsService.getByID(transactions.accountid).categoryid;
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
