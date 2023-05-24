using DAOLib.Models;
using DAOLib.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DAOLib.Managers
{
    public class TransactionsManager : ManagerBase<TransactionsDAO>
    {

#pragma warning disable CS8603
        public override Expression<Func<TransactionsDAO, object>>[] getIncludes()
        {
            return new Expression<Func<TransactionsDAO, object>>[]
            {
                a => a.account,
                a => a.person,
                a => a.category,
                a => a.tag,
                a => a.investmentProducts,
                a => a.transactionStatus
            };
        }
#pragma warning restore CS8603

        public List<TransactionsDAO>? getByAccount(AccountsDAO? accounts)
        {
            return getByAccount(accounts?.id);
        }

        public List<TransactionsDAO>? getByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?.Where(x => id.Equals(x.accountid))?.ToList();
            }
        }
        public List<TransactionsDAO>? getByAccountOrderByDateDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.accountid))?
                    .OrderByDescending(x => x.date)?.ToList();
            }
        }

        public List<TransactionsDAO>? getAllOpenned()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?.Where(x => !x.account.closed.HasValue || !x.account.closed.Value)?.ToList();
            }
        }

        public List<TransactionsDAO>? getAllOpennedOrderByDateDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?
                    .Where(x => !x.account.closed.HasValue || !x.account.closed.Value)?
                    .OrderByDescending(x => x.date)?.ToList();
            }
        }

        public List<TransactionsDAO>? getByPerson(PersonsDAO? persons)
        {
            return getByPerson(persons?.id);
        }

        public List<TransactionsDAO>? getByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?.Where(x => id.Equals(x.personid))?.ToList();
            }
        }

        public List<TransactionsDAO>? getByInvestmentProduct(InvestmentProductsDAO? investment)
        {
            return getByInvestmentProduct(investment?.id);
        }

        public List<TransactionsDAO>? getByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                return getEntyWithInclude(repository)?.Where(x => id.Equals(x.investmentProductsid))?.ToList();
            }
        }

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                    GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";

                unitOfWork.getDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                int id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
