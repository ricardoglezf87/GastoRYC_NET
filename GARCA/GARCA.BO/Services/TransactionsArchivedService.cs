using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using GARCA.View.Views.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class TransactionsArchivedService
    {
        #region Propiedades y Contructor

        private readonly TransactionsArchivedManager transactionsManager;

        public TransactionsArchivedService()
        {
            transactionsManager = new TransactionsArchivedManager();
        }

        #endregion Propiedades y Contructor

        #region TransactionsArchivedActions

        public HashSet<TransactionsArchived?>? GetAll()
        {
            return transactionsManager.GetAll()?.ToHashSetBo();
        }

        public HashSet<TransactionsArchived?>? GetAllOpenned()
        {
            return transactionsManager.GetAllOpenned()?.ToHashSetBo();
        }

        public IEnumerable<TransactionsArchived>? GetAllOpennedOrderByOrderDesc(int startIndex, int nPage)
        {   //TODO: Ver si podemos saltar esta conversion
            return transactionsManager.GetAllOpennedOrderByOrdenDesc(startIndex, nPage)?.ToSortedSetBo();
        }

        public TransactionsArchived? GetById(int? id)
        {
            return (TransactionsArchived?)transactionsManager.GetById(id);
        }

        private HashSet<TransactionsArchived?>? GetByInvestmentProduct(int? id)
        {
            return transactionsManager.GetByInvestmentProduct(id)?.ToHashSetBo();
        }

        public HashSet<TransactionsArchived?>? GetByInvestmentProduct(InvestmentProducts? investment)
        {
            return GetByInvestmentProduct(investment.Id);
        }

        public TransactionsArchived? Update(TransactionsArchived transactions)
        {
            return (TransactionsArchived?)transactionsManager.Update(transactions.ToDao());
        }

        private void UpdateList(List<TransactionsArchived?>? lObj)
        {
            transactionsManager.UpdateList(lObj.ToListDao());
        }

        public void Delete(TransactionsArchived? transactions)
        {
            transactionsManager.Delete(transactions?.ToDao());
        }

        public HashSet<TransactionsArchived?>? GetByAccount(int? id)
        {
            return transactionsManager.GetByAccount(id)?.ToHashSetBo();
        }

        private IEnumerable<TransactionsArchived?>? GetByAccountOrderByOrderDesc(int? id)
        {
            return transactionsManager.GetByAccountOrderByOrdenDesc(id)?.ToSortedSetBo();
        }

        public IEnumerable<TransactionsArchived>? GetByAccountOrderByOrderDesc(int? id, int startIndex, int nPage)
        {   //TODO: Ver si podemos saltar esta conversion
            return transactionsManager.GetByAccountOrderByOrdenDesc(id, startIndex, nPage)?.ToSortedSetBo();
        }

        public HashSet<TransactionsArchived?>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
        }

        private HashSet<TransactionsArchived?>? GetByPerson(int? id)
        {
            return transactionsManager.GetByPerson(id)?.ToHashSetBo();
        }

        public HashSet<TransactionsArchived?>? GetByPerson(Persons? person)
        {
            return GetByPerson(person?.Id);
        }

        public async Task ArchiveTransactions(DateTime date)
        {
            IEnumerable<Transactions?>? lTrans = await Task.Run(() => DependencyConfig.TransactionsService.GetAll()?.Where(x => x.Date != null && x.Date <= date));
            if (lTrans != null)
            {

                LoadDialog loadDialog = new(lTrans.Count());
                loadDialog.Show();

                foreach (var trans in lTrans)
                {
                    if (trans != null)
                    {
                        TransactionsArchived? tArchived;
                        tArchived = await Task.Run(() => Update(trans.ToArchived()));
                        HashSet<Splits?>? lSplits = await Task.Run(() => DependencyConfig.SplitsService.GetbyTransactionid(trans.Id));
                        if (lSplits != null)
                        {
                            loadDialog.setMax(lSplits.Count);

                            foreach (var splits in lSplits)
                            {
                                SplitsArchived sArchived = splits.ToArchived();
                                sArchived.Transactionid = tArchived.Id;
                                sArchived.Transaction = tArchived;
                                await Task.Run(() => DependencyConfig.SplitsArchivedService.Update(sArchived));
                                loadDialog.PerformeStep();
                            }
                        }
                    }
                    loadDialog.PerformeStep();
                }

                loadDialog.Close();
            }

        }

        private int GetNextId()
        {
            return transactionsManager.GetNextId();
        }

        private double CreateOrden(TransactionsArchived transactions)
        {
            return Convert.ToDouble(
                    transactions.Date?.Year.ToString("0000")
                    + transactions.Date?.Month.ToString("00")
                    + transactions.Date?.Day.ToString("00")
                    + transactions.Id.ToString("000000")
                    + (transactions.AmountIn != 0 ? "1" : "0"));
        }

        public void SaveChanges(ref TransactionsArchived? transactions)
        {
            transactions.AmountIn ??= 0;

            transactions.AmountOut ??= 0;

            UpdateTranfer(transactions);
            UpdateTranferFromSplit(transactions);
            transactions = Update(transactions);
            DependencyConfig.PersonsService.SetCategoryDefault(transactions.Person);            
        }

        public void RefreshBalanceTransactionsArchived(TransactionsArchived? tUpdate, bool dateFilter = false, bool pararRec = false)
        {
            var tList = GetByAccountOrderByOrderDesc(tUpdate.Accountid);
            decimal? balanceTotal = 0;

            if (tList != null)
            {
                balanceTotal = tList.Sum(x => x.Amount);
            }

            if (tUpdate.Date != null)
            {
                var aux = tList?.Where(x => x.Date >= tUpdate.Date.AddDay(-1) || dateFilter).ToList();
                for (var i = 0; i < aux.Count; i++)
                {
                    if (aux[i].Amount != null)
                    {
                        aux[i].Balance = balanceTotal;
                        balanceTotal -= aux[i].Amount;
                    }
                    aux[i].Orden = CreateOrden(aux[i]);

                    if (!pararRec)
                    {
                        if (tUpdate.Tranferid != null)
                        {
                            RefreshBalanceTransactionsArchived(DependencyConfig.TransactionsArchivedService.GetById(tUpdate.Tranferid), false, true);
                        }

                        HashSet<Splits?>? lsplits = DependencyConfig.SplitsService.GetbyTransactionid(aux[i].Id);

                        if (lsplits != null)
                        {
                            foreach (var splits in lsplits.Where(splits => splits.Tranferid != null))
                            {
                                RefreshBalanceTransactionsArchived(DependencyConfig.TransactionsArchivedService.GetById(splits.Tranferid), false, true);
                            }
                        }
                    }
                }
                UpdateList(aux);
            }
        }

        private void UpdateTranfer(TransactionsArchived transactions)
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

                TransactionsArchived tContraria = new()
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
                    RefreshBalanceTransactionsArchived(tContraria);
                }
            }
        }

        #endregion

        #region SplitsActions

        private void UpdateTranferFromSplit(TransactionsArchived transactions)
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

        public void UpdateTransactionAfterSplits(TransactionsArchived? transactions)
        {
            var lSplits = transactions.Splits ?? DependencyConfig.SplitsArchivedService.GetbyTransactionid(transactions.Id);

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
                    DependencyConfig.SplitsArchivedService.Update(splits);
                }
            }
            else
            {
                Update(transactions);
            }
        }

        public void UpdateTranferSplits(TransactionsArchived? transactions, ref Splits splits)
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

                TransactionsArchived tContraria = new()
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
