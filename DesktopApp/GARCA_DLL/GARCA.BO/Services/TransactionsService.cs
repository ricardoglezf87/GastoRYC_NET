using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using System.Runtime.Intrinsics.X86;

namespace GARCA.BO.Services
{
    public class TransactionsService
    {
        #region Propiedades y Contructor

        private readonly TransactionsManager transactionsManager;

        public TransactionsService()
        {
            transactionsManager = new TransactionsManager();
        }

        #endregion Propiedades y Contructor

        #region TransactionsActions

        public HashSet<Transactions?>? GetAll()
        {
            return transactionsManager.GetAll()?.ToHashSetBo();
        }

        public HashSet<Transactions?>? GetAllOpenned()
        {
            return transactionsManager.GetAllOpenned()?.ToHashSetBo();
        }

        public IEnumerable<Transactions>? GetAllOpennedOrderByOrderDesc(int startIndex, int nPage)
        {   //TODO: Ver si podemos saltar esta conversion
            return transactionsManager.GetAllOpennedOrderByOrdenDesc(startIndex, nPage)?.ToSortedSetBo();
        }

        public Transactions? GetById(int? id)
        {
            return (Transactions?)transactionsManager.GetById(id);
        }

        private HashSet<Transactions?>? GetByInvestmentProduct(int? id)
        {
            return transactionsManager.GetByInvestmentProduct(id)?.ToHashSetBo();
        }

        public HashSet<Transactions?>? GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return GetByInvestmentProduct(investment.Id);
        }

        public Transactions? Update(Transactions transactions)
        {
            transactions.Date = transactions.Date.RemoveTime();
            transactions.Orden = CreateOrden(transactions);
            return (Transactions?)transactionsManager.Update(transactions.ToDao());
        }

        private void UpdateList(List<Transactions?>? lObj)
        {
            transactionsManager.UpdateList(lObj.ToListDao());
        }

        private void UpdateList(IEnumerable<Transactions?>? lObj)
        {
            transactionsManager.UpdateList(lObj.ToListDao());
        }

        public void Delete(Transactions? transactions)
        {
            transactionsManager.Delete(transactions?.ToDao());
        }
        public void Delete(int? id)
        {
            transactionsManager.Delete(id);
        }

        public HashSet<Transactions?>? GetByAccount(int? id)
        {
            return transactionsManager.GetByAccount(id)?.ToHashSetBo();
        }

        private IEnumerable<Transactions?>? GetByAccountOrderByOrderDesc(int? id)
        {
            return transactionsManager.GetByAccountOrderByOrdenDesc(id)?.ToSortedSetBo();
        }

        public IEnumerable<Transactions>? GetByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {   //TODO: Ver si podemos saltar esta conversion
            return transactionsManager.GetByAccountOrderByOrdenDesc(id, startIndex, nPage)?.ToSortedSetBo();
        }

