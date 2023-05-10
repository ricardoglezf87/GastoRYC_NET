using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TransactionsService
    {
        public List<Transactions>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactions?.ToList();
        }

        public Transactions? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactions?.FirstOrDefault(x => id.Equals(x.id));
        }

        public List<Transactions>? getByInvestmentProduct(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactions?.Where(x => id.Equals(x.investmentProductsid)).ToList();
        }

        public List<Transactions>? getByInvestmentProduct(InvestmentProducts? investment)
        {
            return RYCContextService.getInstance().BBDD.transactions?.Where(x => investment.id.Equals(x.investmentProductsid)).ToList();
        }

        public void update(Transactions transactions)
        {
            RYCContextService.getInstance().BBDD.Update(transactions);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Transactions? transactions)
        {
            if (transactions != null)
            {
                RYCContextService.getInstance().BBDD.Remove(transactions);
                RYCContextService.getInstance().BBDD.SaveChanges();
            }
        }       
    }
}
