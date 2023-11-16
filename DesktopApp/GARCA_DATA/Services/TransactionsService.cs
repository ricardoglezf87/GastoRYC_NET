using Dapper;

using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA.Data.Services;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class TransactionsService : ServiceBase<TransactionsManager,Transactions,Int32>
    {
        #region TransactionsActions

        public async Task<IEnumerable<Transactions>?> GetAllOpenned()
        {
            return (await GetAll())?.Where(x => !x.Account.Closed.HasValue 
                || !x.Account.Closed.Value);
        }

        private async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(int id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.InvestmentProductsid));
        }

        public async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(InvestmentProducts investment)
        {
            return await GetByInvestmentProduct(investment.Id);
        }

        public override async Task<bool> Update(Transactions transactions)
        {
            transactions.Date = transactions.Date.RemoveTime();
            transactions.Orden = CreateOrden(transactions);
            return await base.Update(transactions);
        }         
        
        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc()
        {
            return (await GetAll())?.OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return (await GetAllOpennedOrderByOrdenDesc())?.Skip(startIndex).Take(nPage);
        }

        private async Task<IEnumerable<Transactions>?> GetByAccountOrderByOrderDesc(int? id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.Accountid))
                    .OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {
            return (await GetByAccountOrderByOrderDesc(id))?.Skip(startIndex).Take(nPage);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int? id)
        {
            return  (await GetAll())?.Where(x => id.Equals(x.Accountid));
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(Accounts? accounts)
        {
            return await GetByAccount(accounts?.Id);
        }

        public async Task<IEnumerable<Transactions>?> GetByPerson(int? id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.Personid));
        }

        public async Task<IEnumerable<Transactions>?> GetByPerson(Persons? person)
        {
            return await GetByPerson(person?.Id);
        }

        private async Task<int> GetNextId()
        {
            return await manager.GetNextId();
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

            await UpdateTranfer(transactions);
            await UpdateTranferFromSplit(transactions);
            //TODO: Revisar este caso
            //transactions = await Update(transactions);
            //await iPersonsService.SetCategoryDefault(transactions.Personid ?? -11);
            return transactions;
        }

        public async Task RefreshBalanceTransactions(Accounts? acc)
        {
            decimal? balanceTotal = 0;
            var tList = await GetByAccountOrderByOrderDesc(acc.Id);

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
                await Update(tList);
            }
        }

        public async Task RefreshBalanceAllTransactions()
        {
            foreach (var acc in await iAccountsService.GetAll())
            {
                await RefreshBalanceTransactions(acc);
            }
        }

        private async Task UpdateTranfer(Transactions transactions)
        {
            if (transactions.Tranferid != null &&
                transactions.Category.CategoriesTypesid != (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = await GetById(transactions.Tranferid ?? -99);
                if (tContraria != null)
                {
                    await Delete(tContraria);
                }
                transactions.Tranferid = null;
            }
            else if (transactions.Tranferid == null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                transactions.Tranferid = await GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    Accountid = (await iAccountsService.GetByCategoryId(transactions.Categoryid ?? -99))?.Id,
                    Personid = transactions.Personid,
                    Categoryid = (await iAccountsService.GetById(transactions.Accountid ?? -99))?.Categoryid,
                    Memo = transactions.Memo,
                    Tagid = transactions.Tagid,
                    AmountIn = transactions.AmountOut,
                    AmountOut = transactions.AmountIn
                };

                tContraria.Tranferid = transactions.Id != 0 ? transactions.Id : await GetNextId() + 1;

                tContraria.TransactionStatusid = transactions.TransactionStatusid;

                await Update(tContraria);
            }
            else if (transactions.Tranferid != null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = await GetById(transactions.Tranferid ?? -1);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.Accountid = (await iAccountsService.GetByCategoryId(transactions.Categoryid ?? -99))?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = (await iAccountsService.GetById(transactions.Accountid ?? -99))?.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.TransactionStatusid = transactions.TransactionStatusid;
                    await Update(tContraria);
                    await RefreshBalanceAllTransactions();
                }
            }
        }

        #endregion

        #region SplitsActions

        private async Task UpdateTranferFromSplit(Transactions transactions)
        {
            if (transactions.TranferSplitid != null &&
                transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = await iSplitsService.GetById(transactions.TranferSplitid ?? -99);
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
                    await iSplitsService.Update(tContraria);
                }
            }
        }

        public async Task UpdateTransactionAfterSplits(Transactions? transactions)
        {
            var lSplits = transactions.Splits ?? await iSplitsService.GetbyTransactionid(transactions.Id);

            if (lSplits != null && lSplits.Count() != 0)
            {
                transactions.AmountIn = 0;
                transactions.AmountOut = 0;

                foreach (var splits in lSplits)
                {
                    transactions.AmountIn += splits.AmountIn ?? 0;
                    transactions.AmountOut += splits.AmountOut ?? 0;
                }

                transactions.Categoryid = (int)CategoriesService.ESpecialCategories.Split;
                transactions.Category = await iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.Split);
            }
            else if (transactions.Categoryid is (int)CategoriesService.ESpecialCategories.Split)
            {
                transactions.Categoryid = (int)CategoriesService.ESpecialCategories.WithoutCategory;
                transactions.Category = await iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.WithoutCategory);
            }

            if (transactions.Id == 0)
            {
                await Update(transactions);
                
                if (lSplits == null) return;

                foreach (var splits in lSplits)
                {
                    splits.Transactionid = transactions.Id;
                    iSplitsService.Update(splits);
                }
            }
            else
            {
                await Update(transactions);
            }
        }
        
        public async Task<Splits> UpdateTranferSplits(Transactions? transactions, Splits splits)
        {
            Categories? category = await iCategoriesService.GetById(splits.Categoryid ?? -99);

            if (splits.Tranferid != null &&
                category?.CategoriesTypesid != (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = await GetById(splits.Tranferid ?? -99);
                if (tContraria != null)
                {
                    await Delete(tContraria);
                }
                splits.Tranferid = null;
            }
            else if (splits.Tranferid == null &&
                category?.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                splits.Tranferid = await GetNextId();

                Transactions tContraria = new()
                {
                    Date = transactions.Date,
                    Accountid = (await iAccountsService.GetByCategoryId(splits.Categoryid ?? -99))?.Id,
                    Personid = transactions.Personid,
                    Categoryid = (await iAccountsService.GetById(transactions.Accountid ?? -99)).Categoryid,
                    Memo = splits.Memo,
                    Tagid = transactions.Tagid,
                    AmountIn = splits.AmountOut,
                    AmountOut = splits.AmountIn,
                    TranferSplitid = splits.Id != 0 ? splits.Id : await GetNextId() + 1,
                    TransactionStatusid = transactions.TransactionStatusid
                };

                await Update(tContraria);

            }
            else if (splits.Tranferid != null &&
                category?.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
            {
                var tContraria = await GetById(splits.Tranferid ?? -99);
                if (tContraria != null)
                {
                    tContraria.Date = transactions.Date;
                    tContraria.Accountid = (await iAccountsService.GetByCategoryId(splits.Categoryid ?? -99))?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = (await iAccountsService.GetById(transactions.Accountid ?? -99)).Categoryid;
                    tContraria.Memo = splits.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = splits.AmountOut ?? 0;
                    tContraria.AmountOut = splits.AmountIn ?? 0;
                    tContraria.TransactionStatusid = transactions.TransactionStatusid;
                    await Update(tContraria);
                }
            }

            return splits;
        }

        #endregion

    }
}
