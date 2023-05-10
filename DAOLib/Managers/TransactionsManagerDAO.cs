using DAOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class TransactionsManagerDAO : IServiceDAO<TransactionsDAO>
    {
        public List<TransactionsDAO>? getByInvestmentProduct(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => id.Equals(x.investmentProductsid)).ToList();
        }

        public List<TransactionsDAO>? getByInvestmentProduct(InvestmentProductsDAO? investment)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactions?.Where(x => investment.id.Equals(x.investmentProductsid)).ToList();
        }       
    }
}
