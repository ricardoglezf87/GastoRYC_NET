using BOLib.Models;
using BOLib.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BOLib.Helpers;
using BOLib.ModelsView;
using DAOLib.Managers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BOLib.Services
{
    public class TransactionsService
    {
        #region Propiedades y Contructor

        private readonly TransactionsManager transactionsService;
        private readonly PersonsService personsService;
        
        public TransactionsService()
        {
            transactionsService = new();
            personsService = new();
        }

        #endregion Propiedades y Contructor

        #region TransactionsActions

        public List<Transactions>? getAll()
        {
            return transactionsService.getAll()?.toListBO();
        }

        public Transactions? getByID(int? id)
        {
            return (Transactions)transactionsService.getByID(id);
        }

        public List<Transactions>? getByInvestmentProduct(int? id)
        {
            return transactionsService.getByInvestmentProduct(id)?.toListBO();
        }

        public List<Transactions>? getByInvestmentProduct(InvestmentProducts? investment)
        {
            return getByInvestmentProduct(investment.id);
        }

        public void update(Transactions transactions)
        {
            transactions.date = transactions.date.removeTime();
            transactionsService.update(transactions?.toDAO());
        }

        public void delete(Transactions? transactions)
        {
            transactionsService.delete(transactions?.toDAO());
        }

        public List<Transactions>? getByAccount(int? id)
        {
            return transactionsService.getByAccount(id)?.toListBO();
        }

        public List<Transactions>? getByAccount(AccountsView? accounts)
        {
            return getByAccount(accounts?.id);
        }        

        public int getNextID()
        {
            return transactionsService.getNextID();
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
            //TODO: Revisar
            //if (transactions.tranferid != null &&
            //    transactions.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //{
            //    Transactions? tContraria = getByID(transactions.tranferid);
            //    if (tContraria != null)
            //    {
            //        delete(tContraria);
            //    }
            //    transactions.tranferid = null;
            //}
            //else if (transactions.tranferid == null &&
            //    transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //{
            //    transactions.tranferid = getNextID();

            //    Transactions? tContraria = new()
            //    {
            //        date = transactions.date,
            //        accountid = transactions.category.accounts.id,
            //        personid = transactions.personid,
            //        categoryid = servicesContainer.GetInstance<AccountsService>().getByID(transactions.accountid)?.categoryid,
            //        memo = transactions.memo,
            //        tagid = transactions.tagid,
            //        amountIn = transactions.amountOut,
            //        amountOut = transactions.amountIn
            //    };

            //    if (transactions.id != 0)
            //        tContraria.tranferid = transactions.id;
            //    else
            //        tContraria.tranferid = getNextID() + 1;

            //    tContraria.transactionStatusid = transactions.transactionStatusid;

            //    update(tContraria);

            //}
            //else if (transactions.tranferid != null &&
            //    transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //{
            //    Transactions? tContraria = getByID(transactions.tranferid);
            //    if (tContraria != null)
            //    {
            //        tContraria.date = transactions.date;
            //        tContraria.accountid = transactions.category.accounts.id;
            //        tContraria.personid = transactions.personid;
            //        tContraria.categoryid = servicesContainer.GetInstance<AccountsService>().getByID(transactions.accountid)?.categoryid;
            //        tContraria.memo = transactions.memo;
            //        tContraria.tagid = transactions.tagid;
            //        tContraria.amountIn = transactions.amountOut;
            //        tContraria.amountOut = transactions.amountIn;
            //        tContraria.transactionStatusid = transactions.transactionStatusid;
            //        update(tContraria);
            //    }
            //}
        }

        #endregion

        #region SplitsActions

        public void updateTranferFromSplit(Transactions transactions)
        {
            //TODO:Hacer en clase servicios
            //if (transactions.tranferSplitid != null &&
            //    transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //{
            //    Splits? tContraria = servicesContainer.GetInstance<SplitsService>().getByID(transactions.tranferSplitid);
            //    if (tContraria != null)
            //    {
            //        tContraria.transaction.date = transactions.date;
            //        tContraria.transaction.personid = transactions.personid;
            //        tContraria.categoryid = transactions.account.categoryid;
            //        tContraria.memo = transactions.memo;
            //        tContraria.tagid = transactions.tagid;
            //        tContraria.amountIn = transactions.amountOut;
            //        tContraria.amountOut = transactions.amountIn;
            //        tContraria.transaction.transactionStatusid = transactions.transactionStatusid;
            //        //servicesContainer.GetInstance<SplitsService>().update(tContraria);            
            //    }
            //}
        }

        public void updateTransactionAfterSplits(Transactions? transactions)
        {
            //TODO:Hacer en clase servicios
            //List<Splits>? lSplits = transactions.splits ?? servicesContainer.GetInstance<SplitsService>().getbyTransactionid(transactions.id);

            //if (lSplits != null && lSplits.Count != 0)
            //{
            //    transactions.amountIn = 0;
            //    transactions.amountOut = 0;

            //    foreach (Splits splits in lSplits)
            //    {
            //        transactions.amountIn += (splits.amountIn == null ? 0 : splits.amountIn);
            //        transactions.amountOut += (splits.amountOut == null ? 0 : splits.amountOut);
            //    }

            //    transactions.categoryid = (int)CategoriesService.eSpecialCategories.Split;
            //    transactions.category = servicesContainer.GetInstance<CategoriesService>().getByID((int)CategoriesService.eSpecialCategories.Split);
            //}
            //else if (transactions.categoryid != null
            //    && transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            //{
            //    transactions.categoryid = (int)CategoriesService.eSpecialCategories.WithoutCategory;
            //    transactions.category = servicesContainer.GetInstance<CategoriesService>().getByID((int)CategoriesService.eSpecialCategories.WithoutCategory);
            //}

            //if (transactions.id == 0)
            //{
            //    update(transactions);
            //    foreach (Splits splits in lSplits)
            //    {
            //        splits.transactionid = transactions.id;
            //        servicesContainer.GetInstance<SplitsService>().update(splits);
            //    }
            //}
            //else
            //{
            //    update(transactions);
            //}
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
