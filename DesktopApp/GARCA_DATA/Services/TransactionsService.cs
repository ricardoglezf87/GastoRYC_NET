using Dapper;
using GARCA.DAO.Repositories;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Services;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class TransactionsService : IServiceCache<Transactions>
    {

        protected override IEnumerable<Transactions>? GetAllCache()
        {
            return iRycContextService.getConnection().Query<Transactions,Accounts,Categories,TransactionsStatus,Persons,Tags,InvestmentProducts,Transactions>(
                @"
                    select * 
                    from Transactions    
                        inner join Accounts on Accounts.Id = Transactions.Accountid 
                        inner join Categories on Categories.Id = Transactions.Categoryid 
                        inner join TransactionsStatus on TransactionsStatus.Id = Transactions.TransactionStatusid                                       
                        left join Persons on Categories.Id = Transactions.Personid                         
                        left join Tags on Tags.Id = Transactions.Tagid                         
                        left join InvestmentProducts on InvestmentProducts.Id = Transactions.InvestmentProductsid
                "
                , (transactions, accounts, categories, transactionsStatus, persons, tags, investmentProducts) =>
                {
                    transactions.Account = accounts;
                    transactions.Category = categories;
                    transactions.Person = persons;
                    transactions.Tag = tags;
                    transactions.TransactionStatus = transactionsStatus;
                    transactions.InvestmentProducts = investmentProducts;
                    return transactions;
                }).AsEnumerable();
        }

        #region TransactionsActions

        public HashSet<Transactions>? GetAllOpenned()
        {
            return GetAll()?.Where(x => !x.Account.Closed.HasValue 
                || !x.Account.Closed.Value)?.ToHashSet();
        }

        private HashSet<Transactions>? GetByInvestmentProduct(int? id)
        {
            return GetAll().Where(x => id.Equals(x.InvestmentProductsid))?.ToHashSet();
        }

        public HashSet<Transactions>? GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return GetByInvestmentProduct(investment.Id);
        }

        public override Transactions? Update(Transactions transactions)
        {
            transactions.Date = transactions.Date.RemoveTime();
            transactions.Orden = CreateOrden(transactions);
            return base.Update(transactions);
        }         

        private void UpdateList(IEnumerable<Transactions?>? lObj)
        {
            foreach (var item in lObj)
            {
                if (item != null)
                {
                    Update(item);
                }
            }
            SaveChanges();
        }
        public IEnumerable<Transactions>? GetAllOpennedOrderByOrdenDesc()
        {
            return GetAll()?.OrderByDescending(x => x.Orden);
        }

        public IEnumerable<Transactions>? GetAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return GetAllOpennedOrderByOrdenDesc()?.Skip(startIndex).Take(nPage);
        }

        private IEnumerable<Transactions>? GetByAccountOrderByOrderDesc(int? id)
        {
            return GetAll().Where(x => id.Equals(x.Accountid))
                    .OrderByDescending(x => x.Orden);
        }

        public IEnumerable<Transactions>? GetByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {
            return GetByAccountOrderByOrderDesc(id)?.Skip(startIndex).Take(nPage);
        }

        public HashSet<Transactions>? GetByAccount(int? id)
        {
            return GetAll().Where(x => id.Equals(x.Accountid))?.ToHashSet();
        }

        public HashSet<Transactions>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
        }

        public HashSet<Transactions>? GetByPerson(int? id)
        {
            return GetAll().Where(x => id.Equals(x.Personid))?.ToHashSet();
        }

        public HashSet<Transactions>? GetByPerson(Persons? person)
        {
            return GetByPerson(person?.Id);
        }

        private int GetNextId()
        {
            return 999999;
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
            UpdateTranferFromSplit(transactions);
            transactions = Update(transactions);
            await Task.Run(() => iPersonsService.SetCategoryDefault(transactions.Personid));
            return transactions;
        }

        public async Task RefreshBalanceTransactions(Accounts? acc)
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
            foreach (var acc in await Task.Run(() => iAccountsService.GetAll()))
            {
                await RefreshBalanceTransactions(acc);
            }
        }

        private async Task UpdateTranfer(Transactions transactions)
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
                    Accountid = iAccountsService.GetByCategoryId(transactions.Categoryid)?.Id,
                    Personid = transactions.Personid,
                    Categoryid = iAccountsService.GetById(transactions.Accountid)?.Categoryid,
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
                    tContraria.Accountid = iAccountsService.GetByCategoryId(transactions.Categoryid)?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = iAccountsService.GetById(transactions.Accountid)?.Categoryid;
                    tContraria.Memo = transactions.Memo;
                    tContraria.Tagid = transactions.Tagid;
                    tContraria.AmountIn = transactions.AmountOut;
                    tContraria.AmountOut = transactions.AmountIn;
                    tContraria.TransactionStatusid = transactions.TransactionStatusid;
                    Update(tContraria);
                    await RefreshBalanceAllTransactions();
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
                var tContraria = iSplitsService.GetById(transactions.TranferSplitid);
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
                    iSplitsService.Update(tContraria);
                }
            }
        }

        public void UpdateTransactionAfterSplits(Transactions? transactions)
        {
            var lSplits = transactions.Splits ?? iSplitsService.GetbyTransactionid(transactions.Id);

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
                transactions.Category = iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.Split);
            }
            else if (transactions.Categoryid is (int)CategoriesService.ESpecialCategories.Split)
            {
                transactions.Categoryid = (int)CategoriesService.ESpecialCategories.WithoutCategory;
                transactions.Category = iCategoriesService.GetById((int)CategoriesService.ESpecialCategories.WithoutCategory);
            }

            if (transactions.Id == 0)
            {
                Update(transactions);
                
                if (lSplits == null) return;

                foreach (var splits in lSplits)
                {
                    splits.Transactionid = transactions.Id;
                    iSplitsService.Update(splits);
                }
            }
            else
            {
                Update(transactions);
            }
        }

        public void UpdateTranferSplits(Transactions? transactions, ref Splits splits)
        {
            Categories? category = iCategoriesService.GetById(splits.Categoryid);

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
                    Accountid = iAccountsService.GetByCategoryId(splits.Categoryid)?.Id,
                    Personid = transactions.Personid,
                    Categoryid = iAccountsService.GetById(transactions.Accountid).Categoryid,
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
                    tContraria.Accountid = iAccountsService.GetByCategoryId(splits.Categoryid)?.Id;
                    tContraria.Personid = transactions.Personid;
                    tContraria.Categoryid = iAccountsService.GetById(transactions.Accountid).Categoryid;
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
