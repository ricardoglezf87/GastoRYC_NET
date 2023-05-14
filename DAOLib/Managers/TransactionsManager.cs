using DAOLib.Models;
using DAOLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class TransactionsManager : ManagerBase<TransactionsDAO>
    {
        public List<TransactionsDAO>? getByAccount(AccountsDAO? accounts)
        {
            return getByAccount(accounts?.id);
        }

        public List<TransactionsDAO>? getByAccount(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => id.Equals(x.accountid))?.ToList();
        }

        public List<TransactionsDAO>? getByPerson(PersonsDAO? persons)
        {
            return getByPerson(persons?.id);
        }

        public List<TransactionsDAO>? getByPerson(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => id.Equals(x.personid))?.ToList();
        }

        public List<TransactionsDAO>? getByInvestmentProduct(InvestmentProductsDAO? investment)
        {
            return getByInvestmentProduct(investment?.id);
        }

        public List<TransactionsDAO>? getByInvestmentProduct(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => id.Equals(x.investmentProductsid))?.ToList();
        }       

        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
