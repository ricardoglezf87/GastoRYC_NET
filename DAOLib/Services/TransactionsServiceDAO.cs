using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TransactionsServiceDAO
    {
        public List<TransactionsDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.ToList();
        }

        public TransactionsDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.FirstOrDefault(x => id.Equals(x.id));
        }

        public List<TransactionsDAO>? getByInvestmentProduct(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => id.Equals(x.investmentProductsid)).ToList();
        }

        public List<TransactionsDAO>? getByInvestmentProduct(InvestmentProductsDAO? investment)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => investment.id.Equals(x.investmentProductsid)).ToList();
        }

        public void update(TransactionsDAO transactions)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(transactions);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(TransactionsDAO? transactions)
        {
            if (transactions != null)
            {
                RYCContextServiceDAO.getInstance().BBDD.Remove(transactions);
                RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
            }
        }       
    }
}
