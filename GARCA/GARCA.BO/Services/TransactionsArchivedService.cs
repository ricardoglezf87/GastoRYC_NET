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

        public HashSet<TransactionsArchived?>? GetByAccount(Accounts? accounts)
        {
            return GetByAccount(accounts?.Id);
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
                                DependencyConfig.SplitsService.Delete(splits);
                                loadDialog.PerformeStep();                                
                            }
                        }
                        DependencyConfig.TransactionsService.Delete(trans);
                    }
                    loadDialog.PerformeStep();
                }

                loadDialog.Close();
            }

        }

        #endregion

    }
}