        public HashSet<Transactions?>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
        }

        public HashSet<Transactions?>? GetByPerson(int? id)
        {
            return transactionsManager.GetByPerson(id)?.ToHashSetBo();
        }

        public HashSet<Transactions?>? GetByPerson(Persons? person)
        {
            return GetByPerson(person?.Id);
        }

        private int GetNextId()
        {
            return transactionsManager.GetNextId();
        }

        private double CreateOrden(Transactions transactions)
        {
            return Convert.ToDouble(
                    transactions.Date?.Year.ToString("0000")
                    + transactions.Date?.Month.ToString("00")
                    + transactions.Date?.Day.ToString("00")
                    + transactions.Id.ToString("000000")
                    + (transactions.AmountIn != 0 ? "1" : "0"));
        }

        public async Task<Transactions?> SaveChanges(Transactions? transactions)
        {
            transactions.AmountIn ??= 0;

            transactions.AmountOut ??= 0;

            UpdateTranfer(transactions);
            UpdateTranferFromSplit(transactions);
            transactions = Update(transactions);
            await Task.Run(() => DependencyConfig.PersonsService.SetCategoryDefault(transactions.Personid));
            return transactions;
        }

        public async Task RefreshBalanceTransactions(Accounts acc)
        {
            decimal? balanceTotal = 0;
            var tList = await Task.Run(() => GetByAccountOrderByOrderDesc(acc.Id));

            if (tList != null)
            {
                balanceTotal = tList.Sum(x => x.Amount);

                foreach (var t in tList)
                {
                    if (t.Amount != null)
                    {
                        t.Balance = balanceTotal;
                        balanceTotal -= t.Amount;
                    }
                    t.Orden = CreateOrden(t);
                }
                await Task.Run(() => UpdateList(tList));
            }
        }

        public async Task RefreshBalanceAllTransactions()
        {
            foreach(var acc in await Task.Run(() => DependencyConfig.AccountsService.GetAll()))
            {
                await RefreshBalanceTransactions(acc);
            }
        }

        private void UpdateTranfer(Transactions transactions)
        {
            if (transactions.Tranferid != null &&
                transactions.Category.CategoriesTypesid != (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = GetById(transactions.Tranferid);
                if (tContraria != null)
                {
                    Delete(tContraria);
                }
                transactions.Tranferid = null;
            }
            else if (transactions.Tranferid == null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                transactions.Tranferid = GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    Accountid = DependencyConfig.AccountsService.GetByCategoryId(transactions.Categoryid)?.Id,
                    Personid = transactions.Personid,
                    Categoryid = DependencyConfig.AccountsService.GetById(transactions.Accountid)?.Categoryid,
                    Memo = transactions.Memo,
                    Tagid = transactions.Tagid,
                    AmountIn = transactions.AmountOut,
                    AmountOut = transactions.AmountIn
                };

                tContraria.Tranferid = transactions.Id != 0 ? transactions.Id : GetNextId() + 1;

                tContraria.TransactionStatusid = transactions.TransactionStatusid;

                Update(tContraria);
            }
            else if (transactions.Tranferid != null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = GetById(transactions.Tranferid);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.Accountid = DependencyConfig.AccountsService.GetByCategoryId(transactions.Categoryid)?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = DependencyConfig.AccountsService.GetById(transactions.Accountid)?.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.TransactionStatusid = transactions.TransactionStatusid;
                    Update(tContraria);
                    RefreshBalanceAllTransactions();
                }
            }
        }

        #endregion

        #region SplitsActions

        private void UpdateTranferFromSplit(Transactions transactions)
        {
            if (transactions.TranferSplitid != null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = DependencyConfig.SplitsService.GetById(transactions.TranferSplitid);
                if (tContraria != null)
                {
                    tContraria.Transaction.Date = transactions.Date;
                    tContraria.Transaction.Personid = transactions.Personid;
                    tContraria.Categoryid = transactions.Account.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.Transaction.TransactionStatusid = transactions.TransactionStatusid;
                    DependencyConfig.SplitsService.Update(tContraria);
                }
            }
        }

        public void UpdateTransactionAfterSplits(Transactions? transactions)
        {
            var lSplits = transactions.Splits ?? DependencyConfig.SplitsService.GetbyTransactionid(transactions.Id);

            if (lSplits != null && lSplits.Count != 0)
            {
                transactions.AmountIn = 0;
                transactions.AmountOut = 0;

                foreach (var splits in lSplits)
                {
                    transactions.AmountIn += splits.AmountIn ?? 0;
                    transactions.AmountOut += splits.AmountOut ?? 0;
                }

                transactions.Categoryid = (int)CategoriesService.ESpecialCategories.Split;
                transactions.Category = DependencyConfig.CategoriesService.GetById((int)CategoriesService.ESpecialCategories.Split);
            }
            else if (transactions.Categoryid is not null
                and (int)CategoriesService.ESpecialCategories.Split)
            {
                transactions.Categoryid = (int)CategoriesService.ESpecialCategories.WithoutCategory;
                transactions.Category = DependencyConfig.CategoriesService.GetById((int)CategoriesService.ESpecialCategories.WithoutCategory);
            }

            if (transactions.Id == 0)
            {
                Update(transactions);
                foreach (var splits in lSplits)
                {
                    splits.Transactionid = transactions.Id;
                    DependencyConfig.SplitsService.Update(splits);
                }
            }
            else
            {
                Update(transactions);
            }
        }

        public void UpdateTranferSplits(Transactions? transactions, ref Splits splits)
        {
            Categories? category = DependencyConfig.CategoriesService.GetById(splits.Categoryid);

            if (splits.Tranferid != null &&
                category?.CategoriesTypesid != (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = GetById(splits.Tranferid);
                if (tContraria != null)
                {
                    Delete(tContraria);
                }
                splits.Tranferid = null;
            }
            else if (splits.Tranferid == null &&
                category?.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                splits.Tranferid = GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    Accountid = DependencyConfig.AccountsService.GetByCategoryId(splits.Categoryid)?.Id,
                    Personid = transactions.Personid,
                    Categoryid = DependencyConfig.AccountsService.GetById(transactions.Accountid).Categoryid,
                    Memo = splits.Memo,
                    Tagid = transactions.Tagid,
                    AmountIn = splits.AmountOut,
                    AmountOut = splits.AmountIn,
                    TranferSplitid = splits.Id != 0 ? splits.Id : GetNextId() + 1,
                    TransactionStatusid = transactions.TransactionStatusid
                };

                Update(tContraria);

            }
            else if (splits.Tranferid != null &&
                category?.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = GetById(splits.Tranferid);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.Accountid = DependencyConfig.AccountsService.GetByCategoryId(splits.Categoryid)?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = DependencyConfig.AccountsService.GetById(transactions.Accountid).Categoryid;
                    tContraria.Memo = splits.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = splits.AmountOut ?? 0;
                    tContraria.AmountOut = splits.AmountIn ?? 0;
                    tContraria.TransactionStatusid = transactions.TransactionStatusid;
                    Update(tContraria);
                }
            }
        }

        #endregion

    }
}
